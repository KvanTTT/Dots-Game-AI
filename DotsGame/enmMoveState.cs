using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotsGame
{
	public enum enmMoveState
	{
		/// <summary>
		/// Dot has been putted (MakeMove).
		/// </summary>
		Add,

		/// <summary>
		/// Dot has been removed (UnmakeMove).
		/// </summary>
		Remove,

		/// <summary>
		/// Start has not been changed.
		/// </summary>
		None,
	}
}
