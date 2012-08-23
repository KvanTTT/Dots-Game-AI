using System;
using System.Collections.Generic;
using System.Linq;
using DotsGame;

namespace DotsGame.Shell
{
	public class MoveEventArgs : EventArgs
	{
		public enmMoveState Action;
		public Dot PlayerColor;
		public int Pos;
		public IEnumerable<int> ChainPoses;
		public IEnumerable<int> SurPoses;

		public MoveEventArgs(enmMoveState Action, Dot PlayerColor, int Pos, IEnumerable<int> chainPositions, IEnumerable<int> surroundPositions)
		{
			this.Action = Action;
			this.PlayerColor = PlayerColor;
			this.Pos = Pos;
			this.ChainPoses = chainPositions;
			this.SurPoses = surroundPositions;
		}
	}
}
