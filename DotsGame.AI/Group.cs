using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotsGame.AI
{
    public class Group
    {
        public int Number { get; set; }

        public List<Dot> Dots { get; set; }

        public Group(int number, List<Dot> dots)
        {
            Number = number;
            Dots = dots;
        }
    }
}
