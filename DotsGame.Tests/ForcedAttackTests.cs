using System.Linq;
using DotsGame.AI;
using NUnit.Framework;

namespace DotsGame.Tests
{
    [TestFixture]
    public class ForcedAttackTests
    {
        [Test]
        public void ForcedAttack_FindCapturesMovesDepthOne()
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
            var CapturesMoves = moveGenerator.FindCapturesMoves(field.CurrentPlayer);

            Assert.AreEqual(1, CapturesMoves.Count);
            Assert.AreEqual(Field.GetPosition(startX - 1, startY + 1), CapturesMoves[0].CapturePositions[0]);
            Assert.AreEqual(4, CapturesMoves[0].ChainPositions.Count);
            Assert.AreEqual(Field.GetPosition(startX, startY + 1), CapturesMoves[0].SurroundedPositions[0]);
        }

        [Test]
        public void ForcedAttack_FindCapturesMovesDepthTwoLong()
        {
            int startX = 16;
            int startY = 16;
            var field = new FieldWithGroups(39, 32);

            field.MakeMove(startX, startY);
            field.MakeMove(startX + 1, startY);

            field.MakeMove(startX + 1, startY + 1);
            field.MakeMove(startX + 2, startY);

            field.MakeMove(startX + 2, startY + 1);

            field.MakeMove(startX + 3, startY, 0);

            var moveGenerator = new ForcedAttackMoveGenerator(field);
            var CapturesMoves = moveGenerator.FindCapturesMoves(0, 2);

            Assert.AreEqual(1, CapturesMoves.Count);
            Assert.AreEqual(Field.GetPosition(startX + 1, startY - 1), CapturesMoves[0].CapturePositions[0]);
            Assert.AreEqual(Field.GetPosition(startX + 2, startY - 1), CapturesMoves[0].CapturePositions[1]);
            Assert.AreEqual(6, CapturesMoves[0].ChainPositions.Count);
            Assert.AreEqual(Field.GetPosition(startX + 2, startY), CapturesMoves[0].SurroundedPositions[0]);
            Assert.AreEqual(Field.GetPosition(startX + 1, startY), CapturesMoves[0].SurroundedPositions[1]);
        }

        [Test]
        public void ForcedAttack_FindCapturesMovesDepthTwoShort()
        {
            int startX = 16;
            int startY = 16;
            var field = new FieldWithGroups(39, 32);

            field.MakeMove(startX, startY, 0);
            field.MakeMove(startX - 1, startY + 1, 0);
            field.MakeMove(startX, startY + 2, 0);

            field.MakeMove(startX + 2, startY, 0);
            field.MakeMove(startX + 3, startY + 1, 0);
            field.MakeMove(startX + 2, startY + 2, 0);

            field.MakeMove(startX, startY + 1, 1);
            field.MakeMove(startX + 1, startY + 1, 1);
            field.MakeMove(startX + 2, startY + 1, 1);

            var moveGenerator = new ForcedAttackMoveGenerator(field);
            var CapturesMoves = moveGenerator.FindCapturesMoves(0, 2);

            Assert.AreEqual(4, CapturesMoves.Count);
            Assert.AreEqual(8, CapturesMoves[0].ChainPositions.Count);
            Assert.AreEqual(8, CapturesMoves[0].ChainPositions.Count);
            Assert.AreEqual(8, CapturesMoves[0].ChainPositions.Count);
            Assert.AreEqual(8, CapturesMoves[0].ChainPositions.Count);
        }

        [Test]
        public void ForcedAttack_Ladder()
        {
            int startX = 16;
            int startY = 16;
            var field = new FieldWithGroups(39, 32);

            field.MakeMove(startX, startY);
            field.MakeMove(startX + 1, startY);

            field.MakeMove(startX + 1, startY + 1);
            field.MakeMove(startX + 2, startY);

            field.MakeMove(startX + 2, startY + 1);

            field.MakeMove(startX + 3, startY, 0);

            field.MakeMove(startX + 5, startY - 4, 0);
            //field.MakeMove(startX + 3, startY - 2, Dot.RedPlayer);

            var moveGenerator = new ForcedAttackMoveGenerator(field);
            moveGenerator.GenerateMoves(0);

            var result = moveGenerator.HashEntries.Select(entry => AI.Helper.HashEntryToString(entry)).ToList();
            Assert.AreEqual(Field.GetPosition(startX + 1, startY - 1), moveGenerator.HashEntries[0].GetMove());
            Assert.AreEqual(13, moveGenerator.HashEntries[0].GetDepth());

            field.MakeMove(startX + 1, startY - 1, 0);
            field.MakeMove(startX + 2, startY - 1, 1);
            moveGenerator.GenerateMoves(0);

            result = moveGenerator.HashEntries.Select(entry => AI.Helper.HashEntryToString(entry)).ToList();
            Assert.AreEqual(Field.GetPosition(startX + 2, startY - 2), moveGenerator.HashEntries[0].GetMove());
            Assert.AreEqual(11, moveGenerator.HashEntries[0].GetDepth());
        }

        /*[Test]
		public void SvsVn91TaskTest()
		{
			var field = new FieldWithGroups(DataFolderPath + "s vs vn91.sav");
			var moveGenerator = new ForcedAttackMoveGenerator(field);

			moveGenerator.GenerateMoves(DotState.RedPlayer);
			Assert.AreEqual(976, moveGenerator.HashEntries[0].GetMove());

			field.MakeMove(976);
			field.MakeMove(911);

			moveGenerator.GenerateMoves(DotState.RedPlayer);
			Assert.AreEqual(849, moveGenerator.HashEntries[0].GetMove());

			field.MakeMove(849);
			field.MakeMove(913);

			moveGenerator.GenerateMoves(DotState.RedPlayer);
			Assert.AreEqual(914, moveGenerator.HashEntries[0].GetMove());

			field.MakeMove(914);
			field.MakeMove(977);

			moveGenerator.GenerateMoves(DotState.RedPlayer);
			Assert.AreEqual(978, moveGenerator.HashEntries[0].GetMove());

			field.MakeMove(978);
			field.MakeMove(1041);

			moveGenerator.GenerateMoves(DotState.RedPlayer);

			var result = moveGenerator.HashEntries.Select(entry => DotsGame.AI.Helper.HashEntryToString(entry)).ToList();
		}

		[Test]
		public void TochkiOrgProf02()
		{
			var field = new FieldWithGroups(DataFolderPath + "tochki-org pro-02.sav");
			var moveGenerator = new ForcedAttackMoveGenerator(field);

			moveGenerator.GenerateMoves(DotState.RedPlayer);
			Assert.AreEqual(1106, moveGenerator.HashEntries[0].GetMove());

			field.MakeMove(1106);
			field.MakeMove(1043);

			moveGenerator.GenerateMoves(DotState.RedPlayer);
			Assert.AreEqual(1171, moveGenerator.HashEntries[0].GetMove());

			field.MakeMove(1171);
			field.MakeMove(1108);

			moveGenerator.GenerateMoves(DotState.RedPlayer);
			Assert.AreEqual(1236, moveGenerator.HashEntries[0].GetMove());

			field.MakeMove(1236);
			field.MakeMove(1109);

			moveGenerator.GenerateMoves(DotState.RedPlayer);
			Assert.AreEqual(1110, moveGenerator.HashEntries[0].GetMove());

			var result = moveGenerator.HashEntries.Select(entry => DotsGame.AI.Helper.HashEntryToString(entry)).ToList();
		}

		[Test]
		public void TochkiOrgProf16()
		{
			var field = new FieldWithGroups(DataFolderPath + "tochki-org pro-16.sav");
			var moveGenerator = new ForcedAttackMoveGenerator(field);

			moveGenerator.GenerateMoves(DotState.RedPlayer);
			Assert.AreEqual(1299, moveGenerator.HashEntries[0].GetMove());

			field.MakeMove(1299);

			var result = moveGenerator.HashEntries.Select(entry => DotsGame.AI.Helper.HashEntryToString(entry)).ToList();
		}*/
    }
}
