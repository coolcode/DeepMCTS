using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepMCTS
{
    class Helper
    {
        public static void CopyBytes(byte[] destination, byte[] source)
        {
            source.CopyTo(destination, 0);
        }

        public static int BoardToBits(byte[] board)
        {
            var bits = 0x0;
            for (var i = 0; i < board.Length; i++)
            {
                bits |= board[i] << ((board.Length - i - 1) * 2);
            }

            return bits;
        }

        public static byte[] BitsToBoard(int bits)
        {
            var board = new byte[9];
            for (var i = 0; i < board.Length; i++)
            {
                var s = bits & 3;
                board[board.Length - i - 1] = (byte)s;
                bits = bits >> 2;
            }

            return board;
        }

        public static string BitsToText(int bits)
        {
            var ss = new[] { '_', 'O', 'X' };
            var sb = new StringBuilder();
            var board = BitsToBoard(bits);
            for (var i = 0; i < board.Length; i++)
            {
                sb.Append(ss[board[i]]);
            }

            return sb.ToString();
        }
    }
}
