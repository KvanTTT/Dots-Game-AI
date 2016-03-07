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

        public GameType GameType { get; set; }

        public string AppName { get; set; }

        public Encoding Encoding { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string Player1Name { get; set; }

        public string Player2Name { get; set; }

        public DateTime Date { get; set; }

        public GameTree GameTree { get; set; }

        public string Description { get; set; }

        public string Rules { get; set; }

        public GameInfo()
        {
            GameTree = new GameTree();
            GameTree.AddMove(GameMove.Root);
        }

        public GameInfo(IList<GameMove> moves)
        {
            GameTree = new GameTree();
        }

        public GameTree GetDefaultLastTree()
        {
            return GameTree.GetDefaultLastTree();
        }
    }
}
