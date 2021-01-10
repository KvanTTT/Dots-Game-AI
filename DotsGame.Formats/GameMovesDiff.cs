using System.Collections.Generic;

namespace DotsGame
{
    public struct GameMovesDiff
    {
        internal readonly static List<GameMove> EmptyMoves = new List<GameMove>();

        public int UnmakeMovesCount;

        public IList<GameMove> MakeMoves;

        public GameMovesDiff(int unmakeMovesCount, IList<GameMove> makeMoves)
        {
            UnmakeMovesCount = unmakeMovesCount;
            MakeMoves = makeMoves;
        }
    }
}
