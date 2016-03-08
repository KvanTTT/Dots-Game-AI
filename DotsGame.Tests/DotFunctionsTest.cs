using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using DotsGame;
using System.IO;
using System.Reflection;

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

                    Assert.IsTrue(field[pos].IsRedPutted());
                    Assert.IsTrue(field[pos + 1].IsRedPutted());
                    Assert.IsTrue(field[pos + 2].IsRedPutted());
                    Assert.IsTrue(field[pos + 3].IsRedPutted());
                }
                else if (field.DotsSequenceCount == 58)
                {
                    int pos = Field.GetPosition(13, 13);

                    Assert.IsTrue(field[pos].IsBluePutted());
                    Assert.IsTrue(field[pos + 1].IsBluePutted());
                    Assert.IsTrue(field[pos + 2].IsBluePutted());
                    Assert.IsTrue(field[pos + 3].IsBluePutted());
                    Assert.IsTrue(field[Field.GetPosition(13 - 3, 13 - 1)].IsBluePutted());
                    Assert.IsTrue(field[Field.GetPosition(13 + 5, 13)].IsBluePutted());

                    Assert.IsFalse(field[pos].IsRedPutted());
                    Assert.IsFalse(field[pos + 1].IsRedPutted());
                    Assert.IsFalse(field[pos + 2].IsRedPutted());
                    Assert.IsFalse(field[pos + 3].IsRedPutted());
                }
            }

            Assert.AreEqual(0, field.RedCaptureCount);
            Assert.AreEqual(16, field.BlueCaptureCount);

            field.UnmakeAllMoves();

            Assert.AreEqual(0, field.RedCaptureCount);
            Assert.AreEqual(0, field.BlueCaptureCount);
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