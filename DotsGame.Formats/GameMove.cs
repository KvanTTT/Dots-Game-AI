using System;

namespace DotsGame
{
    public class GameMove : IEquatable<GameMove>
    {
        public int PlayerNumber { get; set; }

        public int Row { get; set; }

        public int Column { get; set; }

        public GameMove(int playerNumber, int row, int column)
        {
            PlayerNumber = playerNumber;
            Row = row;
            Column = column;
        }

        public bool Equals(GameMove other)
        {
            return PlayerNumber == other.PlayerNumber && Row == other.Row && Column == other.Column;
        }

        public override string ToString()
        {
            if (PlayerNumber == -1)
            {
                return "Root";
            }

            return $"{Row}:{Column},{(PlayerNumber == 0 ? "Player0" : "Player1")}";
        }

        public override int GetHashCode()
        {
            return PlayerNumber ^ Row ^ Column;
        }
    }
}
