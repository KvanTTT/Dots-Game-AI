using System;
using System.Collections.Generic;
using System.Linq;

namespace DotsGame.AI
{
    public class LinkedGroup
    {
        #region Readonly & Fields

        public const int ZeroGroup = 0;
        public readonly DotState Player;
        public readonly List<int> Positions;
        public readonly int Number;
        private List<int> EnvelopePositions_;

        #endregion

        #region Constructors

        public LinkedGroup(DotState player, int number, List<int> positions)
        {
            Player = player;
            Positions = positions;
            Number = number;

            BuildEnvelope();
        }

        #endregion

        #region Public Methods

        public void BuildEnvelope()
        {
            EnvelopePositions_ = new List<int>(Positions);

            if (Positions.Count == 1)
                return;

            int minPos = Positions.Min();
            Field.GetPosition(minPos, out int minPosX, out int minPosY);

            EnvelopePositions_.Remove(minPos);

            EnvelopePositions_.Sort((pos2, pos1) =>
            {
                Field.GetPosition(pos1, out int x1, out int y1);
                Field.GetPosition(pos2, out int x2, out int y2);
                x1 -= minPosX;
                y1 -= minPosY;
                x2 -= minPosX;
                y2 -= minPosY;
                return ((float)x1 / (Math.Abs(x1) + Math.Abs(y1))).CompareTo((float)x2 / (Math.Abs(x2) + Math.Abs(y2)));
            });

            EnvelopePositions_.Insert(0, minPos);

            int m = 0;
            for (int i = 1; i < EnvelopePositions_.Count; i++)
            {
                if (i != m)
                {
                    if (m > 1)
                    {
                        while (m >= 1 && IsCCW(EnvelopePositions_[m - 1], EnvelopePositions_[m], EnvelopePositions_[i]) <= 0)
                            m--;
                    }
                }
                m++;
                int t = EnvelopePositions_[m];
                EnvelopePositions_[m] = EnvelopePositions_[i];
                EnvelopePositions_[i] = t;
            }

            if (m + 1 != EnvelopePositions_.Count)
                EnvelopePositions_.RemoveRange(m + 1, EnvelopePositions_.Count - m - 1);
        }

        #endregion

        #region Helpers

        private float IsCCW(int p1, int p2, int p3)
        {
            Field.GetPosition(p1, out int x1, out int y1);
            Field.GetPosition(p2, out int x2, out int y2);
            Field.GetPosition(p3, out int x3, out int y3);
            return (x2 - x1) * (y3 - y1) - (y2 - y1) * (x3 - x1);
        }

        #endregion

        #region Properties

        public IEnumerable<int> EnvelopePositions
        {
            get
            {
                return EnvelopePositions_;
            }
        }

        #endregion
    }
}
