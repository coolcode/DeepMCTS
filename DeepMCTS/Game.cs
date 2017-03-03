using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepMCTS
{
    public class Game
    {
        private int bitBoard = 0x0;

        public int BitBoard
        {
            get
            {
                //bitBoard = Helper.BoardToBits(board);
                return bitBoard;
            }
            set
            {
                bitBoard = value;
                board = Helper.BitsToBoard(value);
            }
        }

        public int Depth
        {
            get
            {
                return board.Count(c => c == 0);
            }
        }

        private byte[] board = new byte[9];

        public void Restart()
        {
            BitBoard = 0x0;
        }

        public void Mark(int player, byte index)
        {
            if (board[index] != 0)
                throw new Exception("Tryed to mark an non empty slot");

            if (player == 1)
                board[index] = 1;
            else
                board[index] = 2;
            bitBoard = Helper.BoardToBits(board);
        }

        public List<byte> GetValidMoves()
        {
            return GetValidMoves(bitBoard);
        }

        public List<byte> GetValidMoves(int b)
        {
            var state = Helper.BitsToBoard(b);
            List<byte> moves = new List<byte>();

            for (int i = 0; i < 9; i++)
                if (state[i] == 0)
                    moves.Add((byte)i);

            return moves;
        }

        public int GetWinner()
        {
            return GetWinner(bitBoard);
        }

        public int GetWinner(int b)
        {
            var board = Helper.BitsToBoard(b);
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

            if (GetValidMoves(b).Count == 0)
                return 3;

            return 0;
        }

        public bool Over()
        {
            return Over(bitBoard);
        }

        public bool Over(int b)
        {
            return GetWinner(b) != 0;
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

            sb.AppendLine($"winner: {GetWinner()}");

            return sb.ToString();
        }

        public override string ToString()
        {
            return Draw();
        }


    }
}
