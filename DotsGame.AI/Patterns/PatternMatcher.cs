using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotsGame.AI.Patterns
{
    public class PatternMatcher
    {
        private DotState[][] _dots;
        private IList<Pattern> _patterns;

        public PatternMatcher(DotState[][] dotStates, IList<Pattern> patterns)
        {
            _dots = dotStates;
            _patterns = patterns;
        }

        public List<PatternMatchResult> GetMatches()
        {
            var result = new List<PatternMatchResult>();
            for (int x = 0; x < _dots.Length; x++)
            {
                for (int y = 0; y < _dots[x].Length; y++)
                {
                    foreach (var pattern in _patterns)
                    {
                        if (pattern.Match(_dots, x, y))
                        {
                            result.Add(new PatternMatchResult(x, y, pattern));
                        }
                    }
                }
            }
            return result;
        }
    }
}
