namespace DotsGame.AI
{
    public struct Dot
    {
        public DotState DotState { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public Dot(DotState dotState, int x, int y)
        {
            DotState = dotState;
            X = x;
            Y = y;
        }
    }
}
