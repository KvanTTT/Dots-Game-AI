namespace DotsGame.AI.Patterns
{
    public struct PatternDot
    {
        public PatternDotState State;

        public int X;

        public int Y;

        public bool Player0;

        public PatternDot(int x, int y, PatternDotState state, bool player0)
        {
            State = state;
            X = x;
            Y = y;
            Player0 = player0;
        }

        public bool Match(DotState dotState, bool player0)
        {
            if (State == PatternDotState.Free)
            {
                return dotState.IsNotPutted();
            }

            if (State == PatternDotState.FreeOrPut)
            {
                if (dotState.IsNotPutted())
                {
                    return true;
                }
            }
            else if (State == PatternDotState.Put)
            {
                if (dotState.IsNotPutted())
                {
                    return false;
                }
            }

            int dotPlayerNumber = dotState.GetPlayerNumber();
            if (player0)
            {
                return Player0 && dotPlayerNumber == 0 || !Player0 && dotPlayerNumber == 1;
            }

            return Player0 && dotPlayerNumber == 1 || !Player0 && dotPlayerNumber == 0;
        }
    }
}
