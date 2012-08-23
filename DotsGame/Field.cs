using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotsGame
{
	public sealed class Field
	{
		#region Constatnts and masks

		public static int[] DiagVertHorizDeltas = {
			-Field.RealWidth - 1, -Field.RealWidth, -Field.RealWidth + 1,
			-1, +1, 
			+Field.RealWidth - 1, +Field.RealWidth, +Field.RealWidth + 1 };

		public static int[] VertHorizDeltas = { -Field.RealWidth, +1, +Field.RealWidth, -1 };

		public const int RealWidth = 64;
		public const int InputSurroundDotsCount = 4;

		#endregion

		#region Fields

		private List<int> TempList_;
		private List<int> ChainPositions_;
		private List<int> SurroundPositions_;
		private bool EmptyBaseCreated_;
		private int LastBaseCaptureCount_;
		private int LastBaseFreedCount_;
		private int LastSquareCaptureCount_;
		private int LastSquareFreedCount_;

		/// <summary>
		/// Indexes and states of last move chain dots.
		/// </summary>
		private List<DotPosition> ChainDotsPositions_;

		/// <summary>
		/// Indexes and states of last move surround dots.
		/// </summary>
		private List<DotPosition> SurroundDotsPositions_;

		/// <summary>
		/// Array of dots or dot states
		/// </summary>
		private Dot[] Dots_;

		/// <summary>
		/// Sequance of putted dots.
		/// Using for moves rollback.
		/// </summary>
		private List<State> DotsSequanceStates_;

		private List<int> InputChainDots_;

		private List<int> InputSurroundedDots_;

		#endregion

		#region Readonly

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

		public Dot this[int i]
		{
			get
			{
				return Dots_[i];
			}
		}

		public IEnumerable<State> DotsSequanceStates
		{
			get
			{
				return DotsSequanceStates_;
			}
		}

		public IEnumerable<int> DotsSequancePositions
		{
			get
			{
				return DotsSequanceStates.Select(state => state.Move.Position);
			}
		}

		public int RedCaptureCount
		{
			get;
			private set;
		}

		public int BlueCaptureCount
		{
			get;
			private set;
		}

		public int OldRedCaptureCount
		{
			get;
			private set;
		}

		public int OldBlueCaptureCount
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
				return Dots_.Length;
			}
		}

		public int DotsSequenceCount
		{
			get
			{
				return DotsSequanceStates_.Count;
			}
		}

		public int RedSquare
		{
			get;
			private set;
		}

		public int BlueSquare
		{
			get;
			private set;
		}

		#endregion

		#region Last Move

		public Dot CurrentPlayer
		{
			get;
			private set;
		}

		public int LastPosition
		{
			get;
			private set;
		}

		public int LastMoveCaptureCount
		{
			get;
			private set;
		}

		public int LastMoveFreedCount
		{
			get;
			private set;
		}

		/// <summary>
		/// Indexes of last move chain dots.
		/// </summary>
		public IEnumerable<int> ChainPositions
		{
			get
			{
				return ChainPositions_;
			}
		}

		/// <summary>
		/// Indexes of last move surorund dots.
		/// </summary>
		public IEnumerable<int> SurroundPositions
		{
			get
			{
				return SurroundPositions_;
			}
		}

		public State LastState
		{
			get;
			private set;
		}

		#endregion

		#region Constructors

		private Field()
		{
		}

		public Field(int width, int height, enmSurroundCondition surroundCondition = enmSurroundCondition.Standart)
		{
			Width = width;
			Height = height;
			SurroundCondition = surroundCondition;

			Dots_ = new Dot[RealWidth * (height + 2)];
			FillBadValues();

			DotsSequanceStates_ = new List<State>(64);

			ChainPositions_ = new List<int>(16);
			SurroundPositions_ = new List<int>(InputSurroundDotsCount);
			ChainDotsPositions_ = new List<DotPosition>(16);
			SurroundDotsPositions_ = new List<DotPosition>(InputSurroundDotsCount);
			TempList_ = new List<int>(16);
			InputChainDots_ = new List<int>(InputSurroundDotsCount);
			InputSurroundedDots_ = new List<int>(InputSurroundDotsCount);
			LastMoveState = enmMoveState.None;
		}

		#endregion

		#region Private and protected methods

		/// <summary>
		/// Returns square bounded by the triangle with vertexes (0, pos1, pos2)
		/// </summary>
		/// <param name="pos1"></param>
		/// <param name="pos2"></param>
		/// <returns></returns>
		private static int GetSquare(int pos1, int pos2)
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
		private static void GetFirstNextPos(int centerPosition, ref int pos)
		{
			pos = centerPosition + Helper.NextFirstPosOffsets_[pos - centerPosition + Field.RealWidth + 1];
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
		private static void GetNextPos(int centerPosition, ref int pos)
		{
			pos = centerPosition + Helper.NextPosOffsets_[pos - centerPosition + Field.RealWidth + 1];
		}

		private void RemoveEmptyBaseFlags(int startPosition)
		{
			SurroundDotsPositions_.Add(new DotPosition(startPosition, Dots_[startPosition]));
			Dots_[startPosition] &= ~Dot.EmptyBase;

			var pos = startPosition;
			TempList_.Clear();
			TempList_.Add(startPosition);

			while (TempList_.Count != 0)
			{
				pos = TempList_.Last();
				TempList_.RemoveAt(TempList_.Count - 1);

				if ((Dots_[pos - 1] & Dot.EmptyBase) == Dot.EmptyBase)
				{
					// Save state for rollback.
					SurroundDotsPositions_.Add(new DotPosition(pos - 1, Dots_[pos - 1]));

					TempList_.Add(pos - 1);
					Dots_[pos - 1] &= ~Dot.EmptyBase;
				}

				if ((Dots_[pos - RealWidth] & Dot.EmptyBase) == Dot.EmptyBase)
				{
					// Save state for rollback.
					SurroundDotsPositions_.Add(new DotPosition(pos - RealWidth, Dots_[pos - RealWidth]));

					TempList_.Add(pos - RealWidth);
					Dots_[pos - RealWidth] &= ~Dot.EmptyBase;
				}

				if ((Dots_[pos + 1] & Dot.EmptyBase) == Dot.EmptyBase)
				{
					// Save state for rollback.
					SurroundDotsPositions_.Add(new DotPosition(pos + 1, Dots_[pos + 1]));

					TempList_.Add(pos + 1);
					Dots_[pos + 1] &= ~Dot.EmptyBase;
				}

				if ((Dots_[pos + RealWidth] & Dot.EmptyBase) == Dot.EmptyBase)
				{
					// Save state for rollback.
					SurroundDotsPositions_.Add(new DotPosition(pos + RealWidth, Dots_[pos + RealWidth]));

					TempList_.Add(pos + RealWidth);
					Dots_[pos + RealWidth] &= ~Dot.EmptyBase;
				}
			}
		}

		/// <summary>
		/// Check closure at last position.
		/// </summary>
		private void CheckClosure()
		{
			// If putted in empty base.
			if ((Dots_[LastPosition] & Dot.EmptyBase) == Dot.EmptyBase)
			{
				// Check player of territoty.
				var chainPosition = LastPosition - 1;
				while (!Dots_[chainPosition].IsPutted())
					chainPosition--;

				// If put in own empty base.
				if ((Dots_[chainPosition] & Dot.Player) == (Dots_[LastPosition] & Dot.Player))
				{
					// Подумать, т.к. здесь могут быть сложности.
					Dots_[LastPosition] &= ~Dot.EmptyBase;
					return;
				}

				if (SurroundCondition == enmSurroundCondition.Standart)
				{
					if (GetInputDots(LastPosition) > 1)
						FindAndMarkChainAndSurroundedDots(LastPosition);
					// If surround anything.
					if (ChainPositions_.Count != 0)
					{
						RemoveEmptyBaseFlags(LastPosition);
						return;
					}
				}

				// Find first opponent dot.
				chainPosition++;
				var opponentEnableCondition = Dot.Putted | CurrentPlayer.NextPlayer();
				var nextPlayer = CurrentPlayer.NextPlayer();
				do
				{
					chainPosition--;
					while (!Dots_[chainPosition].IsEnable(opponentEnableCondition))
						chainPosition--;

					int inputChainDot, inputSurroundedDot;
					//if (GetInputDotForEmptyBase(chainPosition, nextPlayer, out inputChainDot, out inputSurroundedDot))
					if (GetInputDots(chainPosition) > 1)
					{
						/*InputChainDots_.Clear();
						InputChainDots_.Add(inputChainDot);
						InputSurroundedDots_.Clear();
						InputSurroundedDots_.Add(inputSurroundedDot);
						*/
						ChainPositions_.Clear();
						SurroundPositions_.Clear();
						ChainDotsPositions_.Clear();
						SurroundDotsPositions_.Clear();
						
						FindAndMarkChainAndSurroundedDots(chainPosition);
					}
				}
				while (Dots_[LastPosition].IsZeroSurroundLevel());

				LastMoveCaptureCount = -1;
			}
			else
			{
				if (GetInputDots(LastPosition) > 1)
					FindAndMarkChainAndSurroundedDots(LastPosition);
			}
		}

		/// <summary>
		/// Highly-optimized function for finding and disabling closed territory and its bound.
		/// </summary>
		/// <param name="position">Starting position of Dot in one-dimensional array</param>
		private void FindAndMarkChainAndSurroundedDots(int position)
		{
			int pos;
			var inputDotsCount = InputChainDots_.Count;

			var dotColor = Dots_[position] & Dot.Player;
			var negativeSquare = true;
			var enabledCondition = Dots_[position].GetEnabledCondition();

			for (var i = 0; i < InputChainDots_.Count; i++)
			{
				var previousChainDotsCount = ChainPositions_.Count;
				ChainPositions_.Add(position);
				pos = InputChainDots_[i];
				var centerPos = position;

				// Returns square bounded by the triangle with vertexes (0, centerPos, pos)
				int tempSquare = GetSquare(centerPos, pos);
				do
				{
					if ((ChainPositions_.Count > previousChainDotsCount + 1) && (pos == ChainPositions_[ChainPositions_.Count - 2]))
						ChainPositions_.RemoveAt(ChainPositions_.Count - 1);
					else
						ChainPositions_.Add(pos);

					int t = pos;
					pos = centerPos;
					centerPos = t;

					GetFirstNextPos(centerPos, ref pos);
					while (!Dots_[pos].IsEnable(enabledCondition))
						GetNextPos(centerPos, ref pos);

					tempSquare += GetSquare(centerPos, pos);
				}
				while (pos != position);

				// Bypass territory only couter-clockwise (territiry wtih negative square)
				if (tempSquare <= 0)
				{
					ChainPositions_.RemoveRange(previousChainDotsCount, ChainPositions_.Count - previousChainDotsCount);
					negativeSquare = false;
				}
				else
				{
					for (var j = previousChainDotsCount; j < ChainPositions_.Count; j++)
					{
						// Save chain dot states for further rollback.
						ChainDotsPositions_.Add(new DotPosition(ChainPositions_[j], Dots_[ChainPositions_[j]]));

						// Mark surrounded dots by flag "Bound".
						Dots_[ChainPositions_[j]] |= Dot.Bound;
					}

					var previousSuroundDotsCount = SurroundPositions_.Count;

					// Find array of dots, bounded by ChainPositions
					AddCapturedDots(InputSurroundedDots_[i], dotColor);

					// Changing "RedCapturedCount" and "BlackCaptureCount".
					AddCapturedFreedCount(dotColor);

					// If capture not empty base or turned on an option "Surround all".
					if ((LastMoveCaptureCount != 0) || (SurroundCondition == enmSurroundCondition.Always))
					{
						for (var j = previousSuroundDotsCount; j < SurroundPositions_.Count; j++)
						{
							// Clear tag.
							Dots_[SurroundPositions_[j]] = Dots_[SurroundPositions_[j]] & ~Dot.Tagged;

							// Save surrounded dot states for further rollback.
							SurroundDotsPositions_.Add(new DotPosition(SurroundPositions_[j], Dots_[SurroundPositions_[j]]));

							SetCaptureFreeState(SurroundPositions_[j], dotColor);
						}
					}
					// If capture empty base.
					else
					{
						for (var j = previousChainDotsCount; j < ChainPositions_.Count; j++)
						{
							// Clear "Bound" flag and set special flag "EmptyBound".
							Dots_[ChainPositions_[j]] &= ~Dot.Bound;
							Dots_[ChainPositions_[j]] |= Dot.EmptyBound;
						}

						for (var j = previousSuroundDotsCount; j < SurroundPositions_.Count; j++)
						{
							// Clear tag.
							Dots_[SurroundPositions_[j]] &= ~Dot.Tagged;

							// Save surrounded dot states for further rollback.
							SurroundDotsPositions_.Add(new DotPosition(SurroundPositions_[j], Dots_[SurroundPositions_[j]]));

							// If dot is not putted in empty territory then set flag "EmptyBase"
							if ((Dots_[SurroundPositions_[j]] & Dot.Putted) == 0)
								Dots_[SurroundPositions_[j]] |= Dot.EmptyBase;
						}

						if (dotColor == Dot.RedPlayer)
							RedSquare -= LastSquareCaptureCount_;
						else
							BlueSquare -= LastSquareCaptureCount_;

						ChainPositions_.RemoveRange(previousChainDotsCount, ChainPositions_.Count - previousChainDotsCount);
						SurroundPositions_.RemoveRange(previousSuroundDotsCount, SurroundPositions_.Count - previousSuroundDotsCount);
					}

					// Special optimization.
					if ((i == InputChainDots_.Count - 2) && negativeSquare)
						break;
				}
			}
		}

		private void AddCapturedDots(int startPosition, Dot player)
		{
			Dot boundCondition = player | Dot.Putted | Dot.Bound;

			Dots_[startPosition] |= Dot.Tagged;

			LastBaseCaptureCount_ = 0;
			LastBaseFreedCount_ = 0;
			LastSquareCaptureCount_ = 0;
			LastSquareFreedCount_ = 0;
			var pos = startPosition;
			TempList_.Clear();
			TempList_.Add(startPosition);

			while (TempList_.Count != 0)
			{
				pos = TempList_.Last();
				TempList_.RemoveAt(TempList_.Count - 1);
				CheckCapturedAndFreed(pos, player);

				SurroundPositions_.Add(pos);

				if (!Dots_[pos - 1].IsBound(boundCondition) && !Dots_[pos - 1].IsTagged())
				{
					TempList_.Add(pos - 1);
					Dots_[pos - 1] |= Dot.Tagged;
				}

				if (!Dots_[pos - RealWidth].IsBound(boundCondition) && !Dots_[pos - RealWidth].IsTagged())
				{
					TempList_.Add(pos - RealWidth);
					Dots_[pos - RealWidth] |= Dot.Tagged;
				}

				if (!Dots_[pos + 1].IsBound(boundCondition) && !Dots_[pos + 1].IsTagged())
				{
					TempList_.Add(pos + 1);
					Dots_[pos + 1] |= Dot.Tagged;
				}

				if (!Dots_[pos + RealWidth].IsBound(boundCondition) && !Dots_[pos + RealWidth].IsTagged())
				{
					TempList_.Add(pos + RealWidth);
					Dots_[pos + RealWidth] |= Dot.Tagged;
				}
			}
		}

		private void CheckCapturedAndFreed(int position, Dot player)
		{
			LastSquareCaptureCount_++;
			if ((Dots_[position] & Dot.RealPutted) == Dot.RealPutted)
			{
				if ((Dots_[position] & Dot.RealPlayer) != (Dot)((int)player << DotConstants.RealPlayerShift))
				{
					LastBaseCaptureCount_++;
					LastMoveCaptureCount++;
				}
				else if ((int)(Dots_[position] & Dot.SurroundCountMask) >= (int)Dot.FirstSurroundLevel)
				{
					LastBaseFreedCount_++;
					LastMoveFreedCount++;

					LastSquareFreedCount_++;
				}
			}
		}

		private void SetCaptureFreeState(int pos, Dot player)
		{
			Dots_[pos] |= (Dot)((int)Dots_[pos] << DotConstants.RealPlayerShift) & (Dot.RealPlayer | Dot.RealPutted);
			Dots_[pos] &= ~Dot.Player;
			Dots_[pos] |= Dot.Putted | player | ((Dots_[pos] & Dot.SurroundCountMask) + (int)Dot.FirstSurroundLevel);
		}

		private void AddCapturedFreedCount(Dot dotColor)
		{
			if (dotColor == Dot.RedPlayer)
			{
				RedCaptureCount += LastBaseCaptureCount_;
				BlueCaptureCount -= LastBaseFreedCount_;
				RedSquare += LastSquareCaptureCount_;
				BlueSquare -= LastSquareFreedCount_;
			}
			else
			{
				BlueCaptureCount += LastBaseCaptureCount_;
				RedCaptureCount -= LastBaseFreedCount_;
				BlueSquare += LastSquareCaptureCount_;
				RedSquare -= LastSquareFreedCount_;
			}
		}

		private void SubCapturedFreedCount(Dot dotColor)
		{
			if (dotColor == Dot.RedPlayer)
			{
				if (LastMoveCaptureCount > 0)
				{
					RedCaptureCount -= LastMoveCaptureCount;
					BlueCaptureCount += LastMoveFreedCount;
				}
				else
					BlueCaptureCount += LastMoveCaptureCount;
			}
			else
			{
				if (LastMoveCaptureCount > 0)
				{
					BlueCaptureCount -= LastMoveCaptureCount;
					RedCaptureCount += LastMoveFreedCount;
				}
				else
					RedCaptureCount += LastMoveCaptureCount;
			}
		}

		private int GetInputDots(int centerPos)
		{
			InputChainDots_.Clear();
			InputSurroundedDots_.Clear();

			Dot enableCond = Dots_[centerPos] & Dot.EnableMask;
			if (!Dots_[centerPos - 1].IsEnable(enableCond))
			{
				if (Dots_[centerPos - RealWidth - 1].IsEnable(enableCond))
				{
					InputChainDots_.Add(centerPos - RealWidth - 1);
					InputSurroundedDots_.Add(centerPos - 1);
				}
				else if (Dots_[centerPos - RealWidth].IsEnable(enableCond))
				{
					InputChainDots_.Add(centerPos - RealWidth);
					InputSurroundedDots_.Add(centerPos - 1);
				}
			}

			if (!Dots_[centerPos + RealWidth].IsEnable(enableCond))
			{
				if (Dots_[centerPos + RealWidth - 1].IsEnable(enableCond))
				{
					InputChainDots_.Add(centerPos + RealWidth - 1);
					InputSurroundedDots_.Add(centerPos + RealWidth);

				}
				else if (Dots_[centerPos - 1].IsEnable(enableCond))
				{
					InputChainDots_.Add(centerPos - 1);
					InputSurroundedDots_.Add(centerPos + RealWidth);
				}
			}

			if (!Dots_[centerPos + 1].IsEnable(enableCond))
			{
				if (Dots_[centerPos + RealWidth + 1].IsEnable(enableCond))
				{
					InputChainDots_.Add(centerPos + RealWidth + 1);
					InputSurroundedDots_.Add(centerPos + 1);
				}
				else if (Dots_[centerPos + RealWidth].IsEnable(enableCond))
				{
					InputChainDots_.Add(centerPos + RealWidth);
					InputSurroundedDots_.Add(centerPos + 1);
				}
			}

			if (!Dots_[centerPos - RealWidth].IsEnable(enableCond))
			{
				if (Dots_[centerPos - RealWidth + 1].IsEnable(enableCond))
				{
					InputChainDots_.Add(centerPos - RealWidth + 1);
					InputSurroundedDots_.Add(centerPos - RealWidth);
				}
				else if (Dots_[centerPos + 1].IsEnable(enableCond))
				{
					InputChainDots_.Add(centerPos + 1);
					InputSurroundedDots_.Add(centerPos - RealWidth);
				}
			}

			return InputChainDots_.Count();
		}

		private bool GetInputDotForEmptyBase(int centerPos, Dot Player, out int inputChainDot, out int inputSurroundedDot)
		{
			if (Dots_[centerPos + 1].IsPlayerPutted(Player))
			{
				inputChainDot = 0;
				inputSurroundedDot = 0;
				return false;
			}

			inputSurroundedDot = centerPos + 1;
			var pos = inputSurroundedDot;
			GetNextPos(centerPos, ref pos);
			var k = 0;
			while (!Dots_[pos].IsPlayerPutted(Player) && (k < 8))
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
				Dots_[i] = Dot.Invalid;

			for (int i = 1; i < Height + 1; i++)
			{
				Dots_[i * RealWidth] = Dot.Invalid;
				for (int j = Width + 1; j < RealWidth; j++)
					Dots_[i * RealWidth + j] = Dot.Invalid;
			}

			for (int i = Dots_.Length - RealWidth; i < Dots_.Length; i++)
				Dots_[i] = Dot.Invalid;
		}

		#endregion

		#region Public methods

		public bool IsBaseAddedAtLastMove
		{
			get
			{
				return ChainDotsPositions_.Count != 0 && ChainDotsPositions_.Count != 0 && !EmptyBaseCreated_;
			}
		}

		public static void GetPosition(int position, out int x, out int y)
		{
			x = position % RealWidth;
			y = position / RealWidth;
		}

		public bool IsValidPos(int position)
		{
			return position > Field.RealWidth + 1 && position < Dots_.Length - Field.RealWidth;
		}

		public static int GetPosition(int x, int y)
		{
			return y * RealWidth + x;
		}

		public bool MakeMove(int x, int y)
		{
			return MakeMove(y * RealWidth + x);
		}

		public bool MakeMove(int position, Dot color)
		{
			Dot oldCurrentPlayer = CurrentPlayer;
			CurrentPlayer = color;
			if (MakeMove(position))
				return true;
			else
			{
				CurrentPlayer = oldCurrentPlayer;
				return false;
			}
		}

		public bool MakeMove(int x, int y, Dot color)
		{
			Dot oldCurrentPlayer = CurrentPlayer;
			CurrentPlayer = color;
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
			if (Dots_[position].IsPuttingAllowed())
			{
				Dot oldDot = Dots_[position];

				Dots_[position] |= Dot.Putted;
				Dots_[position] |= CurrentPlayer;
				Dots_[position] |= Dot.RealPutted;
				Dots_[position] |= (Dot)((int)CurrentPlayer << DotConstants.RealPlayerShift);
				LastPosition = position;

				ChainPositions_.Clear();
				SurroundPositions_.Clear();
				ChainDotsPositions_.Clear();
				SurroundDotsPositions_.Clear();
				LastMoveCaptureCount = 0;
				LastMoveFreedCount = 0;
				OldRedCaptureCount = RedCaptureCount;
				OldBlueCaptureCount = BlueCaptureCount;
				var oldRedSquare = RedSquare;
				var oldBlueSquare = BlueSquare;

				CheckClosure();

				// Save current state for rollback.
				DotsSequanceStates_.Add(new State()
					{
						Move = new DotPosition(position, oldDot),
						Base = ChainDotsPositions_.Count == 0 ? null :
							new Base(LastMoveCaptureCount, LastMoveFreedCount,
								new List<DotPosition>(ChainDotsPositions_), new List<DotPosition>(SurroundDotsPositions_),
								new List<int>(ChainPositions_), new List<int>(SurroundPositions_), oldRedSquare, oldBlueSquare)
					});

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
			if (DotsSequanceStates_.Count > 0)
			{
				LastState = DotsSequanceStates_[DotsSequanceStates_.Count - 1];

				if (LastState.Base != null)
				{
					EmptyBaseCreated_ =
						LastState.Base.LastCaptureCount + LastState.Base.LastFreedCount == 0 ? true : false;

					foreach (var dotPosition in LastState.Base.ChainDotPositions)
						Dots_[dotPosition.Position] = dotPosition.Dot;

					foreach (var dotPosition in LastState.Base.SurrroundDotPositions)
						Dots_[dotPosition.Position] = dotPosition.Dot;

					LastMoveCaptureCount = LastState.Base.LastCaptureCount;
					LastMoveFreedCount = LastState.Base.LastFreedCount;

					OldRedCaptureCount = RedCaptureCount;
					OldBlueCaptureCount = BlueCaptureCount;

					SubCapturedFreedCount(CurrentPlayer.NextPlayer());

					ChainDotsPositions_ = LastState.Base.ChainDotPositions;
					SurroundDotsPositions_ = LastState.Base.SurrroundDotPositions;
					ChainPositions_ = LastState.Base.ChainPositions;
					SurroundPositions_ = LastState.Base.SurroundPoistions;
					RedSquare = LastState.Base.RedSquare;
					BlueSquare = LastState.Base.BlueSquare;
				}
				else
				{
					ChainPositions_.Clear();
					SurroundPositions_.Clear();
					ChainDotsPositions_.Clear();
					SurroundDotsPositions_.Clear();
					LastMoveCaptureCount = 0;
					LastMoveFreedCount = 0;
				}

				Dots_[LastState.Move.Position] = LastState.Move.Dot;
				CurrentPlayer = CurrentPlayer.NextPlayer();
				DotsSequanceStates_.RemoveAt(DotsSequanceStates_.Count - 1);

				LastPosition = LastState.Move.Position;
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
			while (DotsSequanceStates_.Count != 0)
				if (!UnmakeMove())
					return false;
			return true;
		}

		#endregion

		public Dot[] CloneDots()
		{
			return (Dot[])Dots_.Clone();
		}

		public Field Clone()
		{
			var result = new Field();
			result.TempList_ = new List<int>(TempList_);
			result.ChainPositions_ = new List<int>(ChainPositions_);
			result.SurroundPositions_ = new List<int>(SurroundPositions_);
			result.EmptyBaseCreated_ = EmptyBaseCreated_;
			result.RedCaptureCount = RedCaptureCount;
			result.BlueCaptureCount = BlueCaptureCount;
			result.OldRedCaptureCount = OldRedCaptureCount;
			result.OldBlueCaptureCount = OldBlueCaptureCount;
			result.RedSquare = RedSquare;
			result.BlueSquare = BlueSquare;
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
			result.ChainDotsPositions_ = new List<DotPosition>(ChainDotsPositions_);
			result.SurroundDotsPositions_ = new List<DotPosition>(SurroundDotsPositions_);
			result.Dots_ = (Dot[])Dots_.Clone();
			var newDotsSequanceStates = new List<State>(DotsSequanceStates_.Capacity);
			DotsSequanceStates_.ForEach(state => newDotsSequanceStates.Add(state.Clone()));
			result.DotsSequanceStates_ = newDotsSequanceStates;
			result.InputChainDots_ = new List<int>(InputSurroundDotsCount);
			result.InputSurroundedDots_ = new List<int>(InputSurroundDotsCount);
			result.Width = Width;
			result.Height = Height;
			result.SurroundCondition = SurroundCondition;
			return result;
		}

		#region Methods for tests

		public bool IsEmpty
		{
			get
			{
				for (int i = 1; i <= Width; i++)
					for (int j = 1; j <= Height; j++)
						if (Dots_[GetPosition(i, j)] != Dot.Empty)
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
						if (Dots_[GetPosition(i, j)] != Dot.Empty)
							result.Add(new DotPosition(GetPosition(i, j), Dots_[GetPosition(i, j)]));
				return result;
			}
		}

		#endregion
	}
}
