using System.Collections.Generic;

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
