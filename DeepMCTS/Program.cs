using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepMCTS
{
    class Program
    {
        static void Main(string[] args)
        {
            TestFight();
           // TestMCTS();
            // TestBoardConvert();
            Console.WriteLine($"done!");
            Console.Read();
        }


        private static void TestFight()
        {
            var ai1 = new MCTS();
            var ai2 = new MCTS();

            var i = 0;
            var lose = 0;
            var win = 0;
            var count = 100000;
            while (i++ < count)
            {
                Console.WriteLine($"{i}. now: {DateTime.Now}");

                var game = new Game();
                var player = 1;

                //Console.WriteLine(game);

                do
                {
                    var bestmove = ai2.GetBestMove_RealtimeTraining(game, player);
                    //Console.WriteLine($"p1 best move: {bestmove}");
                    Console.Write(bestmove);
                    game.Mark(player, bestmove);

                    //change player
                    player = 3 - player;
                    if (!game.Over())
                    {
                        //bestmove = ai2.RandomMove(game, player);
                        bestmove = ai1.GetBestMove(game, player);
                        //Console.WriteLine($"p2 best move: {bestmove}");
                        Console.Write(bestmove);
                        game.Mark(player, bestmove);
                    }

                    //change player
                    player = 3 - player;
                }
                while (!game.Over());

                var winner = game.GetWinner();
                if (winner == 1)
                { 
                    lose++;
                }else if (winner == 2)
                {
                    win++;
                }
                
                Console.WriteLine($" winner:{winner} score: {lose}-{win}");

                //Console.WriteLine(game);
            }
        }

        private static void TestBoardConvert()
        {
            var board = new byte[9] { 2, 1, 2, 1, 0, 1, 1, 2, 1 };
            var bits = Helper.BoardToBits(board);
            var board2 = Helper.BitsToBoard(bits);
            for (var i = 0; i < board.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {board[i]} - {board2[i]} eq:{board[i] == board2[i]}");
            }
        }

        private static void TestMCTS2()
        {
            var mcts = new MCTS();

            var i = 0;
            while (i < 1000)
            {
                Console.WriteLine("now: {0}", DateTime.Now);
                mcts.Train(i++);
            }
        }

        private static void TestMCTS()
        {
            var game = new Game();
            var mcts = new MCTS();

            var player = 1;

            Console.WriteLine(game);

            do
            {
                var bestmove = mcts.GetBestMove(game, player);
                Console.WriteLine($"best move: {bestmove}");
                game.Mark(player, bestmove);
                Console.WriteLine(game);
                //change player
                player = 3 - player;
            }
            while (!game.Over());
        }
    }
}
