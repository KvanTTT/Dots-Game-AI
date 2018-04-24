using System.Collections.Generic;

namespace DotsGame
{
    public class Base
    {
        public readonly int LastCaptureCount;
        public readonly int LastFreedCount;
        public readonly int Player0Square;
        public readonly int Player1Square;
        public readonly List<short> ChainPositions;
        public readonly List<short> SurroundPositions;
        public readonly List<DotPosition> ChainDotPositions;
        public readonly List<DotPosition> SurrroundDotPositions;

        public Base(int lastCaptureCount, int lastFreedCount,
            List<DotPosition> chainPointPoses, List<DotPosition> surroundPointPoses,
            List<short> chainPositions, List<short> surroundPoistions,
            int player0Square, int player1Square)
        {
            LastCaptureCount = lastCaptureCount;
            LastFreedCount = lastFreedCount;
            ChainDotPositions = chainPointPoses;
            SurrroundDotPositions = surroundPointPoses;
            ChainPositions = chainPositions;
            SurroundPositions = surroundPoistions;
            Player0Square = player0Square;
            Player1Square = player1Square;
        }

        public override string ToString()
        {
            return "Player0: " + LastCaptureCount + ", Player1: " + LastFreedCount;
        }
    }
}
