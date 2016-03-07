using System;
using System.Collections.Generic;
using System.Linq;
using DotsGame;
using DotsGame.AI;

namespace DotsGame.Shell
{
	public partial class GameField
	{
		#region Properties

		public Field Field;

		public GameBot Bot;

		public string PlayerNameRed
		{
			get;
			protected set;
		}

		public string PlayerNameBlack
		{
			get;
			protected set;
		}

		public enmBeginPattern BeginPattern;

		MovesTree MovesTree;

		public List<int> InitSequance;

		#endregion

		#region Constructors

		public GameField()
		{
			MovesTree = new MovesTree();
			this.BeginPattern = enmBeginPattern.Crosswise;
			PlaceBeginPattern();
		}

		public GameField(Field field, enmBeginPattern beginPattern)
		{
			Field = field;
			MovesTree = new MovesTree();
			BeginPattern = beginPattern;
		}

		#endregion

		#region Public

		public bool MakeMove(int X, int Y)
		{
			if (Field.MakeMove(X, Y))
			{
				MovesTree.Add(X, Y);
				OnMove(new MoveEventArgs(enmMoveState.Add, Field.CurrentPlayer.NextPlayer(),
					(short)Field.LastPosition, Field.ChainPositions, Field.SurroundPositions));

				return true;
			}
			else
				return false;
		}

		public bool UnmakeMove()
		{
			if (Field.UnmakeMove())
			{
				OnMove(new MoveEventArgs(enmMoveState.Remove, Field.CurrentPlayer.NextPlayer(),
					(short)Field.LastPosition, Field.ChainPositions, Field.SurroundPositions));
				return true;
			}
			else
				return false;
		}

		public void SetMoveNumber(int MoveNumber)
		{
			if (MoveNumber < Field.States.Count())
				RollbackMoves(Field.States.Count() - MoveNumber);
			else
				if (MoveNumber > Field.States.Count())
					NextMoves(MoveNumber - Field.States.Count());
		}

		public void RollbackMoves(int Count)
		{
			for (int i = 0; i < Count; i++)
				UnmakeMove();
		}

		public void NextMoves(int Count)
		{
			for (int i = 0; i < Count; i++)
			{
				int pos = InitSequance[Field.States.Count()];
				int x, y;
				Field.GetPosition(pos, out x, out y);
				MakeMove(x, y);
			}
		}

		#endregion

		public MoveEventHandler Move;

		private void OnMove(MoveEventArgs e)
		{
			if (Move != null)
				Move(this, e);
		}

		private void PlaceBeginPattern()
		{
			int centerX, centerY;
			switch (BeginPattern)
			{
				case (enmBeginPattern.Crosswise):
					centerX = Field.Width / 2 - 1;
					centerY = Field.Height / 2 - 1;
					MakeMove(centerX, centerY);
					MakeMove(centerX + 1, centerY);
					MakeMove(centerX + 1, centerY);
					MakeMove(centerX, centerY + 1);
					break;
			}
		}
	}
}
