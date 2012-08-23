using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotsGame
{
	public static class DotConstants
	{
		public const byte RealPlayerShift = 6;
		public const byte RealPuttedShift = RealPlayerShift + 1;
	}

	/// <summary>
	/// Representation of Dot state on Field.
	/// Player and Putted is general flags.
	/// Additional flags are required for internal using.
	/// All possibles external states:
	/// 00 - empty item.
	/// 01 - invalid (field border).
	/// 10 - red dot is putted.
	/// 11 - blue dot is putted.
	/// </summary>
	[Flags]
	public enum Dot : int
	{
		Player = 1 << 0,
		Putted = 1 << 1,

		Bound = 1 << 2,
		EmptyBound = 1 << 3,
		EmptyBase = 1 << 4,
		Tagged = 1 << 5,
		RealPlayer = 1 << DotConstants.RealPlayerShift,
		RealPutted = 1 << DotConstants.RealPuttedShift,
		
		RedPlayer = 0,
		BluePlayer = 1,
		RedRealPlayer = 0 << DotConstants.RealPlayerShift,
		BlueRealPlayer = 1 << DotConstants.RealPlayerShift,
		Empty = 0,
		Invalid = 1,

		EnableMask = Dot.Putted | Dot.Player,
		BoundMask = Dot.Bound | Dot.Putted | Dot.Player,
		AllowingMask = Dot.Putted,
		SurroundCountMask = (1 << 16) | (1 << 17), // 2 bits are enough for base depth saving.
		FirstSurroundLevel = 0x00010000
	}
}
