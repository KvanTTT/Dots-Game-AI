namespace DotsGame.AI
{
    public static class Helper
	{
		public static ushort GetMove(this ulong data)
		{
			return (ushort)((data & (ulong)HashEntryData.BestMoveMask) >> HashEntryConstants.BestMoveShift);
		}

		public static byte GetDepth(this ulong data)
		{
			return (byte)(data & (ulong)HashEntryData.DepthMask);
		}

		public static HashEntryData GetMoveType(this ulong data)
		{
			return (HashEntryData)data & HashEntryData.TypeMask;
		}

		public static ushort GetBestMove(this ulong data)
		{
			return (ushort)((data & (ulong)HashEntryData.BestMoveMask) >> HashEntryConstants.BestMoveShift);
		}

		public static unsafe float GetScore(this ulong data)
		{
			int scoreBitwise = (int)(data >> HashEntryConstants.ScoreShift);
			return *(float*)&scoreBitwise;
		}

		public static string HashEntryToString(ulong data)
		{
			return string.Format("depth:{0},type:{1},bestMove:{2},score:{3:0.0000}", data.GetDepth(), data.GetMoveType(), data.GetBestMove(), data.GetScore());
		}
	}
}
