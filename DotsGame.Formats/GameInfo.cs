using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotsGame
{
    public class GameInfo
    {
        public bool FromUrl { get; set; }

        public GameType GameType { get; set; } = GameType.Kropki;

        public string AppName { get; set; } = "Dots Game AI";

        public int Width { get; set; } = 39;

        public int Height { get; set; } = 32;

        public string Player1Name { get; set; } = "";

        public string Player2Name { get; set; } = "";

        public DateTime Date { get; set; } = DateTime.Now;

        public GameTree GameTree { get; set; }

        public string Description { get; set; } = "";

        public string Rules { get; set; } = "";

        public GameInfo()
        {
            GameTree = new GameTree() { Number = 0 };
            GameTree.AddMove(GameMove.Root);
        }

        public GameTree GetDefaultLastTree()
        {
            return GameTree.GetDefaultLastTree();
        }
    }
}
