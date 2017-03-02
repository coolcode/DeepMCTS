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
            var game = new Game();
            var mcts = new MCTS();

            var player = 1;

            Console.WriteLine(game);

            while(!game.Over())
            {
                var bestmove = mcts.GetBestMove(game, player);
                Console.WriteLine($"best move: {bestmove}");
                game.Mark(player, bestmove);
                Console.WriteLine(game);
                //change player
                player = 3 - player;
            }

            Console.WriteLine($"Game over!");
            Console.Read();
        }
    }
}
