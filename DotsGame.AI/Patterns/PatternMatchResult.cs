using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotsGame.AI.Patterns
{
    public struct PatternMatchResult
    {
        public readonly int X;

        public readonly int Y;

        public readonly Pattern Pattern;

        public PatternMatchResult(int x, int y, Pattern pattern)
        {
            X = x;
            Y = y;
            Pattern = pattern;
        }
    }
}
