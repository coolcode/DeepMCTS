using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepMCTS
{
    public class MCTS
    {
        Random r = new Random(1337);

        //THE EXECUTING FUNCTION
        public byte GetBestMove(Game game, int player)
        {
            //Setup root and initial variables
            Node root = new Node(null, 0, Opponent(player), 0);
            int startPlayer = player;

            Helper.CopyBytes(root.state, game.board);

            //four phases: descent, roll-out, update and growth done iteratively X times
            //-----------------------------------------------------------------------------------------------------
            for (int iteration = 0; iteration < 1000; iteration++)
            {
                Node current = Selection(root, game);
                int value = Rollout(current, game, startPlayer);
                Update(current, value);
            }

            //Restore game state and return move with highest value
            Helper.CopyBytes(game.board, root.state);

            //Draw tree
            var text= root.DrawTree();
            Console.WriteLine(text);

            //return root.children.Aggregate((i1, i2) => i1.visits > i2.visits ? i1 : i2).action;
            return BestChildUCB(root, 0).action;
        }

        //#1. Select a node if 1: we have more valid feasible moves or 2: it is terminal 
        public Node Selection(Node current, Game game)
        {
            while (!game.Over(current.state))
            {
                List<byte> validMoves = game.GetValidMoves(current.state);

                if (validMoves.Count > current.children.Count)
                    return Expand(current, game);
                else
                    current = BestChildUCB(current, 1.44);
            }

            return current;
        }

        //#1. Helper
        public Node BestChildUCB(Node current, double C)
        {
            Node bestChild = null;
            double best = double.NegativeInfinity;

            foreach (Node child in current.children)
            {
                double UCB1 = ((double)child.value / (double)child.visits) + C * Math.Sqrt((2.0 * Math.Log((double)current.visits)) / (double)child.visits);
                child.ucb1 = UCB1;

                if (UCB1 > best)
                {
                    bestChild = child;
                    best = UCB1;
                }
            }

            return bestChild;
        }

        //#2. Expand a node by creating a new move and returning the node
        public Node Expand(Node current, Game game)
        {
            //Copy current state to the game
            Helper.CopyBytes(game.board, current.state);

            List<byte> validMoves = game.GetValidMoves(current.state);

            for (int i = 0; i < validMoves.Count; i++)
            {
                //We already have evaluated this move
                if (current.children.Exists(a => a.action == validMoves[i]))
                    continue;

                int playerActing = Opponent(current.PlayerTookAction);

                Node node = new Node(current, validMoves[i], playerActing, current.depth + 1);
                current.children.Add(node);

                //Do the move in the game and save it to the child node
                game.Mark(playerActing, validMoves[i]);
                Helper.CopyBytes(node.state, game.board);

                //Return to the previous game state
                Helper.CopyBytes(game.board, current.state);

                return node;
            }

            throw new Exception("Error");
        }

        //#3. Roll-out. Simulate a game with a given policy and return the value
        public int Rollout(Node current, Game game, int startPlayer)
        {
            Helper.CopyBytes(game.board, current.state);

            //If this move is terminal and the opponent wins, this means we have previously made a move where the opponent can always find a move to win.. not good
            if (game.GetWinner() == Opponent(startPlayer))
            {
                current.parent.value = int.MinValue;
                return 0;
            }

            int player = Opponent(current.PlayerTookAction);

            //Do the policy until a winner is found for the first (change?) node added
            while (game.GetWinner() == 0)
            {
                //Random
                List<byte> moves = game.GetValidMoves();
                byte move = moves[r.Next(0, moves.Count)];
                game.Mark(player, move);
                player = Opponent(player);
            }

            if (game.GetWinner() == startPlayer || game.GetWinner() == 3)
                return 1;

            return 0;

        }

        //#4. Update
        public void Update(Node current, int value)
        {
            do
            {
                current.visits++;
                current.value += value;
                current = current.parent;
            }
            while (current != null);
        }



        public int Opponent(int player)
        {
            if (player == 1)
                return 2;
            return 1;
        }

    }
}
