using System;
using System.Collections.Generic;
using System.Linq;
using DotsGame;

namespace DotsGame.Shell
{
	public class MoveEventArgs : EventArgs
	{
		public enmMoveState Action;
		public DotState PlayerColor;
		public short Pos;
		public IEnumerable<short> ChainPoses;
		public IEnumerable<short> SurPoses;

		public MoveEventArgs(enmMoveState Action, DotState PlayerColor, short Pos, IEnumerable<short> chainPositions, IEnumerable<short> surroundPositions)
		{
			this.Action = Action;
			this.PlayerColor = PlayerColor;
			this.Pos = Pos;
			this.ChainPoses = chainPositions;
			this.SurPoses = surroundPositions;
		}
	}
}
