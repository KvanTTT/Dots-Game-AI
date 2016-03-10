using System;
using System.IO;
using NUnit.Framework;
using DotsGame;
using System.Reflection;

namespace DotsGame.Tests
{
    [TestFixture]
    public class FieldTests
    {
        #region Tests

        [Test]
        public virtual void Play_SimpleSequence()
        {
            SimpleSequenceTest(new Field(39, 32));
        }

        [Test]
        public virtual void Play_BlueSurroundFirst()
        {
            BlueSurroundFirstTest(new Field(39, 32));
        }

        [Test]
        public virtual void Play_OneBase()
        {
            OneBaseTest(new Field(39, 32));
        }

        [Test]
        public virtual void Play_BaseInBase()
        {
            BaseInBaseTest(new Field(39, 32));
        }

        [Test]
        public virtual void Play_ComplexBaseInBase()
        {
            ComplexBaseInBaseTest(new Field(39, 32));
        }

        [Test]
        public virtual void Play_EmptyBase()
        {
            EmptyBaseTest(new Field(39, 32));
        }

        [Test]
        public virtual void Play_BlueSurroundFirstInEmptyBase()
        {
            BlueSurroundFirstInEmptyBase(new Field(39, 32));
        }

        [Test]
        public virtual void Play_ThreeAdjacentBases()
        {
            ThreeAdjacentBasesTest(new Field(39, 32));
        }

        [Test]
        public virtual void Play_BigEmptyBase()
        {
            BigEmptyBaseTest(new Field(39, 32));
        }

        [Test]
        public virtual void Play_UCTBagEmptyBase()
        {
            UCTBagEmptyBaseTest(new Field(39, 32));
        }

        [Test]
        public virtual void Play_VerylongGame()
        {
            VerylongGameTest(new Field(39, 32));
        }

        [Test]
        public void Play_SurrondBaseAndEmptyBase()
        {
            var field = new Field(39, 32);
            int startX = 16;
            int startY = 16;

            field.MakeMove(startX + 2, startY, 0);
            field.MakeMove(startX + 1, startY + 1, 0);
            field.MakeMove(startX + 3, startY + 1, 0);
            field.MakeMove(startX, startY + 2, 0);
            field.MakeMove(startX + 1, startY + 3, 0);

            field.MakeMove(startX + 1, startY + 2, 1);
            field.MakeMove(startX + 2, startY + 2, 0);
            Assert.IsTrue(field.MakeMove(startX + 2, startY + 1, 1));
            Assert.AreEqual(2, field.Player0CaptureCount);

            field.UnmakeMove();
            field.UnmakeMove();
            field.UnmakeMove();

            field.MakeMove(startX + 2, startY + 1, 1);
            field.MakeMove(startX + 2, startY + 2, 0);
            Assert.IsTrue(field.MakeMove(startX + 1, startY + 2, 1));
            Assert.AreEqual(2, field.Player0CaptureCount);
        }

        #endregion

        #region Helpers

        protected void SimpleSequenceTest(Field field)
        {
            int startX = 16;
            int startY = 16;

            field.MakeMove(startX, startY);
            field.MakeMove(startX + 1, startY);
            field.MakeMove(startX + 1, startY + 1);
            field.MakeMove(startX, startY + 1);
            field.UnmakeAllMoves();
            Assert.IsTrue(field.IsEmpty);
            Assert.AreEqual(0, field.Player0CaptureCount + field.Player1CaptureCount);
        }

        protected void BlueSurroundFirstTest(Field field)
        {
            int startX = 16;
            int startY = 16;

            field.MakeMove(startX, startY);
            field.MakeMove(startX - 1, startY);

            field.MakeMove(startX + 1, startY + 1);
            field.MakeMove(startX, startY + 1);

            field.MakeMove(startX + 1, startY - 1);
            field.MakeMove(startX, startY - 1);

            field.MakeMove(startX - 2, startY);

            field.MakeMove(startX + 1, startY);

            Assert.AreEqual(0, field.Player0CaptureCount);
            Assert.AreEqual(1, field.Player1CaptureCount);

            field.UnmakeAllMoves();

            Assert.IsTrue(field.IsEmpty);
            Assert.AreEqual(0, field.Player0CaptureCount + field.Player1CaptureCount);
        }

        protected void OneBaseTest(Field field)
        {
            int startX = 16;
            int startY = 16;

            field.MakeMove(startX, startY);
            field.MakeMove(startX + 1, startY);
            field.MakeMove(startX + 1, startY + 1);
            field.MakeMove(startX, startY + 1);

            field.MakeMove(startX + 2, startY);
            field.MakeMove(startX - 1, startY);
            field.MakeMove(startX + 1, startY - 1);

            Assert.AreEqual(1, field.Player0CaptureCount);

            field.UnmakeAllMoves();

            Assert.IsTrue(field.IsEmpty);
            Assert.AreEqual(0, field.Player0CaptureCount + field.Player1CaptureCount);
        }

        protected void BaseInBaseTest(Field field)
        {
            int startX = 16;
            int startY = 16;

            field.MakeMove(startX, startY);
            field.MakeMove(startX + 1, startY);

            field.MakeMove(startX + 2, startY);
            field.MakeMove(startX + 2, startY + 1);

            field.MakeMove(startX + 1, startY + 1);
            field.MakeMove(startX + 3, startY);

            field.MakeMove(startX + 2, startY + 2);
            field.MakeMove(startX + 2, startY - 1);

            Assert.AreEqual(0, field.Player0CaptureCount);
            Assert.AreEqual(1, field.Player1CaptureCount);

            field.MakeMove(startX + 3, startY + 1);
            field.MakeMove(startX + 10, startY);

            field.MakeMove(startX + 4, startY);
            field.MakeMove(startX + 10, startY + 1);

            field.MakeMove(startX + 3, startY - 1);
            field.MakeMove(startX + 10, startY + 2);

            field.MakeMove(startX + 2, startY - 2);
            field.MakeMove(startX + 10, startY + 3);

            field.MakeMove(startX + 1, startY - 1);
            field.MakeMove(startX + 10, startY + 4);

            Assert.AreEqual(4, field.Player0CaptureCount);
            Assert.AreEqual(0, field.Player1CaptureCount);

            field.UnmakeAllMoves();

            Assert.IsTrue(field.IsEmpty);
            Assert.AreEqual(0, field.Player0CaptureCount + field.Player1CaptureCount);
        }

        protected void ComplexBaseInBaseTest(Field field)
        {
            GameMove[] moves = TestUtils.LoadMovesFromPointsXt("DotFunctionsTest.sav");

            foreach (var move in moves)
            {
                if (field.DotsSequenceCount == 24)
                {
                    field.MakeMove(move.Column, move.Row);
                    Assert.AreEqual(1, field.Player0CaptureCount);
                    Assert.AreEqual(0, field.Player1CaptureCount);
                }
                else
                    field.MakeMove(move.Column, move.Row);
            }

            Assert.AreEqual(0, field.Player0CaptureCount);
            Assert.AreEqual(16, field.Player1CaptureCount);

            field.UnmakeAllMoves();

            Assert.AreEqual(0, field.Player0CaptureCount);
            Assert.AreEqual(0, field.Player1CaptureCount);
            Assert.IsTrue(field.IsEmpty);
        }

        protected void EmptyBaseTest(Field field)
        {
            int startX = 16;
            int startY = 16; ;

            field.MakeMove(startX, startY);
            field.MakeMove(startX - 1, startY);

            field.MakeMove(startX + 1, startY + 1);
            field.MakeMove(startX, startY + 1);

            field.MakeMove(startX + 2, startY);
            field.MakeMove(startX - 2, startY);

            // Create empty base.
            field.MakeMove(startX + 1, startY - 1);

            // Red Surround.
            field.MakeMove(startX + 1, startY);

            Assert.AreEqual(1, field.Player0CaptureCount);
            Assert.AreEqual(0, field.Player1CaptureCount);

            field.UnmakeAllMoves();

            Assert.IsTrue(field.IsEmpty);
            Assert.AreEqual(0, field.Player0CaptureCount + field.Player1CaptureCount);
        }

        protected void BlueSurroundFirstInEmptyBase(Field field)
        {
            int startX = 16;
            int startY = 16;

            field.MakeMove(startX, startY);
            field.MakeMove(startX - 1, startY);

            field.MakeMove(startX + 1, startY + 1);
            field.MakeMove(startX, startY + 1);

            field.MakeMove(startX + 1, startY - 1);
            field.MakeMove(startX, startY - 1);

            field.MakeMove(startX + 2, startY);
            field.MakeMove(startX + 1, startY);

            Assert.AreEqual(0, field.Player0CaptureCount);
            Assert.AreEqual(1, field.Player1CaptureCount);

            field.UnmakeAllMoves();

            Assert.IsTrue(field.IsEmpty);
            Assert.AreEqual(0, field.Player0CaptureCount + field.Player1CaptureCount);
        }

        protected void ThreeAdjacentBasesTest(Field field)
        {
            int startX = 16;
            int startY = 16;

            field.MakeMove(startX, startY);
            field.MakeMove(startX + 1, startY);

            field.MakeMove(startX + 1, startY + 1);
            field.MakeMove(startX + 2, startY - 1);

            field.MakeMove(startX + 2, startY);
            field.MakeMove(startX + 1, startY - 2);

            field.MakeMove(startX + 3, startY - 1);
            field.MakeMove(startX, startY + 1);

            field.MakeMove(startX + 2, startY - 2);
            field.MakeMove(startX + 2, startY + 1);

            field.MakeMove(startX + 1, startY - 3);
            field.MakeMove(startX + 3, startY - 2);

            field.MakeMove(startX, startY - 2);
            field.MakeMove(startX, startY - 3);

            field.MakeMove(startX + 1, startY - 1);

            Assert.AreEqual(3, field.Player0CaptureCount);
            Assert.AreEqual(0, field.Player1CaptureCount);

            field.UnmakeAllMoves();

            Assert.IsTrue(field.IsEmpty);
            Assert.AreEqual(0, field.Player0CaptureCount + field.Player1CaptureCount);
        }

        protected void BigEmptyBaseTest(Field field)
        {
            int startX = 12;
            int startY = 2;

            // top chain.
            field.MakeMove(startX, startY);
            field.MakeMove(startX + 10, startY);

            field.MakeMove(startX + 1, startY);
            field.MakeMove(startX + 10, startY + 1);

            field.MakeMove(startX + 2, startY);
            field.MakeMove(startX + 10, startY + 2);

            field.MakeMove(startX + 3, startY);
            field.MakeMove(startX + 10, startY + 3);

            field.MakeMove(startX + 4, startY);
            field.MakeMove(startX + 10, startY + 4);

            // right chain.
            field.MakeMove(startX + 5, startY + 1);
            field.MakeMove(startX + 10, startY + 5);

            field.MakeMove(startX + 5, startY + 2);
            field.MakeMove(startX + 10, startY + 6);

            field.MakeMove(startX + 5, startY + 3);
            field.MakeMove(startX + 10, startY + 7);

            field.MakeMove(startX + 5, startY + 4);
            field.MakeMove(startX + 10, startY + 8);

            // bottom chain.
            field.MakeMove(startX + 4, startY + 5);
            field.MakeMove(startX + 10, startY + 9);

            field.MakeMove(startX + 3, startY + 5);
            field.MakeMove(startX + 10, startY + 10);

            field.MakeMove(startX + 2, startY + 5);
            field.MakeMove(startX + 10, startY + 11);

            field.MakeMove(startX + 1, startY + 5);
            field.MakeMove(startX + 10, startY + 12);

            field.MakeMove(startX, startY + 5);
            field.MakeMove(startX + 10, startY + 13);

            field.MakeMove(startX - 1, startY + 5);
            field.MakeMove(startX + 10, startY + 14);

            // left chain.
            field.MakeMove(startX - 2, startY + 4);
            field.MakeMove(startX + 10, startY + 15);

            field.MakeMove(startX - 2, startY + 3);
            field.MakeMove(startX + 10, startY + 16);

            field.MakeMove(startX - 2, startY + 2);
            field.MakeMove(startX + 10, startY + 17);

            field.MakeMove(startX - 1, startY + 1);
            field.MakeMove(startX + 10, startY + 18);

            // center.
            Assert.IsTrue(field.MakeMove(startX, startY + 3));
            field.MakeMove(startX + 10, startY + 19);

            Assert.IsTrue(field.MakeMove(startX + 2, startY + 2));
            field.MakeMove(startX + 10, startY + 20);

            Assert.IsTrue(field.MakeMove(startX + 2, startY + 3));
            field.MakeMove(startX + 10, startY + 21);

            Assert.IsTrue(field.MakeMove(startX + 3, startY + 3));

            // blue final.
            field.MakeMove(startX + 3, startY + 2);

            Assert.AreEqual(1, field.Player0CaptureCount);
            Assert.AreEqual(0, field.Player1CaptureCount);

            field.UnmakeAllMoves();

            Assert.IsTrue(field.IsEmpty);
            Assert.AreEqual(0, field.Player0CaptureCount + field.Player1CaptureCount);
        }

        protected void UCTBagEmptyBaseTest(Field field)
        {
            int startX = 16;
            int startY = 16;

            field.MakeMove(startX + 2, startY + 2, 0);

            field.MakeMove(startX + 2, startY, 0);
            field.MakeMove(startX + 3, startY, 0);
            field.MakeMove(startX + 4, startY, 0);

            field.MakeMove(startX + 5, startY + 1, 0);
            field.MakeMove(startX + 5, startY + 2, 0);
            field.MakeMove(startX + 5, startY + 3, 0);

            field.MakeMove(startX + 4, startY + 4, 0);
            field.MakeMove(startX + 3, startY + 4, 0);
            field.MakeMove(startX + 2, startY + 4, 0);
            field.MakeMove(startX + 1, startY + 4, 0);

            field.MakeMove(startX, startY + 3, 0);
            field.MakeMove(startX, startY + 2, 0);
            field.MakeMove(startX + 1, startY + 1, 0);

            // Put opponent point.
            field.MakeMove(startX + 4, startY + 1, 1);

            Assert.AreEqual(1, field.Player0CaptureCount);
            Assert.AreEqual(0, field.Player1CaptureCount);
        }

        protected void VerylongGameTest(Field field)
        {
            GameMove[] moves = TestUtils.LoadMovesFromPointsXt("VeryLongGame.sav");

            foreach (var move in moves)
            {
                Assert.IsTrue(field.MakeMove(move.Column, move.Row));
            }

            Assert.AreEqual(179, field.Player0CaptureCount);
            Assert.AreEqual(20, field.Player1CaptureCount);

            field.UnmakeAllMoves();

            Assert.AreEqual(0, field.Player0CaptureCount);
            Assert.AreEqual(0, field.Player1CaptureCount);
            Assert.IsTrue(field.IsEmpty);
        }

        #endregion
    }
}
