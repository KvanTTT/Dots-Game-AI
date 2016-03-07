using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotsGame
{
    public class GameTree
    {
        public IList<GameMove> GameMoves { get; } = new List<GameMove>();

        public IList<GameTree> Childs { get; } = new List<GameTree>();

        public GameTree Parent { get; set; }

        public string Comment { get; set; }

        public GameMove Move => GameMoves.First();

        public GameTree Child => Childs.First();

        public GameTree()
        {
        }

        public GameTree(GameTree parent, GameMove move)
        {
            Parent = parent;
            GameMoves.Add(move);
        }

        public void AddMove(GameMove move)
        {
            GameMoves.Add(move);
        }

        public int GetMaxSequenceLength()
        {
            int maxSequenceLength = 0;
            foreach (var child in Childs)
            {
                int maxLength = child.GetMaxSequenceLength();
                if (maxLength > maxSequenceLength)
                {
                    maxSequenceLength = maxLength;
                }
            }
            return maxSequenceLength + 1;
        }

        public int GetMaxSequenceWidth()
        {
            int maxSequenceWidth = 0;
            if (Childs.Count == 0)
            {
                maxSequenceWidth = 1;
            }
            else
            {
                foreach (var child in Childs)
                {
                    maxSequenceWidth += child.GetMaxSequenceWidth();
                }
            }

            return maxSequenceWidth;
        }

        public GameTree GetDefaultLastTree()
        {
            var tree = this;
            while (tree.Childs.Count > 0)
            {
                tree = tree.Childs.First();
            }
            return tree;
        }

        public IList<GameTree> GetDefaultSequence()
        {
            var result = new List<GameTree>();
            var tree = this;
            result.Add(tree);
            while (tree.Childs.Count > 0)
            {
                tree = tree.Childs.First();
                result.Add(tree);
            }
            return result;
        }

        /// <summary>
        /// TODO: optimize it.
        /// </summary>
        /// <param name="neighborNode"></param>
        /// <returns></returns>
        public GameMovesDiff GetMovesDiff(GameTree neighborNode)
        {
            List<GameTree> parents = new List<GameTree>();
            List<GameTree> neighborParents = new List<GameTree>();

            GameTree parent = this;
            do
            {
                parents.Add(parent);
                parent = parent.Parent;
            }
            while (parent != null);

            GameTree neighborParent = neighborNode;
            do
            {
                neighborParents.Add(neighborParent);
                neighborParent = neighborParent.Parent;
            }
            while (neighborParent != null);

            int parentInd = parents.Count - 1;
            int neighborParentInd = neighborParents.Count - 1;
            while (parentInd >= 0 && neighborParentInd >= 0 &&
                   parents[parentInd] == neighborParents[neighborParentInd])
            {
                parentInd--;
                neighborParentInd--;
            }
            parentInd++;

            int unmakeMovesCount = parentInd;
            var makeMoves = new List<GameMove>();
            for (int i = neighborParentInd; i >= 0; i--)
            {
                var move = neighborParents[i].GameMoves.First();
                if (!move.IsRoot)
                {
                    makeMoves.Add(move);
                }
            }

            var result = new GameMovesDiff(unmakeMovesCount, makeMoves);
            return result;
        }

        public override string ToString()
        {
            return string.Join(";", GameMoves);
        }
    }
}
