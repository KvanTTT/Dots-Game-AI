using System.Collections.Generic;

namespace DotsGame
{
    /// <remarks>
    /// Struct for field changes on each move
    /// if surround on Move then Base != null
    /// </remarks>
    public class State
    {
        public Base Base { get; set; }

        public DotPosition Move { get; set; }

        public int MovePlayerNumber { get; set; }

        public int DiagonalGroupCount
        {
            get;
            set;
        }

        public bool HasBase()
        {
            return Base != null;
        }

        public override string ToString()
        {
            return Move + (Base == null ? string.Empty : "; " + Base);
        }

        public State Clone()
        {
            var result = new State();
            if (Base != null)
            {
                result.Base = new Base(Base.LastCaptureCount, Base.LastFreedCount,
                    new List<DotPosition>(Base.ChainDotPositions), new List<DotPosition>(Base.SurrroundDotPositions),
                    new List<short>(Base.ChainPositions), new List<short>(Base.SurroundPositions), Base.Player0Square, Base.Player1Square);
            }
            result.Move = Move;
            result.DiagonalGroupCount = DiagonalGroupCount;
            return result;
        }
    }
}
