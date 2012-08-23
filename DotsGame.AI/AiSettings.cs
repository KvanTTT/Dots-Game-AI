using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotsGame.AI
{
	public static class AiSettings
	{
		public static float InfinityScore
		{
			get;
			private set;
		}

		public static float MaxPly
		{
			get;
			private set;
		}

		public static float SquareCoef
		{
			get;
			private set;
		}

		public static ulong HashTableSize
		{
			get;
			private set;
		}

		static AiSettings()
		{
			InfinityScore = float.MaxValue - 100;
			MaxPly = 50;
			SquareCoef = 0.005f;
			HashTableSize = 1 << 20;
		}
	}
}
