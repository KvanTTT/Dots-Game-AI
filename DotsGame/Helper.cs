using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace DotsGame
{
	public static class Helper
	{
		public static int[] NextPosOffsets;
		public static int[] NextFirstPosOffsets;
		public static int[] NextFirstPosOffsetsCCW;

		public static Dot GetPlayer(this Dot dot)
		{
			return dot & Dot.Player;
		}

		public static bool IsPutted(this Dot dot)
		{
			return (dot & Dot.Putted) == Dot.Putted;
		}

		public static bool IsNotPutted(this Dot dot)
		{
			return (dot & Dot.EnableMask) == Dot.Empty;
		}

		public static bool IsPuttingAllowed(this Dot dot)
		{
			return (dot & Dot.EnableMask) == Dot.Empty;
		}

		public static Dot NextPlayer(this Dot player)
		{
			return (player & Dot.Player) == Dot.RedPlayer ? Dot.BluePlayer : Dot.RedPlayer;
		}

		public static bool IsPlayerPutted(this Dot dot, Dot player)
		{
			return (dot & Dot.EnableMask) == (Dot.Putted | player);
		}

		public static bool IsRedPutted(this Dot dot)
		{
			return (dot & Dot.EnableMask) == (Dot.Putted | Dot.RedPlayer);
		}

		public static bool IsBluePutted(this Dot dot)
		{
			return (dot & Dot.EnableMask) == (Dot.Putted | Dot.BluePlayer);
		}

		public static bool IsZeroSurroundLevel(this Dot dot)
		{
			return (dot & Dot.SurroundCountMask) == (Dot)0;
		}

		public static bool IsOneSurroundLevel(this Dot dot)
		{
			return (dot & Dot.SurroundCountMask) == Dot.FirstSurroundLevel;
		}

		public static bool IsMoreThanOneSurroundLevel(this Dot dot)
		{
			return (dot & Dot.SurroundCountMask) > Dot.FirstSurroundLevel;
		}

		public static bool IsSurrounded(this Dot dot)
		{
			return (dot & Dot.SurroundCountMask) >= Dot.FirstSurroundLevel;
		}

		public static bool IsRealPutted(this Dot dot)
		{
			return (dot & Dot.RealPutted) == Dot.RealPutted;
		}

		public static bool IsRealRedPlayer(this Dot dot)
		{
			return (dot & Dot.RealPlayer) == Dot.RedRealPlayer;
		}

		public static bool IsRealBluePlayer(this Dot dot)
		{
			return (dot & Dot.RealPlayer) == Dot.BlueRealPlayer;
		}

		public static bool IsEnable(this Dot dot, Dot enableCondition)
		{
			return (dot & Dot.EnableMask) == enableCondition;
		}

		public static bool IsBound(this Dot dot, Dot boundCond)
		{
			return (dot & Dot.BoundMask) == boundCond;
		}

		public static bool IsTagged(this Dot dot)
		{
			return (dot & Dot.Tagged) == Dot.Tagged;
		}

		public static bool IsInEmptyBase(this Dot dot)
		{
			return (dot & Dot.EmptyBase) == Dot.EmptyBase;
		}

		public static void ClearTag(this Dot dot)
		{
			dot &= ~Dot.Tagged;
		}

		public static Dot GetEnabledCondition(this Dot dot)
		{
			return dot & Dot.EnableMask;
		}

		public static Dot GetDiagGroupNumber(this Dot dot)
		{
			return dot & Dot.DiagonalGroupMask;
		}

		public static Dot SetDiagonalGroupNumber(this Dot dot, int groupNumber)
		{
			return (Dot)((dot & ~Dot.DiagonalGroupMask) | (Dot)groupNumber);
		}

		public static Dot ClearGroupNumber(this Dot dot)
		{
			return (Dot)(dot & ~Dot.DiagonalGroupMask);
		}

		static Helper()
		{
			NextPosOffsets = new int[Field.RealWidth * 2 + 3];
			NextPosOffsets[0] = -Field.RealWidth;
			NextPosOffsets[1] = -Field.RealWidth + 1;
			NextPosOffsets[2] = +1;
			NextPosOffsets[2 + Field.RealWidth] = +Field.RealWidth + 1;
			NextPosOffsets[2 + Field.RealWidth * 2] = +Field.RealWidth;
			NextPosOffsets[1 + Field.RealWidth * 2] = +Field.RealWidth - 1;
			NextPosOffsets[0 + Field.RealWidth * 2] = -1;
			NextPosOffsets[0 + Field.RealWidth] = -Field.RealWidth - 1;
			
			NextFirstPosOffsets = new int[Field.RealWidth * 2 + 3];
			NextFirstPosOffsets[0] = -Field.RealWidth + 1;
			NextFirstPosOffsets[1] = +Field.RealWidth + 1;
			NextFirstPosOffsets[2] = +Field.RealWidth + 1;
			NextFirstPosOffsets[2 + Field.RealWidth] = +Field.RealWidth - 1;
			NextFirstPosOffsets[2 + Field.RealWidth * 2] = +Field.RealWidth - 1;
			NextFirstPosOffsets[1 + Field.RealWidth * 2] = -Field.RealWidth - 1;
			NextFirstPosOffsets[0 + Field.RealWidth * 2] = -Field.RealWidth - 1;
			NextFirstPosOffsets[0 + Field.RealWidth] = -Field.RealWidth + 1;

			NextFirstPosOffsetsCCW = new int[Field.RealWidth * 2 + 3];
			NextFirstPosOffsetsCCW[0] = +Field.RealWidth - 1;
			NextFirstPosOffsetsCCW[1] = +Field.RealWidth - 1;
			NextFirstPosOffsetsCCW[2] = -Field.RealWidth - 1;
			NextFirstPosOffsetsCCW[2 + Field.RealWidth] = -Field.RealWidth - 1;
			NextFirstPosOffsetsCCW[2 + Field.RealWidth * 2] = -Field.RealWidth + 1;
			NextFirstPosOffsetsCCW[1 + Field.RealWidth * 2] = -Field.RealWidth + 1;
			NextFirstPosOffsetsCCW[0 + Field.RealWidth * 2] = +Field.RealWidth + 1;
			NextFirstPosOffsetsCCW[0 + Field.RealWidth] = +Field.RealWidth + 1;
		}
	}
}
