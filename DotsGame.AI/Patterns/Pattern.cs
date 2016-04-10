using DotsGame.AI.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotsGame
{
    public class Pattern
    {
        public readonly int Id;
        public readonly int Width;
        public readonly int Height;
        public readonly IList<PatternDot> PatternDots;

        public Pattern(int id, IList<PatternDot> patternDots)
        {
            Id = id;
            PatternDots = patternDots;
            Width = 0;
            Height = 0;
            foreach (var state in patternDots)
            {
                if (Width < state.X + 1)
                {
                    Width = state.X + 1;
                }

                if (Height < state.Y + 1)
                {
                    Height = state.Y + 1;
                }
            }
        }

        public bool Match(DotState[][] dots, int x, int y)
        {
            bool result = true;
            // Match all colors.
            result = Match(dots, x, y, true);
            if (!result)
            {
                result = Match(dots, x, y, false);
            }
            return result;
        }

        private bool Match(DotState[][] dots, int x, int y, bool player0)
        {
            bool result = true;
            foreach (var patternDot in PatternDots)
            {
                int xOffset = x + patternDot.X;
                int yOffset = y + patternDot.Y;
                if (xOffset < 0 || xOffset >= dots.Length || yOffset < 0 || yOffset >= dots[0].Length ||
                    !patternDot.Match(dots[x + patternDot.X][y + patternDot.Y], player0))
                {
                    result = false;
                    break;
                }
            }

            return result;
        }
    }
}
