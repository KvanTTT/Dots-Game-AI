using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotsGame.AI
{
    public struct Move
    {
        public int X;
        public int Y;

        public Move(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
