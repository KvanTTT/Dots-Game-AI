using System;
using System.Diagnostics;
using NUnit.Framework;
using DotsGame;
using DotsGame.AI;

namespace DotsGame.Tests
{
	[TestFixture]
	public class AlphaBetaHashAlgoritmTest
	{
		[Test]
		public void AlphaBetaUsualAndHashPerformanceTest()
		{
			int startX = 16;
			int startY = 16;
			var field = new Field(39, 32);

			field.MakeMove(startX, startY);
			field.MakeMove(startX + 1, startY);

			field.MakeMove(startX + 1, startY + 1);
			field.MakeMove(startX, startY + 1);

			field.MakeMove(startX + 1, startY - 1);
			field.MakeMove(startX, startY - 1);

			int expectedDestMove = Field.GetPosition(startX + 2, startY);

			var stopwatch = new Stopwatch();
			byte depth = 6;

			var alphaBetaAlgoritm = new AlphaBetaAlgoritm(field);
			stopwatch.Start();
			int alphaBetaBestMove = alphaBetaAlgoritm.SearchBestMove(depth, Dot.RedPlayer, -AiSettings.InfinityScore, AiSettings.InfinityScore);
			stopwatch.Stop();
			TimeSpan alphaBetaElapsed = stopwatch.Elapsed;
			stopwatch.Reset();
			
			var alphaBetaHashAlgoritm = new AlphaBetaHashAlgoritm(field);
			stopwatch.Start();
			int alphaBetaHashBestMove = alphaBetaHashAlgoritm.SearchBestMove((byte)depth, Dot.RedPlayer, -AiSettings.InfinityScore, AiSettings.InfinityScore);
			stopwatch.Stop();
			TimeSpan alphaBetaHashElapsed = stopwatch.Elapsed;

			Assert.AreEqual(alphaBetaBestMove, alphaBetaHashBestMove);
			//if (depth > 2)
			//	Assert.IsTrue(alphaBetaHashElapsed < alphaBetaElapsed);

			#if DEBUG
				Console.WriteLine("Configuration: Debug");
			#else
				Console.WriteLine("Configuration: Release");
			#endif
			Console.WriteLine("Depth: {0}", depth);
			Console.WriteLine("Usual AlphaBeta time elapsed: {0}", alphaBetaElapsed);
			Console.WriteLine("Hash AlphaBeta time elapsed: {0}", alphaBetaHashElapsed);
			Console.WriteLine("Ratio: {0}", (double)alphaBetaHashElapsed.Ticks / (double)alphaBetaElapsed.Ticks);
		}
	}
}
