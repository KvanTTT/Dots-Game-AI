﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
