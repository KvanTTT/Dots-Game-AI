using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotsGame;

namespace DotsGame.AI
{
    public class StrategicMovesAnalyzer
    {
        #region Fields

        private DotState[] _dots;

        private List<LinkedGroup> _groups;
        private List<LinkedGroup> _ownGroups;
        private List<LinkedGroup> _enemyGroups;
        private int[] DotsGroups;

        #endregion

        #region Constructors

        public StrategicMovesAnalyzer(Field field)
        {
            Field = field;
            _dots = Field.CloneDots();
            DotsGroups = new int[Field.RealDotsCount];
        }

        #endregion

        #region Public Methods

        #endregion

        #region Helpers

        public void GenerateGroups()
        {
            _groups = new List<LinkedGroup>();
            _ownGroups = new List<LinkedGroup>();
            _enemyGroups = new List<LinkedGroup>();
            var curPlayer = Field.CurrentPlayer.NextPlayer();
            var enemyPlayer = Field.CurrentPlayer;
            LinkedGroup group;

            for (int i = 1; i <= Field.Width; i++)
                for (int j = 1; j <= Field.Height; j++)
                {
                    int ind = Field.GetPosition(i, j);

                    if (!_dots[ind].IsTagged())
                    {
                        if (_dots[ind].IsPlayerPutted(curPlayer))
                        {
                            group = new LinkedGroup(curPlayer, _groups.Count + 1, FillDiagLinkedDots(ind, _groups.Count + 1));
                            _ownGroups.Add(group);
                            _groups.Add(group);
                        }
                        else if (_dots[ind].IsPlayerPutted(enemyPlayer))
                        {
                            group = new LinkedGroup(enemyPlayer, _groups.Count + 1, FillDiagLinkedDots(ind, _groups.Count + 1));
                            _enemyGroups.Add(group);
                            _groups.Add(group);
                        }
                    }
                }

            ClearAllTags();
        }

        private List<int> FillDiagLinkedDots(int pos, int currentGroupNumber)
        {
            var result = new List<int>();

            DotState player = _dots[pos].GetPlayer();
            var tempStack = new Stack<int>();
            _dots[pos] |= DotState.Tagged;
            tempStack.Push(pos);
            result.Add(pos);

            List<DotState> dots = new List<DotState>();
            while (tempStack.Count != 0)
            {
                pos = tempStack.Pop();

                for (int i = 0; i < Field.DiagDeltas.Length; i++)
                {
                    int newPos = pos + Field.DiagDeltas[i];
                    if (_dots[newPos].IsPlayerPutted(player) && !_dots[newPos].IsTagged())
                    {
                        _dots[newPos] |= DotState.Tagged;
                        tempStack.Push(newPos);
                        result.Add(newPos);

                        DotsGroups[newPos] = currentGroupNumber;
                    }
                }
            }

            return result;
        }

        private void ClearAllTags()
        {
            for (int i = 1; i <= Field.Width; i++)
                for (int j = 1; j <= Field.Height; j++)
                    _dots[j * Field.RealWidth + i].ClearTag();
        }

        #endregion

        #region Properties

        public IEnumerable<LinkedGroup> OwnGroups
        {
            get
            {
                return _ownGroups;
            }
        }

        public IEnumerable<LinkedGroup> EnemyGroups
        {
            get
            {
                return _enemyGroups;
            }
        }

        public IEnumerable<LinkedGroup> Groups
        {
            get
            {
                return _groups;
            }
        }

        public Field Field
        {
            get;
            set;
        }

        #endregion
    }
}