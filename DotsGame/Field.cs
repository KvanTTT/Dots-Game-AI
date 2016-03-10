using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DotsGame
{
    public class Field
    {
        #region Constatnts and masks

        public const int RealWidth = 64;
        public const int InputSurroundDotsCount = 4;

        public static int[] DiagDeltas = {
            -Field.RealWidth - 1, -Field.RealWidth, -Field.RealWidth + 1,
            -1, +1,
            +Field.RealWidth - 1, +Field.RealWidth, +Field.RealWidth + 1 };

        public static int[] VertHorizDeltas = { -Field.RealWidth, +1, +Field.RealWidth, -1 };

        #endregion

        #region Fields & readonly

        protected List<int> _tempList;
        protected List<short> _chainPositions;
        protected List<short> _surroundPositions;
        private bool _emptyBaseCreated;
        private int _lastBaseCaptureCount;
        private int _lastBaseFreedCount;
        protected int _lastSquareCaptureCount;
        private int _lastSquareFreedCount;

        /// <summary>
        /// Indexes and states of last move chain dots.
        /// </summary>
        protected List<DotPosition> _chainDotsPositions;

        /// <summary>
        /// Indexes and states of last move surround dots.
        /// </summary>
        protected List<DotPosition> _surroundDotsPositions;

        /// <summary>
        /// Array of dots or dot states
        /// </summary>
        protected DotState[] _dots;

        /// <summary>
        /// Sequance of putted dots.
        /// Using for moves rollback.
        /// </summary>
        private List<State> _dotsSequenceStates;

        protected List<int> _inputChainDots;

        protected List<int> _inputSurroundedDots;

        public int Width
        {
            get;
            private set;
        }

        public int Height
        {
            get;
            private set;
        }

        public enmSurroundCondition SurroundCondition
        {
            get;
            private set;
        }

        #endregion

        #region Main Properties

        public DotState this[int i]
        {
            get
            {
                return _dots[i];
            }
        }

        public IList<State> States
        {
            get
            {
                return _dotsSequenceStates;
            }
        }

        public IEnumerable<int> DotsSequancePositions
        {
            get
            {
                return States.Select(state => (int)state.Move.Position);
            }
        }

        public int Player0CaptureCount
        {
            get;
            private set;
        }

        public int Player1CaptureCount
        {
            get;
            private set;
        }

        public int OldPlayer0CaptureCount
        {
            get;
            private set;
        }

        public int OldPlayer1CaptureCount
        {
            get;
            private set;
        }

        public enmMoveState LastMoveState
        {
            get;
            private set;
        }

        public int RealDotsCount
        {
            get
            {
                return _dots.Length;
            }
        }

        public int DotsSequenceCount
        {
            get
            {
                return _dotsSequenceStates.Count;
            }
        }

        public int Player0Square
        {
            get;
            protected set;
        }

        public int Player1Square
        {
            get;
            protected set;
        }

        public int DiagonalLinkedGroupsCount
        {
            get;
            protected set;
        }

        #endregion

        #region Last Move

        public DotState CurrentPlayer
        {
            get;
            set;
        }

        public int LastPosition
        {
            get;
            private set;
        }

        public int LastMoveCaptureCount
        {
            get;
            protected set;
        }

        public int LastMoveFreedCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Indexes of last move chain dots.
        /// </summary>
        public IEnumerable<short> ChainPositions
        {
            get
            {
                return _chainPositions;
            }
        }

        /// <summary>
        /// Indexes of last move surorund dots.
        /// </summary>
        public IEnumerable<short> SurroundPositions
        {
            get
            {
                return _surroundPositions;
            }
        }

        public State LastState
        {
            get;
            private set;
        }

        #endregion

        #region Constructors

        protected Field()
        {
        }

        public Field(int width, int height, enmSurroundCondition surroundCondition = enmSurroundCondition.Standart)
        {
            Width = width;
            Height = height;
            SurroundCondition = surroundCondition;

            _dots = new DotState[RealWidth * (height + 2)];
            FillBadValues();

            _dotsSequenceStates = new List<State>(64);

            _chainPositions = new List<short>(16);
            _surroundPositions = new List<short>(InputSurroundDotsCount);
            _chainDotsPositions = new List<DotPosition>(16);
            _surroundDotsPositions = new List<DotPosition>(InputSurroundDotsCount);
            _tempList = new List<int>(16);
            _inputChainDots = new List<int>(InputSurroundDotsCount);
            _inputSurroundedDots = new List<int>(InputSurroundDotsCount);
            LastMoveState = enmMoveState.None;
        }

        #endregion

        #region Static Helpers

        /// <summary>
        /// Returns square bounded by the triangle with vertexes (0, pos1, pos2)
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <returns></returns>
        public static int GetSquare(int pos1, int pos2)
        {
            return (pos1 / RealWidth) * (pos2 % RealWidth) - (pos1 % RealWidth) * (pos2 / RealWidth);
        }

        /// <summary>
        ///  * . .   x . *   . x x   . . .
        ///  . o .   x o .   . o .   . o x
        ///  x x .   . . .   . . *   * . x
        ///  o - center pos
        ///  x - pos
        ///  * - result (new pos)
        /// </summary>
        /// <param name="centerPosition"></param>
        /// <param name="pos"></param>
        public static void GetFirstNextPos(int centerPosition, ref int pos)
        {
            pos = centerPosition + Helper.NextFirstPosOffsets[pos - centerPosition + Field.RealWidth + 1];
        }

        /// <summary>
        ///  . . .   . . *   * . x   x x .
        ///  x o .   . o .   . o x   . o .
        ///  x . *   . x x   . . .   * . .
        ///  o - center pos
        ///  x - pos
        ///  * - result (new pos)
        /// </summary>
        /// <param name="centerPosition"></param>
        /// <param name="pos"></param>
        public static void GetFirstNextPosCCW(int centerPosition, ref int pos)
        {
            pos = centerPosition + Helper.NextFirstPosOffsetsCCW[pos - centerPosition + Field.RealWidth + 1];
        }

        /// <summary>
        ///  . . .   * . .   x * .   . x *   . . x   . . .   . . .   . . .
        ///  * o .   x o .   . o .   . o .   . o *   . o x   . o .   . o .
        ///  x . .   . . .   . . .   . . .   . . .   . . *   . * x   * x .
        ///  o - center pos
        ///  x - pos
        ///  * - result (new pos)
        /// </summary>
        /// <param name="centerPosition"></param>
        /// <param name="pos"></param>
        public static void GetNextPos(int centerPosition, ref int pos)
        {
            pos = centerPosition + Helper.NextPosOffsets[pos - centerPosition + Field.RealWidth + 1];
        }

        public static int Distance(int pos1, int pos2)
        {
            return
                Math.Max(Math.Abs((pos1 % Field.RealWidth) - (pos2 % Field.RealWidth)),
                         Math.Abs((pos1 / Field.RealWidth) - (pos2 / Field.RealWidth)));
        }

        #endregion

        #region Helpers

        protected void RemoveEmptyBaseFlags(int startPosition)
        {
            _surroundDotsPositions.Add(new DotPosition(startPosition, _dots[startPosition]));
            _dots[startPosition] &= ~DotState.EmptyBase;

            var pos = startPosition;
            _tempList.Clear();
            _tempList.Add(startPosition);

            while (_tempList.Count != 0)
            {
                pos = _tempList.Last();
                _tempList.RemoveAt(_tempList.Count - 1);

                if ((_dots[pos - 1] & DotState.EmptyBase) == DotState.EmptyBase)
                {
                    // Save state for rollback.
                    _surroundDotsPositions.Add(new DotPosition(pos - 1, _dots[pos - 1]));

                    _tempList.Add(pos - 1);
                    _dots[pos - 1] &= ~DotState.EmptyBase;
                }

                if ((_dots[pos - RealWidth] & DotState.EmptyBase) == DotState.EmptyBase)
                {
                    // Save state for rollback.
                    _surroundDotsPositions.Add(new DotPosition(pos - RealWidth, _dots[pos - RealWidth]));

                    _tempList.Add(pos - RealWidth);
                    _dots[pos - RealWidth] &= ~DotState.EmptyBase;
                }

                if ((_dots[pos + 1] & DotState.EmptyBase) == DotState.EmptyBase)
                {
                    // Save state for rollback.
                    _surroundDotsPositions.Add(new DotPosition(pos + 1, _dots[pos + 1]));

                    _tempList.Add(pos + 1);
                    _dots[pos + 1] &= ~DotState.EmptyBase;
                }

                if ((_dots[pos + RealWidth] & DotState.EmptyBase) == DotState.EmptyBase)
                {
                    // Save state for rollback.
                    _surroundDotsPositions.Add(new DotPosition(pos + RealWidth, _dots[pos + RealWidth]));

                    _tempList.Add(pos + RealWidth);
                    _dots[pos + RealWidth] &= ~DotState.EmptyBase;
                }
            }
        }

        /// <summary>
        /// Check closure at last position.
        /// </summary>
        protected virtual void CheckClosure()
        {
            // If putted in empty base.
            if (_dots[LastPosition].IsInEmptyBase())
            {
                CheckForEmptyBase();
            }
            else if (GetInputDots(LastPosition) > 1)
            {
                FindAndMarkChainAndSurroundedDots(LastPosition);
            }
        }

        protected virtual void CheckForEmptyBase()
        {
            // Check player of territoty.
            var chainPosition = LastPosition - 1;
            while (!_dots[chainPosition].IsPutted())
                chainPosition--;

            // If put in own empty base.
            if ((_dots[chainPosition] & DotState.Player) == (_dots[LastPosition] & DotState.Player))
            {
                // Подумать, т.к. здесь могут быть сложности.
                _dots[LastPosition] &= ~DotState.EmptyBase;
                return;
            }

            if (SurroundCondition == enmSurroundCondition.Standart)
            {
                if (GetInputDots(LastPosition) > 1)
                    FindAndMarkChainAndSurroundedDots(LastPosition);
                // If surround anything.
                if (_chainPositions.Count != 0)
                {
                    RemoveEmptyBaseFlags(LastPosition);
                    return;
                }
            }

            // Find first opponent dot.
            chainPosition++;
            var opponentEnableCondition = DotState.Putted | CurrentPlayer.NextPlayer();
            var nextPlayer = CurrentPlayer.NextPlayer();
            do
            {
                chainPosition--;
                while (!_dots[chainPosition].IsEnable(opponentEnableCondition))
                    chainPosition--;

                if (GetInputDots(chainPosition) > 1)
                {
                    _chainPositions.Clear();
                    _surroundPositions.Clear();
                    _chainDotsPositions.Clear();
                    _surroundDotsPositions.Clear();

                    FindAndMarkChainAndSurroundedDots(chainPosition);
                }
            }
            while (_dots[LastPosition].IsZeroSurroundLevel());

            LastMoveCaptureCount = -1;
        }

        /// <summary>
        /// Highly-optimized function for finding and disabling closed territory and its bound.
        /// </summary>
        /// <param name="position">Starting position of Dot in one-dimensional array</param>
        protected virtual void FindAndMarkChainAndSurroundedDots(int position)
        {
            int pos;
            var inputDotsCount = _inputChainDots.Count;

            var dotColor = _dots[position] & DotState.Player;
            var negativeSquare = true;
            var enabledCondition = _dots[position].GetEnabledCondition();

            for (var i = 0; i < _inputChainDots.Count; i++)
            {
                var previousChainDotsCount = _chainPositions.Count;
                _chainPositions.Add((short)position);
                pos = _inputChainDots[i];
                var centerPos = position;

                // Returns square bounded by the triangle with vertexes (0, centerPos, pos)
                int tempSquare = GetSquare(centerPos, pos);
                do
                {
                    if ((_chainPositions.Count > previousChainDotsCount + 1) && (pos == _chainPositions[_chainPositions.Count - 2]))
                        _chainPositions.RemoveAt(_chainPositions.Count - 1);
                    else
                        _chainPositions.Add((short)pos);

                    int t = pos;
                    pos = centerPos;
                    centerPos = t;

                    GetFirstNextPos(centerPos, ref pos);
                    while (!_dots[pos].IsEnable(enabledCondition))
                        GetNextPos(centerPos, ref pos);

                    tempSquare += GetSquare(centerPos, pos);
                }
                while (pos != position);

                // Bypass territory only couter-clockwise (territiry with negative square)
                if (tempSquare <= 0)
                {
                    _chainPositions.RemoveRange(previousChainDotsCount, _chainPositions.Count - previousChainDotsCount);
                    negativeSquare = false;
                }
                else
                {
                    for (var j = previousChainDotsCount; j < _chainPositions.Count; j++)
                    {
                        // Save chain dot states for further rollback.
                        _chainDotsPositions.Add(new DotPosition(_chainPositions[j], _dots[_chainPositions[j]]));

                        // Mark surrounded dots by flag "Bound".
                        _dots[_chainPositions[j]] |= DotState.Bound;
                    }

                    var previousSuroundDotsCount = _surroundPositions.Count;

                    // Find array of dots, bounded by ChainPositions
                    AddCapturedDots(_inputSurroundedDots[i], dotColor);

                    // Changing "Player0CapturedCount" and "BlackCaptureCount".
                    AddCapturedFreedCount(dotColor);

                    // If capture not empty base or turned on an option "Surround all".
                    if (_lastBaseCaptureCount != 0 || SurroundCondition == enmSurroundCondition.Always)
                    {
                        for (var j = previousSuroundDotsCount; j < _surroundPositions.Count; j++)
                        {
                            // Clear tag.
                            _dots[_surroundPositions[j]] = _dots[_surroundPositions[j]] & ~DotState.Tagged;

                            // Save surrounded dot states for further rollback.
                            _surroundDotsPositions.Add(new DotPosition(_surroundPositions[j], _dots[_surroundPositions[j]]));

                            SetCaptureFreeState(_surroundPositions[j], dotColor);
                        }
                    }
                    // If capture empty base.
                    else
                    {
                        for (var j = previousChainDotsCount; j < _chainPositions.Count; j++)
                        {
                            // Clear "Bound" flag and set special flag "EmptyBound".
                            _dots[_chainPositions[j]] &= ~DotState.Bound;
                            _dots[_chainPositions[j]] |= DotState.EmptyBound;
                        }

                        for (var j = previousSuroundDotsCount; j < _surroundPositions.Count; j++)
                        {
                            // Clear tag.
                            _dots[_surroundPositions[j]] &= ~DotState.Tagged;

                            // Save surrounded dot states for further rollback.
                            _surroundDotsPositions.Add(new DotPosition(_surroundPositions[j], _dots[_surroundPositions[j]]));

                            // If dot is not putted in empty territory then set flag "EmptyBase"
                            if ((_dots[_surroundPositions[j]] & DotState.Putted) == 0)
                                _dots[_surroundPositions[j]] |= DotState.EmptyBase;
                        }

                        if (dotColor == DotState.Player0)
                            Player0Square -= _lastSquareCaptureCount;
                        else
                            Player1Square -= _lastSquareCaptureCount;

                        _chainPositions.RemoveRange(previousChainDotsCount, _chainPositions.Count - previousChainDotsCount);
                        _surroundPositions.RemoveRange(previousSuroundDotsCount, _surroundPositions.Count - previousSuroundDotsCount);
                    }

                    // Special optimization.
                    if ((i == _inputChainDots.Count - 2) && negativeSquare)
                        break;
                }
            }
        }

        protected void AddCapturedDots(int startPosition, DotState player)
        {
            DotState boundCondition = player | DotState.Putted | DotState.Bound;

            _dots[startPosition] |= DotState.Tagged;

            _lastBaseCaptureCount = 0;
            _lastBaseFreedCount = 0;
            _lastSquareCaptureCount = 0;
            _lastSquareFreedCount = 0;
            var pos = startPosition;
            _tempList.Clear();
            _tempList.Add(startPosition);

            while (_tempList.Count != 0)
            {
                pos = _tempList.Last();
                _tempList.RemoveAt(_tempList.Count - 1);
                CheckCapturedAndFreed(pos, player);

                _surroundPositions.Add((short)pos);

                if (!_dots[pos - 1].IsBound(boundCondition) && !_dots[pos - 1].IsTagged())
                {
                    _tempList.Add(pos - 1);
                    _dots[pos - 1] |= DotState.Tagged;
                }

                if (!_dots[pos - RealWidth].IsBound(boundCondition) && !_dots[pos - RealWidth].IsTagged())
                {
                    _tempList.Add(pos - RealWidth);
                    _dots[pos - RealWidth] |= DotState.Tagged;
                }

                if (!_dots[pos + 1].IsBound(boundCondition) && !_dots[pos + 1].IsTagged())
                {
                    _tempList.Add(pos + 1);
                    _dots[pos + 1] |= DotState.Tagged;
                }

                if (!_dots[pos + RealWidth].IsBound(boundCondition) && !_dots[pos + RealWidth].IsTagged())
                {
                    _tempList.Add(pos + RealWidth);
                    _dots[pos + RealWidth] |= DotState.Tagged;
                }
            }
        }

        private void CheckCapturedAndFreed(int position, DotState player)
        {
            _lastSquareCaptureCount++;
            if ((_dots[position] & DotState.RealPutted) == DotState.RealPutted)
            {
                if ((_dots[position] & DotState.RealPlayer) != (DotState)((int)player << DotConstants.RealPlayerShift))
                {
                    _lastBaseCaptureCount++;
                    LastMoveCaptureCount++;
                }
                else if ((int)(_dots[position] & DotState.SurroundCountMask) >= (int)DotState.FirstSurroundLevel)
                {
                    _lastBaseFreedCount++;
                    LastMoveFreedCount++;
                    _lastSquareFreedCount++;
                }
            }
        }

        protected void SetCaptureFreeState(int pos, DotState player)
        {
            _dots[pos] |= (DotState)((int)_dots[pos] << DotConstants.RealPlayerShift) & (DotState.RealPlayer | DotState.RealPutted);
            _dots[pos] &= ~DotState.Player;
            _dots[pos] |= DotState.Putted | player | ((_dots[pos] & DotState.SurroundCountMask) + (int)DotState.FirstSurroundLevel);
        }

        protected void AddCapturedFreedCount(DotState dotColor)
        {
            if (dotColor == DotState.Player0)
            {
                Player0CaptureCount += _lastBaseCaptureCount;
                Player1CaptureCount -= _lastBaseFreedCount;
                Player0Square += _lastSquareCaptureCount;
                Player1Square -= _lastSquareFreedCount;
            }
            else
            {
                Player1CaptureCount += _lastBaseCaptureCount;
                Player0CaptureCount -= _lastBaseFreedCount;
                Player1Square += _lastSquareCaptureCount;
                Player0Square -= _lastSquareFreedCount;
            }
        }

        private void SubCapturedFreedCount(DotState dotColor)
        {
            if (dotColor == DotState.Player0)
            {
                if (LastMoveCaptureCount > 0)
                {
                    Player0CaptureCount -= LastMoveCaptureCount;
                    Player1CaptureCount += LastMoveFreedCount;
                }
                else
                    Player1CaptureCount += LastMoveCaptureCount;
            }
            else
            {
                if (LastMoveCaptureCount > 0)
                {
                    Player1CaptureCount -= LastMoveCaptureCount;
                    Player0CaptureCount += LastMoveFreedCount;
                }
                else
                    Player0CaptureCount += LastMoveCaptureCount;
            }
        }

        protected int GetInputDots(int centerPos)
        {
            _inputChainDots.Clear();
            _inputSurroundedDots.Clear();

            DotState enableCond = _dots[centerPos].GetEnabledCondition();
            if (!_dots[centerPos - 1].IsEnable(enableCond))
            {
                if (_dots[centerPos - RealWidth - 1].IsEnable(enableCond))
                {
                    _inputChainDots.Add(centerPos - RealWidth - 1);
                    _inputSurroundedDots.Add(centerPos - 1);
                }
                else if (_dots[centerPos - RealWidth].IsEnable(enableCond))
                {
                    _inputChainDots.Add(centerPos - RealWidth);
                    _inputSurroundedDots.Add(centerPos - 1);
                }
            }

            if (!_dots[centerPos + RealWidth].IsEnable(enableCond))
            {
                if (_dots[centerPos + RealWidth - 1].IsEnable(enableCond))
                {
                    _inputChainDots.Add(centerPos + RealWidth - 1);
                    _inputSurroundedDots.Add(centerPos + RealWidth);
                }
                else if (_dots[centerPos - 1].IsEnable(enableCond))
                {
                    _inputChainDots.Add(centerPos - 1);
                    _inputSurroundedDots.Add(centerPos + RealWidth);
                }
            }

            if (!_dots[centerPos + 1].IsEnable(enableCond))
            {
                if (_dots[centerPos + RealWidth + 1].IsEnable(enableCond))
                {
                    _inputChainDots.Add(centerPos + RealWidth + 1);
                    _inputSurroundedDots.Add(centerPos + 1);
                }
                else if (_dots[centerPos + RealWidth].IsEnable(enableCond))
                {
                    _inputChainDots.Add(centerPos + RealWidth);
                    _inputSurroundedDots.Add(centerPos + 1);
                }
            }

            if (!_dots[centerPos - RealWidth].IsEnable(enableCond))
            {
                if (_dots[centerPos - RealWidth + 1].IsEnable(enableCond))
                {
                    _inputChainDots.Add(centerPos - RealWidth + 1);
                    _inputSurroundedDots.Add(centerPos - RealWidth);
                }
                else if (_dots[centerPos + 1].IsEnable(enableCond))
                {
                    _inputChainDots.Add(centerPos + 1);
                    _inputSurroundedDots.Add(centerPos - RealWidth);
                }
            }

            return _inputChainDots.Count();
        }

        private bool GetInputDotForEmptyBase(int centerPos, DotState Player,
            out int inputChainDot, out int inputSurroundedDot)
        {
            if (_dots[centerPos + 1].IsPlayerPutted(Player))
            {
                inputChainDot = 0;
                inputSurroundedDot = 0;
                return false;
            }

            inputSurroundedDot = centerPos + 1;
            var pos = inputSurroundedDot;
            GetNextPos(centerPos, ref pos);
            var k = 0;
            while (!_dots[pos].IsPlayerPutted(Player) && (k < 8))
            {
                GetNextPos(centerPos, ref pos);
                k++;
            }
            inputChainDot = pos;

            return k != 8;
        }

        private void FillBadValues()
        {
            for (int i = 0; i < RealWidth; i++)
                _dots[i] = DotState.Invalid;

            for (int i = 1; i < Height + 1; i++)
            {
                _dots[i * RealWidth] = DotState.Invalid;
                for (int j = Width + 1; j < RealWidth; j++)
                    _dots[i * RealWidth + j] = DotState.Invalid;
            }

            for (int i = _dots.Length - RealWidth; i < _dots.Length; i++)
                _dots[i] = DotState.Invalid;
        }

        #endregion

        #region Public methods

        public List<int> GetInputDots(int centerPos, DotState player)
        {
            var result = new List<int>(4);

            DotState enableCond = player | DotState.Putted;
            if (!_dots[centerPos - 1].IsEnable(enableCond))
            {
                if (_dots[centerPos - RealWidth - 1].IsEnable(enableCond))
                    result.Add(centerPos - RealWidth - 1);
                else if (_dots[centerPos - RealWidth].IsEnable(enableCond))
                    result.Add(centerPos - RealWidth);
            }

            if (!_dots[centerPos + RealWidth].IsEnable(enableCond))
            {
                if (_dots[centerPos + RealWidth - 1].IsEnable(enableCond))
                    result.Add(centerPos + RealWidth - 1);
                else if (_dots[centerPos - 1].IsEnable(enableCond))
                    result.Add(centerPos - 1);
            }

            if (!_dots[centerPos + 1].IsEnable(enableCond))
            {
                if (_dots[centerPos + RealWidth + 1].IsEnable(enableCond))
                    result.Add(centerPos + RealWidth + 1);
                else if (_dots[centerPos + RealWidth].IsEnable(enableCond))
                    result.Add(centerPos + RealWidth);
            }

            if (!_dots[centerPos - RealWidth].IsEnable(enableCond))
            {
                if (_dots[centerPos - RealWidth + 1].IsEnable(enableCond))
                    result.Add(centerPos - RealWidth + 1);
                else if (_dots[centerPos + 1].IsEnable(enableCond))
                    result.Add(centerPos + 1);
            }

            return result;
        }

        public bool IsBaseAddedAtLastMove
        {
            get
            {
                return _chainDotsPositions.Count != 0 && _chainDotsPositions.Count != 0 && !_emptyBaseCreated;
            }
        }

        public static void GetPosition(int position, out int x, out int y)
        {
            x = position % RealWidth;
            y = position / RealWidth;
        }

        public bool IsValidPos(int position)
        {
            int x = position % RealWidth;
            int y = position / RealWidth;
            return x >= 1 && x <= Width && y >= 1 && y <= Height;
        }

        public static int GetPosition(int x, int y)
        {
            return y * RealWidth + x;
        }

        public bool MakeMove(int x, int y)
        {
            return MakeMove(y * RealWidth + x);
        }

        public bool MakeMove(int position, DotState color)
        {
            DotState oldCurrentPlayer = CurrentPlayer;
            CurrentPlayer = color;
            if (MakeMove(position))
                return true;
            else
            {
                CurrentPlayer = oldCurrentPlayer;
                return false;
            }
        }

        public bool MakeMove(int x, int y, int playerNumber)
        {
            DotState oldCurrentPlayer = CurrentPlayer;
            CurrentPlayer = (DotState)playerNumber;
            if (MakeMove(y * RealWidth + x))
                return true;
            else
            {
                CurrentPlayer = oldCurrentPlayer;
                return false;
            }
        }

        public bool MakeMove(int position)
        {
            if (_dots[position].IsPuttingAllowed())
            {
                DotState oldDot = _dots[position];

                _dots[position] |= DotState.Putted;
                _dots[position] |= CurrentPlayer;
                _dots[position] |= DotState.RealPutted;
                _dots[position] |= (DotState)((int)CurrentPlayer << DotConstants.RealPlayerShift);
                LastPosition = position;

                _chainPositions.Clear();
                _surroundPositions.Clear();
                _chainDotsPositions.Clear();
                _surroundDotsPositions.Clear();
                LastMoveCaptureCount = 0;
                LastMoveFreedCount = 0;
                OldPlayer0CaptureCount = Player0CaptureCount;
                OldPlayer1CaptureCount = Player1CaptureCount;
                var oldPlayer0Square = Player0Square;
                var oldPlayer1Square = Player1Square;
                var oldDiagonalLinkedGroupsCount = DiagonalLinkedGroupsCount;

                CheckClosure();

                // Save current state for rollback.
                _dotsSequenceStates.Add(new State()
                {
                    Move = new DotPosition(position, oldDot),
                    Base = _chainDotsPositions.Count == 0 && _surroundDotsPositions.Count == 0 ? null :
                            new Base(LastMoveCaptureCount, LastMoveFreedCount,
                            new List<DotPosition>(_chainDotsPositions), new List<DotPosition>(_surroundDotsPositions),
                            new List<short>(_chainPositions), new List<short>(_surroundPositions), oldPlayer0Square, oldPlayer1Square),
                    DiagonalGroupCount = oldDiagonalLinkedGroupsCount
                });
                LastState = _dotsSequenceStates[_dotsSequenceStates.Count - 1];

                CurrentPlayer = CurrentPlayer.NextPlayer();

                LastMoveState = enmMoveState.Add;
                return true;
            }
            else
            {
                LastMoveState = enmMoveState.None;
                return false;
            }
        }

        public bool UnmakeMove()
        {
            if (_dotsSequenceStates.Count > 0)
            {
                LastState = _dotsSequenceStates[_dotsSequenceStates.Count - 1];

                if (LastState.Base != null)
                {
                    _emptyBaseCreated =
                        LastState.Base.LastCaptureCount + LastState.Base.LastFreedCount == 0 ? true : false;

                    for (int i = LastState.Base.ChainDotPositions.Count - 1; i >= 0; i--)
                    {
                        _dots[LastState.Base.ChainDotPositions[i].Position] = LastState.Base.ChainDotPositions[i].Dot;
                    }

                    for (int i = LastState.Base.SurrroundDotPositions.Count - 1; i >= 0; i--)
                    {
                        _dots[LastState.Base.SurrroundDotPositions[i].Position] = LastState.Base.SurrroundDotPositions[i].Dot;
                    }

                    LastMoveCaptureCount = LastState.Base.LastCaptureCount;
                    LastMoveFreedCount = LastState.Base.LastFreedCount;

                    OldPlayer0CaptureCount = Player0CaptureCount;
                    OldPlayer1CaptureCount = Player1CaptureCount;

                    SubCapturedFreedCount(_dots[LastState.Move.Position].GetPlayer());

                    _chainDotsPositions = LastState.Base.ChainDotPositions;
                    _surroundDotsPositions = LastState.Base.SurrroundDotPositions;
                    _chainPositions = LastState.Base.ChainPositions;
                    _surroundPositions = LastState.Base.SurroundPositions;
                    Player0Square = LastState.Base.Player0Square;
                    Player1Square = LastState.Base.Player1Square;
                }
                else
                {
                    _chainPositions.Clear();
                    _surroundPositions.Clear();
                    _chainDotsPositions.Clear();
                    _surroundDotsPositions.Clear();
                    LastMoveCaptureCount = 0;
                    LastMoveFreedCount = 0;
                }

                _dots[LastState.Move.Position] = LastState.Move.Dot;
                CurrentPlayer = CurrentPlayer.NextPlayer();
                _dotsSequenceStates.RemoveAt(_dotsSequenceStates.Count - 1);

                LastPosition = LastState.Move.Position;
                DiagonalLinkedGroupsCount = LastState.DiagonalGroupCount;

                LastMoveState = enmMoveState.Remove;
                return true;
            }
            else
            {
                LastMoveState = enmMoveState.None;
                return false;
            }
        }

        public bool UnmakeAllMoves()
        {
            while (_dotsSequenceStates.Count != 0)
                if (!UnmakeMove())
                    return false;
            return true;
        }

        public DotState[] CloneDots()
        {
            return (DotState[])_dots.Clone();
        }

        public Field Clone()
        {
            var result = new Field();
            result._tempList = new List<int>(_tempList);
            result._chainPositions = new List<short>(_chainPositions);
            result._surroundPositions = new List<short>(_surroundPositions);
            result._emptyBaseCreated = _emptyBaseCreated;
            result.Player0CaptureCount = Player0CaptureCount;
            result.Player1CaptureCount = Player1CaptureCount;
            result.OldPlayer0CaptureCount = OldPlayer0CaptureCount;
            result.OldPlayer1CaptureCount = OldPlayer1CaptureCount;
            result.Player0Square = Player0Square;
            result.Player1Square = Player1Square;
            result.CurrentPlayer = CurrentPlayer;
            result.LastPosition = LastPosition;
            //result.LastBaseCaptureCount_ = LastBaseCaptureCount_;
            //result.LastBaseFreedCount_ = LastBaseFreedCount_;
            result.LastMoveCaptureCount = LastMoveCaptureCount;
            result.LastMoveFreedCount = LastMoveFreedCount;
            //result.LastSquareCaptureCount_ = LastSquareCaptureCount_;
            //result.LastSquareFreedCount_ = LastSquareFreedCount_;
            result.LastState = LastState.Clone();
            result.LastMoveState = LastMoveState;
            result._chainDotsPositions = new List<DotPosition>(_chainDotsPositions);
            result._surroundDotsPositions = new List<DotPosition>(_surroundDotsPositions);
            result._dots = (DotState[])_dots.Clone();
            var newDotsSequanceStates = new List<State>(_dotsSequenceStates.Capacity);
            _dotsSequenceStates.ForEach(state => newDotsSequanceStates.Add(state.Clone()));
            result._dotsSequenceStates = newDotsSequanceStates;
            result._inputChainDots = new List<int>(InputSurroundDotsCount);
            result._inputSurroundedDots = new List<int>(InputSurroundDotsCount);
            result.Width = Width;
            result.Height = Height;
            result.SurroundCondition = SurroundCondition;
            result.DiagonalLinkedGroupsCount = DiagonalLinkedGroupsCount;
            return result;
        }

        public DotState GetDot(int x, int y)
        {
            return _dots[Field.GetPosition(x, y)];
        }

        public State GetState(int ind)
        {
            return _dotsSequenceStates[ind];
        }

        #endregion

        #region Methods for tests

        public bool IsEmpty
        {
            get
            {
                for (int i = 1; i <= Width; i++)
                    for (int j = 1; j <= Height; j++)
                        if (_dots[GetPosition(i, j)] != DotState.Empty)
                            return false;
                return true;
            }
        }

        public IEnumerable<DotPosition> NotZeroPositions
        {
            get
            {
                var result = new List<DotPosition>();
                for (int i = 1; i < Width; i++)
                    for (int j = 1; j < Height; j++)
                        if (_dots[GetPosition(i, j)] != DotState.Empty)
                            result.Add(new DotPosition(GetPosition(i, j), _dots[GetPosition(i, j)]));
                return result;
            }
        }

        #endregion
    }
}
