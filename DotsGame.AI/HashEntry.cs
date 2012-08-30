using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace DotsGame.AI
{
	public enum enmHashEntryType : byte
	{
		Empty = 0,
		Exact = 1,
		Alpha = 2,
		Beta = 3
	}

	public class HashEntryConstants
	{
		public const byte DepthShift = 0;
		public const byte TypeShift = 8;
		public const byte BestMoveShift = 10;
		public const byte ScoreShift = 32;
	}

	[Flags]
	public enum HashEntryData : ulong
	{
		DepthMask =    0xFF,
		TypeMask =     0x3 << HashEntryConstants.TypeShift,
		BestMoveMask = 0xFFF << HashEntryConstants.BestMoveShift,
		ScoreMask =    0xFFFFFFFF00000000,
		// 10 bit still free.

		EmptyType = 0 << HashEntryConstants.TypeShift,
		ExactType = 1 << HashEntryConstants.TypeShift,
		AlphaType = 2 << HashEntryConstants.TypeShift,
		BetaType =  3 << HashEntryConstants.TypeShift,
	}

	public struct HashEntry
	{
		public ulong Data;
		public ulong HashKey;

		public byte GetDepth()
		{
			return (byte)(Data & (ulong)HashEntryData.DepthMask);
		}

		public HashEntryData GetMoveType()
		{
			return (HashEntryData)Data & HashEntryData.TypeMask;
		}

		public ushort GetBestMove()
		{
			return (ushort)((Data & (ulong)HashEntryData.BestMoveMask) >> HashEntryConstants.BestMoveShift);
		}

		public unsafe float GetScore()
		{
			int scoreBitwise = (int)(Data >> HashEntryConstants.ScoreShift);
			return *(float*)&scoreBitwise;
		}

		public static unsafe ulong PackData(ushort bestMove, float score, byte depth, HashEntryData type = HashEntryData.EmptyType)
		{
			return depth |
				(ulong)type |
				((ulong)bestMove << HashEntryConstants.BestMoveShift) |
				((ulong)(*(int*)&score) << HashEntryConstants.ScoreShift);
		}

		public override string ToString()
		{
			return string.Format("depth:{0},type:{1},bestMove:{2},score:{3:0.0000}", GetDepth(), GetMoveType(), GetBestMove(), GetScore());
		}
	}
}
