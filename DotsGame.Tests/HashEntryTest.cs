using System;
using NUnit.Framework;
using DotsGame.AI;

namespace DotsGame.Tests
{
	[TestFixture]
	public class HashEntryTest
	{
		[Test]
		public void PackUnpcackDataTest()
		{
			byte depth = 255;
			HashEntryData type = HashEntryData.AlphaType;
			ushort bestMove = 1167;
			float score = 123.0342f;

			ulong data = HashEntry.PackData(bestMove, score, depth, type);
			HashEntry unpackedHashEntry = new HashEntry() { Data = data };

			Assert.AreEqual(data, unpackedHashEntry.Data);
			Assert.AreEqual(depth, unpackedHashEntry.GetDepth());
			Assert.AreEqual(type, unpackedHashEntry.GetMoveType());
			Assert.AreEqual(bestMove, unpackedHashEntry.GetBestMove());
			Assert.AreEqual(score, unpackedHashEntry.GetScore());
		}
	}
}
