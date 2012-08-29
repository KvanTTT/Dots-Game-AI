using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using DotsGame.AI;

namespace DotsGame.Tests
{
	[TestFixture]
	public class ForcedAttackTests
	{
		[Test]
		public void FindCapturesMovesDepthOneTest()
		{
			int startX = 16;
			int startY = 16;
			var field = new FieldWithGroups(39, 32);

			field.MakeMove(startX, startY);
			field.MakeMove(startX - 1, startY);

			field.MakeMove(startX + 1, startY + 1);
			field.MakeMove(startX, startY + 1);

			field.MakeMove(startX, startY + 2);
			field.MakeMove(startX - 1, startY + 2);

			var moveGenerator = new ForcedAttackMoveGenerator(field);
			var CapturesMoves = moveGenerator.FindCapturesMoves(field.CurrentPlayer, 1);

			Assert.AreEqual(1, CapturesMoves.Count);
			Assert.AreEqual(Field.GetPosition(startX - 1, startY + 1), CapturesMoves[0].CapturePositions[0]);
			Assert.AreEqual(4, CapturesMoves[0].ChainPositions.Count);
			Assert.AreEqual(Field.GetPosition(startX, startY + 1), CapturesMoves[0].SurroundedPositions[0]);
		}

		[Test]
		public void FindCapturesMovesDepthTwoLongTest()
		{
			int startX = 16;
			int startY = 16;
			var field = new FieldWithGroups(39, 32);

			field.MakeMove(startX, startY);
			field.MakeMove(startX + 1, startY);

			field.MakeMove(startX + 1, startY + 1);
			field.MakeMove(startX + 2, startY);

			field.MakeMove(startX + 2, startY + 1);

			field.MakeMove(startX + 3, startY, Dot.RedPlayer);

			var moveGenerator = new ForcedAttackMoveGenerator(field);
			var CapturesMoves = moveGenerator.FindCapturesMoves(Dot.RedPlayer, 2);

			Assert.AreEqual(1, CapturesMoves.Count);
			Assert.AreEqual(Field.GetPosition(startX + 1, startY - 1), CapturesMoves[0].CapturePositions[0]);
			Assert.AreEqual(Field.GetPosition(startX + 2, startY - 1), CapturesMoves[0].CapturePositions[1]);
			Assert.AreEqual(6, CapturesMoves[0].ChainPositions.Count);
			Assert.AreEqual(Field.GetPosition(startX + 2, startY), CapturesMoves[0].SurroundedPositions[0]);
			Assert.AreEqual(Field.GetPosition(startX + 1, startY), CapturesMoves[0].SurroundedPositions[1]);
		}

		[Test]
		public void FindCapturesMovesDepthTwoShortTest()
		{
			int startX = 16;
			int startY = 16;
			var field = new FieldWithGroups(39, 32);

			field.MakeMove(startX, startY, Dot.RedPlayer);
			field.MakeMove(startX - 1, startY + 1, Dot.RedPlayer);
			field.MakeMove(startX, startY + 2, Dot.RedPlayer);

			field.MakeMove(startX + 2, startY, Dot.RedPlayer);
			field.MakeMove(startX + 3, startY + 1, Dot.RedPlayer);
			field.MakeMove(startX + 2, startY + 2, Dot.RedPlayer);

			field.MakeMove(startX, startY + 1, Dot.BluePlayer);
			field.MakeMove(startX + 1, startY + 1, Dot.BluePlayer);
			field.MakeMove(startX + 2, startY + 1, Dot.BluePlayer);

			var moveGenerator = new ForcedAttackMoveGenerator(field);
			var CapturesMoves = moveGenerator.FindCapturesMoves(Dot.RedPlayer, 2);

			Assert.AreEqual(4, CapturesMoves.Count);
			Assert.AreEqual(8, CapturesMoves[0].ChainPositions.Count);
			Assert.AreEqual(8, CapturesMoves[0].ChainPositions.Count);
			Assert.AreEqual(8, CapturesMoves[0].ChainPositions.Count);
			Assert.AreEqual(8, CapturesMoves[0].ChainPositions.Count);
		}
	}
}
