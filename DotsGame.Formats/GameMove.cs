using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotsGame
{
    public class GameMove : IEquatable<GameMove>
    {
        public int PlayerNumber { get; set; }

        public int Row { get; set; }

        public int Column { get; set; }

        public static readonly GameMove Root = new GameMove(-1, 0, 0);

        public GameMove(int playerNumber, int row, int column)
        {
            PlayerNumber = playerNumber;
            Row = row;
            Column = column;
        }

        public bool IsRoot => PlayerNumber == -1;

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
            else
            {
                return $"{Row}:{Column},{(PlayerNumber == 0 ? "Blue" : "Red")}";
            }
        }
    }
}
