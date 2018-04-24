using NUnit.Framework;

namespace DotsGame.Tests
{
    /// <summary>
    /// Summary description for FieldFunctionsTest
    /// </summary>
    [TestFixture]
    public class DotFunctionsTest
    {
        [Test]
        public void IsRedDotTest()
        {
            GameMove[] moves = TestUtils.LoadMovesFromPointsXt("DotFunctionsTest.sav");
            Field field = new Field(39, 32);

            foreach (var move in moves)
            {
                field.MakeMove(move.Column, move.Row);
                if (field.DotsSequenceCount == 25)
                {
                    int pos = Field.GetPosition(13, 13);

                    Assert.IsTrue(field[pos].IsPlayer0Putted());
                    Assert.IsTrue(field[pos + 1].IsPlayer0Putted());
                    Assert.IsTrue(field[pos + 2].IsPlayer0Putted());
                    Assert.IsTrue(field[pos + 3].IsPlayer0Putted());
                }
                else if (field.DotsSequenceCount == 58)
                {
                    int pos = Field.GetPosition(13, 13);

                    Assert.IsTrue(field[pos].IsPlayer1Putted());
                    Assert.IsTrue(field[pos + 1].IsPlayer1Putted());
                    Assert.IsTrue(field[pos + 2].IsPlayer1Putted());
                    Assert.IsTrue(field[pos + 3].IsPlayer1Putted());
                    Assert.IsTrue(field[Field.GetPosition(13 - 3, 13 - 1)].IsPlayer1Putted());
                    Assert.IsTrue(field[Field.GetPosition(13 + 5, 13)].IsPlayer1Putted());

                    Assert.IsFalse(field[pos].IsPlayer0Putted());
                    Assert.IsFalse(field[pos + 1].IsPlayer0Putted());
                    Assert.IsFalse(field[pos + 2].IsPlayer0Putted());
                    Assert.IsFalse(field[pos + 3].IsPlayer0Putted());
                }
            }

            Assert.AreEqual(0, field.Player0CaptureCount);
            Assert.AreEqual(16, field.Player1CaptureCount);

            field.UnmakeAllMoves();

            Assert.AreEqual(0, field.Player0CaptureCount);
            Assert.AreEqual(0, field.Player1CaptureCount);
            Assert.IsTrue(field.IsEmpty);
        }

        [Test]
        public void GetNextPosTest()
        {
            int startX = 16;
            int startY = 16;

            int centerPos = Field.GetPosition(startX, startY);
            int pos = centerPos - Field.RealWidth - 1;

            Field.GetNextPos(centerPos, ref pos);
            Assert.AreEqual(centerPos - Field.RealWidth, pos);

            Field.GetNextPos(centerPos, ref pos);
            Assert.AreEqual(centerPos - Field.RealWidth + 1, pos);

            Field.GetNextPos(centerPos, ref pos);
            Assert.AreEqual(centerPos + 1, pos);

            Field.GetNextPos(centerPos, ref pos);
            Assert.AreEqual(centerPos + Field.RealWidth + 1, pos);

            Field.GetNextPos(centerPos, ref pos);
            Assert.AreEqual(centerPos + Field.RealWidth, pos);

            Field.GetNextPos(centerPos, ref pos);
            Assert.AreEqual(centerPos + Field.RealWidth - 1, pos);

            Field.GetNextPos(centerPos, ref pos);
            Assert.AreEqual(centerPos - 1, pos);

            Field.GetNextPos(centerPos, ref pos);
            Assert.AreEqual(centerPos - Field.RealWidth - 1, pos);
        }

        [Test]
        public void GetFirstNextPosTest()
        {
            int startX = 16;
            int startY = 16;

            int centerPos = Field.GetPosition(startX, startY);
            int pos = centerPos - Field.RealWidth - 1;
            int firstNextPos;

            Field.GetNextPos(centerPos, ref pos);
            firstNextPos = pos;
            Field.GetFirstNextPos(centerPos, ref firstNextPos);
            Assert.AreEqual(centerPos + Field.RealWidth + 1, firstNextPos);

            Field.GetNextPos(centerPos, ref pos);
            firstNextPos = pos;
            Field.GetFirstNextPos(centerPos, ref firstNextPos);
            Assert.AreEqual(centerPos + Field.RealWidth + 1, firstNextPos);

            Field.GetNextPos(centerPos, ref pos);
            firstNextPos = pos;
            Field.GetFirstNextPos(centerPos, ref firstNextPos);
            Assert.AreEqual(centerPos + Field.RealWidth - 1, firstNextPos);

            Field.GetNextPos(centerPos, ref pos);
            firstNextPos = pos;
            Field.GetFirstNextPos(centerPos, ref firstNextPos);
            Assert.AreEqual(centerPos + Field.RealWidth - 1, firstNextPos);

            Field.GetNextPos(centerPos, ref pos);
            firstNextPos = pos;
            Field.GetFirstNextPos(centerPos, ref firstNextPos);
            Assert.AreEqual(centerPos - Field.RealWidth - 1, firstNextPos);

            Field.GetNextPos(centerPos, ref pos);
            firstNextPos = pos;
            Field.GetFirstNextPos(centerPos, ref firstNextPos);
            Assert.AreEqual(centerPos - Field.RealWidth - 1, firstNextPos);

            Field.GetNextPos(centerPos, ref pos);
            firstNextPos = pos;
            Field.GetFirstNextPos(centerPos, ref firstNextPos);
            Assert.AreEqual(centerPos - Field.RealWidth + 1, firstNextPos);

            Field.GetNextPos(centerPos, ref pos);
            firstNextPos = pos;
            Field.GetFirstNextPos(centerPos, ref firstNextPos);
            Assert.AreEqual(centerPos - Field.RealWidth + 1, firstNextPos);
        }

        [Test]
        public void GetFirstNextPosCCWTest()
        {
            int startX = 16;
            int startY = 16;

            int centerPos = Field.GetPosition(startX, startY);
            int pos = centerPos - Field.RealWidth - 1;
            int firstNextPos;

            Field.GetNextPos(centerPos, ref pos);
            firstNextPos = pos;
            Field.GetFirstNextPosCCW(centerPos, ref firstNextPos);
            Assert.AreEqual(centerPos + Field.RealWidth - 1, firstNextPos);

            Field.GetNextPos(centerPos, ref pos);
            firstNextPos = pos;
            Field.GetFirstNextPosCCW(centerPos, ref firstNextPos);
            Assert.AreEqual(centerPos - Field.RealWidth - 1, firstNextPos);

            Field.GetNextPos(centerPos, ref pos);
            firstNextPos = pos;
            Field.GetFirstNextPosCCW(centerPos, ref firstNextPos);
            Assert.AreEqual(centerPos - Field.RealWidth - 1, firstNextPos);

            Field.GetNextPos(centerPos, ref pos);
            firstNextPos = pos;
            Field.GetFirstNextPosCCW(centerPos, ref firstNextPos);
            Assert.AreEqual(centerPos - Field.RealWidth + 1, firstNextPos);

            Field.GetNextPos(centerPos, ref pos);
            firstNextPos = pos;
            Field.GetFirstNextPosCCW(centerPos, ref firstNextPos);
            Assert.AreEqual(centerPos - Field.RealWidth + 1, firstNextPos);

            Field.GetNextPos(centerPos, ref pos);
            firstNextPos = pos;
            Field.GetFirstNextPosCCW(centerPos, ref firstNextPos);
            Assert.AreEqual(centerPos + Field.RealWidth + 1, firstNextPos);

            Field.GetNextPos(centerPos, ref pos);
            firstNextPos = pos;
            Field.GetFirstNextPosCCW(centerPos, ref firstNextPos);
            Assert.AreEqual(centerPos + Field.RealWidth + 1, firstNextPos);

            Field.GetNextPos(centerPos, ref pos);
            firstNextPos = pos;
            Field.GetFirstNextPosCCW(centerPos, ref firstNextPos);
            Assert.AreEqual(centerPos + Field.RealWidth - 1, firstNextPos);
        }
    }
}