using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace DotsGame
{
	public static class Helper
	{
		public static int[] NextPosOffsets_;
		public static int[] NextFirstPosOffsets_;

		public static Dot GetPlayer(this Dot dot)
		{
			return dot & Dot.Player;
		}

		public static bool IsPutted(this Dot dot)
		{
			return (dot & Dot.Putted) == Dot.Putted;
		}

		public static bool IsPuttingAllowed(this Dot dot)
		{
			return (dot & Dot.AllowingMask) == 0;
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

		public static void ClearTag(this Dot dot)
		{
			dot &= ~Dot.Tagged;
		}

		public static Dot GetEnabledCondition(this Dot dot)
		{
			return dot & Dot.EnableMask;
		}

		static Helper()
		{
			NextPosOffsets_ = new int[Field.RealWidth * 2 + 3];
			NextPosOffsets_[0] = -Field.RealWidth;
			NextPosOffsets_[1] = -Field.RealWidth + 1;
			NextPosOffsets_[2] = +1;
			NextPosOffsets_[2 + Field.RealWidth] = +Field.RealWidth + 1;
			NextPosOffsets_[2 + Field.RealWidth * 2] = +Field.RealWidth;
			NextPosOffsets_[1 + Field.RealWidth * 2] = +Field.RealWidth - 1;
			NextPosOffsets_[0 + Field.RealWidth * 2] = -1;
			NextPosOffsets_[0 + Field.RealWidth] = -Field.RealWidth - 1;
			
			NextFirstPosOffsets_ = new int[Field.RealWidth * 2 + 3];
			NextFirstPosOffsets_[0] = -Field.RealWidth + 1;
			NextFirstPosOffsets_[1] = +Field.RealWidth + 1;
			NextFirstPosOffsets_[2] = +Field.RealWidth + 1;
			NextFirstPosOffsets_[2 + Field.RealWidth] = +Field.RealWidth - 1;
			NextFirstPosOffsets_[2 + Field.RealWidth * 2] = +Field.RealWidth - 1;
			NextFirstPosOffsets_[1 + Field.RealWidth * 2] = -Field.RealWidth - 1;
			NextFirstPosOffsets_[0 + Field.RealWidth * 2] = -Field.RealWidth - 1;
			NextFirstPosOffsets_[0 + Field.RealWidth] = -Field.RealWidth + 1;
		}
	}
}
