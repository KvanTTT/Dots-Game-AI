using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public Base(int lastCaptureCount,
            int lastFreedCount,
            List<DotPosition> chainPointPoses,
            List<DotPosition> surPointPoses,
            List<short> chainPositions,
            List<short> surroundPoistions,
            int redSquare, int blueSquare)
        {
            LastCaptureCount = lastCaptureCount;
            LastFreedCount = lastFreedCount;
            ChainDotPositions = chainPointPoses;
            SurrroundDotPositions = surPointPoses;
            ChainPositions = chainPositions;
            SurroundPositions = surroundPoistions;
            Player0Square = redSquare;
            Player1Square = blueSquare;
        }

        public override string ToString()
        {
            return "R: " + LastCaptureCount + ", B: " + LastFreedCount;
        }
    }
}
