namespace DotsGame
{
    public static class Helper
    {
        public static int[] NextPosOffsets;
        public static int[] NextFirstPosOffsets;
        public static int[] NextFirstPosOffsetsCCW;

        public static DotState GetPlayer(this DotState dot)
        {
            return dot & DotState.Player;
        }

        public static int GetPlayerNumber(this DotState dot)
        {
            return (int)(dot & DotState.Player);
        }

        public static bool IsPutted(this DotState dot)
        {
            return (dot & DotState.Putted) == DotState.Putted;
        }

        public static bool IsNotPutted(this DotState dot)
        {
            return (dot & DotState.EnableMask) == DotState.Empty;
        }

        public static DotState NextPlayer(this DotState player)
        {
            return (player & DotState.Player) == DotState.Player0 ? DotState.Player1 : DotState.Player0;
        }

        public static bool IsPlayerPutted(this DotState dot, DotState player)
        {
            return (dot & DotState.EnableMask) == (DotState.Putted | player);
        }

        public static bool IsPlayerPutted(this DotState dot, int playerNumber)
        {
            return (dot & DotState.EnableMask) == (DotState.Putted | (DotState)playerNumber);
        }

        public static bool IsPlayer0Putted(this DotState dot)
        {
            return (dot & DotState.EnableMask) == (DotState.Putted | DotState.Player0);
        }

        public static bool IsPlayer1Putted(this DotState dot)
        {
            return (dot & DotState.EnableMask) == (DotState.Putted | DotState.Player1);
        }

        public static bool IsZeroSurroundLevel(this DotState dot)
        {
            return (dot & DotState.SurroundCountMask) == (DotState)0;
        }

        public static bool IsOneSurroundLevel(this DotState dot)
        {
            return (dot & DotState.SurroundCountMask) == DotState.FirstSurroundLevel;
        }

        public static bool IsMoreThanOneSurroundLevel(this DotState dot)
        {
            return (dot & DotState.SurroundCountMask) > DotState.FirstSurroundLevel;
        }

        public static bool IsSurrounded(this DotState dot)
        {
            return (dot & DotState.SurroundCountMask) >= DotState.FirstSurroundLevel;
        }

        public static bool IsRealPutted(this DotState dot)
        {
            return (dot & DotState.RealPutted) == DotState.RealPutted;
        }

        public static bool IsRealPlayer0(this DotState dot)
        {
            return (dot & DotState.RealPlayer) == DotState.RealPlayer0;
        }

        public static bool IsRealPlayer1(this DotState dot)
        {
            return (dot & DotState.RealPlayer) == DotState.RealPlayer1;
        }

        public static bool IsEnable(this DotState dot, DotState enableCondition)
        {
            return (dot & DotState.EnableMask) == enableCondition;
        }

        public static bool IsBound(this DotState dot, DotState boundCond)
        {
            return (dot & DotState.BoundMask) == boundCond;
        }

        public static bool IsBound(this DotState dot)
        {
            return (dot & DotState.Bound) == DotState.Bound;
        }

        public static bool IsTagged(this DotState dot)
        {
            return (dot & DotState.Tagged) == DotState.Tagged;
        }

        public static bool IsInEmptyBase(this DotState dot)
        {
            return (dot & DotState.EmptyBase) == DotState.EmptyBase;
        }

        public static void ClearTag(this DotState dot)
        {
            dot &= ~DotState.Tagged;
        }

        public static DotState GetEnabledCondition(this DotState dot)
        {
            return dot & DotState.EnableMask;
        }

        public static DotState GetDiagGroupNumber(this DotState dot)
        {
            return dot & DotState.DiagonalGroupMask;
        }

        public static DotState SetDiagonalGroupNumber(this DotState dot, int groupNumber)
        {
            return (DotState)((dot & ~DotState.DiagonalGroupMask) | (DotState)groupNumber);
        }

        public static DotState ClearGroupNumber(this DotState dot)
        {
            return (DotState)(dot & ~DotState.DiagonalGroupMask);
        }

        public static bool IsEmptyBound(this DotState dot)
        {
            return (dot & DotState.EmptyBound) == DotState.EmptyBound;
        }

        static Helper()
        {
            NextPosOffsets = new int[Field.RealWidth * 2 + 3];
            NextPosOffsets[0] = -Field.RealWidth;
            NextPosOffsets[1] = -Field.RealWidth + 1;
            NextPosOffsets[2] = +1;
            NextPosOffsets[2 + Field.RealWidth] = +Field.RealWidth + 1;
            NextPosOffsets[2 + Field.RealWidth * 2] = +Field.RealWidth;
            NextPosOffsets[1 + Field.RealWidth * 2] = +Field.RealWidth - 1;
            NextPosOffsets[0 + Field.RealWidth * 2] = -1;
            NextPosOffsets[0 + Field.RealWidth] = -Field.RealWidth - 1;

            NextFirstPosOffsets = new int[Field.RealWidth * 2 + 3];
            NextFirstPosOffsets[0] = -Field.RealWidth + 1;
            NextFirstPosOffsets[1] = +Field.RealWidth + 1;
            NextFirstPosOffsets[2] = +Field.RealWidth + 1;
            NextFirstPosOffsets[2 + Field.RealWidth] = +Field.RealWidth - 1;
            NextFirstPosOffsets[2 + Field.RealWidth * 2] = +Field.RealWidth - 1;
            NextFirstPosOffsets[1 + Field.RealWidth * 2] = -Field.RealWidth - 1;
            NextFirstPosOffsets[0 + Field.RealWidth * 2] = -Field.RealWidth - 1;
            NextFirstPosOffsets[0 + Field.RealWidth] = -Field.RealWidth + 1;

            NextFirstPosOffsetsCCW = new int[Field.RealWidth * 2 + 3];
            NextFirstPosOffsetsCCW[0] = +Field.RealWidth - 1;
            NextFirstPosOffsetsCCW[1] = +Field.RealWidth - 1;
            NextFirstPosOffsetsCCW[2] = -Field.RealWidth - 1;
            NextFirstPosOffsetsCCW[2 + Field.RealWidth] = -Field.RealWidth - 1;
            NextFirstPosOffsetsCCW[2 + Field.RealWidth * 2] = -Field.RealWidth + 1;
            NextFirstPosOffsetsCCW[1 + Field.RealWidth * 2] = -Field.RealWidth + 1;
            NextFirstPosOffsetsCCW[0 + Field.RealWidth * 2] = +Field.RealWidth + 1;
            NextFirstPosOffsetsCCW[0 + Field.RealWidth] = +Field.RealWidth + 1;
        }
    }
}
