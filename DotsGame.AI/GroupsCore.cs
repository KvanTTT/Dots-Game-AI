using System;
using System.Collections.Generic;
using DotsGame.AI.Patterns;

namespace DotsGame.AI
{
    public class GroupsCore : Core
    {
        public GroupsCore(Field gameField)
            : base(gameField)
        {
        }

        public List<Group> GetStrongGroups()
        {
            var result = new Dictionary<int, Group>();

            int[,] tagged = new int[GameField.Width, GameField.Height];

            return null;
        }

        public List<Group> GetWeakGroups(int order = 0)
        {
            throw new NotImplementedException();
        }

        public List<PatternMatchResult> GetCrosswises()
        {
            var crosswise = new Pattern(1, new[]
            {
                new PatternDot(0, 0, PatternDotState.Put, true),
                new PatternDot(0, 1, PatternDotState.Put, false),
                new PatternDot(1, 1, PatternDotState.Put, true),
                new PatternDot(1, 0, PatternDotState.Put, false)
            });
            var patterns = new[] { crosswise };
            var patternMatcher = new PatternMatcher(_dots, patterns);
            var result = patternMatcher.GetMatches();
            return result;
        }
    }
}
