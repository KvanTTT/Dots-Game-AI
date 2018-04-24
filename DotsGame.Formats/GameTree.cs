using System.Collections.Generic;
using System.Linq;

namespace DotsGame
{
    public class GameTree
    {
        public IList<GameMove> GameMoves { get; } = new List<GameMove>();

        public IList<GameTree> Childs { get; } = new List<GameTree>();

        public GameTree Parent { get; set; }

        public int Number { get; set; } = -1;

        public string Comment { get; set; } = "";

        public GameMove Move => GameMoves.First();

        public bool Root { get; set;}

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
        
        public GameMovesDiff GetMovesDiff(GameTree neighborNode)
        {
            List<GameTree> parents = new List<GameTree>();

            GameTree parent = this;
            do
            {
                if (parent == neighborNode)
                {
                    return new GameMovesDiff(parents.Aggregate(0, (sum, p) => { sum += p.GameMoves.Count; return sum; }),
                        GameMovesDiff.EmptyMoves);
                }
                parents.Add(parent);
                parent = parent.Parent;
            }
            while (parent != null);

            List<GameTree> neighborParents = new List<GameTree>();
            GameTree neighborParent = neighborNode;
            do
            {
                if (neighborParent == this)
                {
                    neighborParents.Reverse();
                    return new GameMovesDiff(0, neighborParents.SelectMany(tree => tree.GameMoves).ToArray());
                }
                neighborParents.Add(neighborParent);
                neighborParent = neighborParent.Parent;
            }
            while (neighborParent != null);

            int parentInd = parents.Count - 1;
            int neighborParentInd = neighborParents.Count - 1;
            while (parentInd >= 0 && neighborParentInd >= 0 && parents[parentInd] == neighborParents[neighborParentInd])
            {
                parentInd--;
                neighborParentInd--;
            }

            int unmakeMovesCount = 0;
            for (int i = 0; i <= parentInd; i++)
            {
                unmakeMovesCount += parents[i].GameMoves.Count;
            }
            List<GameMove> makeMoves;
            if (neighborParentInd >= 0)
            {
                makeMoves = new List<GameMove>();
                for (int i = neighborParentInd; i >= 0; i--)
                {
                    foreach (var move in neighborParents[i].GameMoves)
                    {
                        makeMoves.Add(move);
                    }
                }
            }
            else
            {
                makeMoves = GameMovesDiff.EmptyMoves;
            }

            var result = new GameMovesDiff(unmakeMovesCount, makeMoves);
            return result;
        }

        public override string ToString()
        {
            return string.Join(";", GameMoves);
        }

        /*public override int GetHashCode()
        {
            int hashCode = 0;
            foreach (var child in Childs)
            {
                hashCode ^= child.GetHashCode();
            }
            foreach (var move in GameMoves)
            {
                hashCode ^= move.GetHashCode();
            }
            hashCode ^= Number;
            hashCode ^= Comment.GetHashCode();
            hashCode ^= Root.GetHashCode();
            return hashCode;
        }*/
    }
}
