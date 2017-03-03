using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepMCTS
{
    public class MCTS
    {
        private static readonly Random rand = new Random();

        public void Train(int versionNo)
        {
            var game = new Game();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "json", $"mcts.ttt.v{versionNo}.9.json");

            var root = Node.Load(path);

            int startPlayer = 1;

            root.bits = game.BitBoard;

            //four phases: descent, roll-out, update and growth done iteratively X times
            //-----------------------------------------------------------------------------------------------------
            for (int iteration = 0; iteration < 1000; iteration++)
            {
                Node current = Selection(root, game);
                int value = Rollout(current, game, startPlayer);
                Update(current, value);
            }

            //Restore game state and return move with highest value
            game.BitBoard = root.bits;

            var index = game.GetValidMoves().Count;
            path = Path.Combine(Directory.GetCurrentDirectory(), "json", $"mcts.ttt.v{versionNo + 1}.9.json");
            root.Save(path);

            var bestMove = BestChildUCB(root, 0).action;

            Console.WriteLine($"Best Move:{bestMove}");
            foreach (var item in root.children.OrderByDescending(c => c.ucb1))
            {
                Console.WriteLine($"Best Move:{item}");
            }
        }

        private int versionNo = 0;
        //THE EXECUTING FUNCTION
        public byte GetBestMove_RealtimeTraining(Game game, int player)
        {
            //Setup root and initial variables
            var path = Path.Combine(Directory.GetCurrentDirectory(), "json", $"mcts.ttt.{game.Depth}.{game.BitBoard}.json");
            Node root = null;
            if (File.Exists(path))
            {
                root = Node.Load(path);
                root = root.Search(game.BitBoard);
                versionNo++;
            }


            if (root == null)
            {
                root = new Node(null, 0, Opponent(player), 0);
            }

            root.player = (byte)Opponent(player);
            var startPlayer = player;

            root.bits = game.BitBoard;

            //four phases: descent, roll-out, update and growth done iteratively X times
            //-----------------------------------------------------------------------------------------------------
            for (int iteration = 0; iteration < 10; iteration++)
            {
                Node current = Selection(root, game);
                int value = Rollout(current, game, startPlayer);
                Update(current, value);
            }

            //Restore game state and return move with highest value
            game.BitBoard = root.bits;
            
            root.Save(path);

            //return root.children.Aggregate((i1, i2) => i1.visits > i2.visits ? i1 : i2).action;
            return BestChildUCB(root, 0).action;
        }

        //THE EXECUTING FUNCTION
        public byte GetBestMove(Game game, int player)
        {
            //Setup root and initial variables
            Node root = new Node(null, 0, Opponent(player), 0);
            int startPlayer = player;

            root.bits = game.BitBoard;


            //four phases: descent, roll-out, update and growth done iteratively X times
            //-----------------------------------------------------------------------------------------------------
            for (int iteration = 0; iteration < 10000; iteration++)
            {
                Node current = Selection(root, game);
                int value = Rollout(current, game, startPlayer);
                Update(current, value);
            }

            //Restore game state and return move with highest value
            game.BitBoard = root.bits;

            //var index = game.GetValidMoves().Count;
            //var path = Path.Combine(Directory.GetCurrentDirectory(), "json", $"mcts.ttt.{index}.json");
            //root.Save(path);

            return BestChildUCB(root, 0).action;
        }

        //#1. Select a node if 1: we have more valid feasible moves or 2: it is terminal 
        public Node Selection(Node current, Game game)
        {
            while (!game.Over(current.bits))
            {
                List<byte> validMoves = game.GetValidMoves(current.bits);

                if (validMoves.Count > current.children.Count)
                {
                    return Expand(current, game);
                }
                else
                    current = BestChildUCB(current, 1); /*1.44*/
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
        private Node Expand(Node current, Game game)
        {
            //Copy current state to the game
            game.BitBoard = current.bits;

            List<byte> validMoves = game.GetValidMoves(current.bits);

            for (int i = 0; i < validMoves.Count; i++)
            {
                //We already have evaluated this move
                if (current.children.Exists(a => a.action == validMoves[i]))
                    continue;

                int playerActing = Opponent(current.player);

                Node node = new Node(current, validMoves[i], playerActing, current.depth + 1);
                current.children.Add(node);

                //Do the move in the game and save it to the child node
                game.Mark(playerActing, validMoves[i]);

                node.bits = game.BitBoard;
                game.BitBoard = current.bits;

                return node;
            }

            throw new Exception("Error");
        }

        private Node Expand(Node current, Game game, byte pos)
        {
            int playerActing = Opponent(current.player);

            Node node = new Node(current, pos, playerActing, current.depth + 1);
            current.children.Add(node);

            //Do the move in the game and save it to the child node
            game.Mark(playerActing, pos);
            node.bits = game.BitBoard;

            //Return to the previous game state
            game.BitBoard = current.bits;

            return node;
        }

        //#3. Roll-out. Simulate a game with a given policy and return the value
        public int Rollout(Node current, Game game, int startPlayer)
        {
            game.BitBoard = current.bits;

            //If this move is terminal and the opponent wins, this means we have previously made a move where the opponent can always find a move to win.. not good
            if (game.GetWinner() == Opponent(startPlayer))
            {
                current.parent.value = int.MinValue;
                return 0;
            }

            int player = Opponent(current.player);

            //Do the policy until a winner is found for the first (change?) node added
            while (game.GetWinner() == 0)
            {
                //Random
                List<byte> moves = game.GetValidMoves();
                byte move = moves[rand.Next(0, moves.Count)];
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
