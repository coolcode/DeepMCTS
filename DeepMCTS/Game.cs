using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepMCTS
{
    public class Game
    {
        public byte[] board = new byte[9]; //1,2,3-4,5,6-7,8,9

        public void Restart()
        {
            board = new byte[9];
        }

        public void Mark(int player, byte index)
        {
            if (board[index] != 0)
                throw new Exception("Tryed to mark an non empty slot");

            if (player == 1)
                board[index] = 1;
            else
                board[index] = 2;
        }

        public List<byte> GetValidMoves()
        {
            return GetValidMoves(board);
        }

        public List<byte> GetValidMoves(byte[] state)
        {
            List<byte> moves = new List<byte>();

            for (int i = 0; i < 9; i++)
                if (state[i] == 0)
                    moves.Add((byte)i);

            return moves;
        }

        public int GetWinner()
        {
            return GetWinner(board);
        }

        public int GetWinner(byte[] board)
        {
            for (byte p = 1; p <= 2; p++)
            {
                if (board[0] == p && board[1] == p && board[2] == p)
                    return p;

                if (board[3] == p && board[4] == p && board[5] == p)
                    return p;

                if (board[6] == p && board[7] == p && board[8] == p)
                    return p;

                if (board[0] == p && board[3] == p && board[6] == p)
                    return p;

                if (board[1] == p && board[4] == p && board[7] == p)
                    return p;

                if (board[2] == p && board[5] == p && board[8] == p)
                    return p;

                if (board[0] == p && board[4] == p && board[8] == p)
                    return p;

                if (board[2] == p && board[4] == p && board[6] == p)
                    return p;
            }

            if (GetValidMoves(board).Count == 0)
                return 3;

            return 0;
        }

        public bool Over()
        {
            return Over(board);
        }

        public bool Over(byte[] board)
        {
            return GetWinner(board) != 0;
        }

        public string Draw()
        {
            var sb = new StringBuilder();
            sb.AppendLine("--------------------");

            for (int i = 0; i < 3; i++)
                sb.Append(board[i].ToString() + "-");

            sb.Append("\r\n");

            for (int i = 3; i < 6; i++)
                sb.Append(board[i].ToString() + "-");

            sb.Append("\r\n");

            for (int i = 6; i < 9; i++)
                sb.Append(board[i].ToString() + "-");

            sb.AppendLine("\r\n--------------------");

            return sb.ToString();
        }

        public override string ToString()
        {
            return Draw();
        }


    }
}
