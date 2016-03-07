using DotsGame.AI;
using NUnit.Framework;
using System.Linq;

namespace DotsGame.Tests
{
    [TestFixture]
    public class ZobristHashTest
    {
        [Test]
        public void CalculateHash_SimpleSequence()
        {
            int startX = 16;
            int startY = 16;

            var field = new Field(39, 32);
            ZobristHashField hash = new ZobristHashField(field, 0);

            ulong initKey = hash.Key;

            field.MakeMove(startX, startY);
            hash.UpdateHash();

            field.MakeMove(startX + 1, startY);
            hash.UpdateHash();

            field.MakeMove(startX + 1, startY + 1);
            hash.UpdateHash();

            field.MakeMove(startX, startY + 1);
            hash.UpdateHash();

            while (field.States.Count() != 0)
            {
                field.UnmakeMove();
                hash.UpdateHash();
            }

            Assert.AreEqual(hash.Key, initKey);
        }

        [Test]
        public void CalculateHash_SimpleBase()
        {
            int startX = 16;
            int startY = 16;

            var field = new Field(39, 32);
            ZobristHashField hash = new ZobristHashField(field, 0);

            ulong initKey = hash.Key;

            field.MakeMove(startX, startY);
            hash.UpdateHash();
            field.MakeMove(startX + 1, startY);
            hash.UpdateHash();

            field.MakeMove(startX + 1, startY + 1);
            hash.UpdateHash();
            field.MakeMove(startX + 2, startY + 1);
            hash.UpdateHash();

            field.MakeMove(startX + 1, startY - 1);
            hash.UpdateHash();
            field.MakeMove(startX + 2, startY - 1);
            hash.UpdateHash();

            field.MakeMove(startX + 2, startY);
            hash.UpdateHash();

            while (field.States.Count() != 0)
            {
                field.UnmakeMove();
                hash.UpdateHash();
            }

            Assert.AreEqual(hash.Key, initKey);
        }

        [Test]
        public void CalculateHash_DifferentBaseCreationOrder()
        {
            int startX = 16;
            int startY = 16;

            var field = new Field(39, 32);
            ZobristHashField hash = new ZobristHashField(field, 0);

            ulong initKey = hash.Key;

            field.MakeMove(startX, startY);
            hash.UpdateHash();
            field.MakeMove(startX + 1, startY);
            hash.UpdateHash();

            field.MakeMove(startX + 1, startY + 1);
            hash.UpdateHash();
            field.MakeMove(startX + 2, startY + 1);
            hash.UpdateHash();

            field.MakeMove(startX + 1, startY - 1);
            hash.UpdateHash();
            field.MakeMove(startX + 2, startY - 1);
            hash.UpdateHash();

            field.MakeMove(startX + 2, startY);
            hash.UpdateHash();
            field.MakeMove(startX + 3, startY);
            hash.UpdateHash();

            field.MakeMove(startX - 2, startY);
            hash.UpdateHash();
            field.MakeMove(startX - 2, startY - 1);
            hash.UpdateHash();

            ulong firstHash = hash.Key;

            while (field.States.Count() != 0)
            {
                field.UnmakeMove();
                hash.UpdateHash();
            }

            Assert.AreEqual(hash.Key, initKey);


            field.MakeMove(startX - 2, startY);
            hash.UpdateHash();
            field.MakeMove(startX + 1, startY);
            hash.UpdateHash();

            field.MakeMove(startX, startY);
            hash.UpdateHash();
            field.MakeMove(startX + 2, startY + 1);
            hash.UpdateHash();

            field.MakeMove(startX + 1, startY + 1);
            hash.UpdateHash();
            field.MakeMove(startX + 2, startY - 1);
            hash.UpdateHash();

            field.MakeMove(startX + 1, startY - 1);
            hash.UpdateHash();
            field.MakeMove(startX + 3, startY);
            hash.UpdateHash();

            field.MakeMove(startX + 2, startY);
            hash.UpdateHash();
            field.MakeMove(startX - 2, startY - 1);
            hash.UpdateHash();

            ulong secondHash = hash.Key;

            Assert.AreEqual(firstHash, secondHash);

            while (field.States.Count() != 0)
            {
                field.UnmakeMove();
                hash.UpdateHash();
            }

            Assert.AreEqual(initKey, hash.Key);
        }

        [Test]
        public void CalculateHash_BaseInBase()
        {
            int startX = 5;
            int startY = 2;

            var field = new Field(39, 32);
            ZobristHashField hash = new ZobristHashField(field, 0);

            ulong initKey = hash.Key;

            // center.
            field.MakeMove(startX, startY + 3);
            hash.UpdateHash();
            field.MakeMove(startX, startY + 1);
            hash.UpdateHash();

            field.MakeMove(startX + 10, startY + 1);
            hash.UpdateHash();
            field.MakeMove(startX + 1, startY + 3);
            hash.UpdateHash();

            field.MakeMove(startX + 1, startY + 2);
            hash.UpdateHash();
            field.MakeMove(startX + 10, startY + 2);
            hash.UpdateHash();

            field.MakeMove(startX + 2, startY + 3);
            hash.UpdateHash();
            field.MakeMove(startX + 10, startY + 3);
            hash.UpdateHash();

            field.MakeMove(startX + 1, startY + 4);
            hash.UpdateHash();
            field.MakeMove(startX + 10, startY + 4);
            hash.UpdateHash();

            PutBigBase(field, hash);

            ulong firstHash = hash.Key;

            while (field.States.Count() != 0)
            {
                field.UnmakeMove();
                hash.UpdateHash();
            }

            Assert.AreEqual(initKey, hash.Key);

            // center.
            field.MakeMove(startX + 1, startY + 3);
            hash.UpdateHash();
            field.MakeMove(startX, startY + 1);
            hash.UpdateHash();

            field.MakeMove(startX + 10, startY + 1);
            hash.UpdateHash();
            field.MakeMove(startX + 2, startY + 3);
            hash.UpdateHash();

            field.MakeMove(startX + 2, startY + 2);
            hash.UpdateHash();
            field.MakeMove(startX + 10, startY + 2);
            hash.UpdateHash();

            field.MakeMove(startX + 3, startY + 3);
            hash.UpdateHash();
            field.MakeMove(startX + 10, startY + 3);
            hash.UpdateHash();

            field.MakeMove(startX + 4, startY + 4);
            hash.UpdateHash();
            field.MakeMove(startX + 10, startY + 4);
            hash.UpdateHash();

            PutBigBase(field, hash);

            ulong secondHash = hash.Key;

            Assert.AreEqual(firstHash, secondHash);

            while (field.States.Count() != 0)
            {
                field.UnmakeMove();
                hash.UpdateHash();
            }

            Assert.AreEqual(initKey, hash.Key);

            startX = 16;
            startY = 16;
        }

        [Test]
        public void CalculateHash_EnemyBaseInBase()
        {
            int startX = 5;
            int startY = 2;

            var field = new Field(39, 32);
            ZobristHashField hash = new ZobristHashField(field, 0);

            ulong initKey = hash.Key;

            // center.
            field.MakeMove(startX + 1, startY + 3);
            hash.UpdateHash();
            field.MakeMove(startX, startY + 3);
            hash.UpdateHash();

            field.MakeMove(startX + 10, startY + 2);
            hash.UpdateHash();
            field.MakeMove(startX + 1, startY + 2);
            hash.UpdateHash();

            field.MakeMove(startX + 10, startY + 3);
            hash.UpdateHash();
            field.MakeMove(startX + 2, startY + 2);
            hash.UpdateHash();

            field.MakeMove(startX + 10, startY + 4);
            hash.UpdateHash();
            field.MakeMove(startX + 3, startY + 3);
            hash.UpdateHash();

            field.MakeMove(startX + 10, startY + 5);
            hash.UpdateHash();
            field.MakeMove(startX + 2, startY + 4);
            hash.UpdateHash();

            field.MakeMove(startX + 10, startY + 6);
            hash.UpdateHash();
            field.MakeMove(startX + 1, startY + 4);
            hash.UpdateHash();

            PutBigBase(field, hash);

            ulong firstHash = hash.Key;

            while (field.States.Count() != 0)
            {
                field.UnmakeMove();
                hash.UpdateHash();
            }

            Assert.AreEqual(initKey, hash.Key);

            startX = 16;
            startY = 16;
        }

        [Test]
        public void CalculateHash_EmptyBase()
        {
            int startX = 16;
            int startY = 16;

            var field = new Field(39, 32);
            ZobristHashField hash = new ZobristHashField(field, 0);

            ulong initKey = hash.Key;

            field.MakeMove(startX, startY);
            hash.UpdateHash();
            field.MakeMove(startX + 10, startY);
            hash.UpdateHash();

            field.MakeMove(startX + 1, startY + 1);
            hash.UpdateHash();
            field.MakeMove(startX + 10, startY + 1);
            hash.UpdateHash();

            field.MakeMove(startX + 2, startY + 1);
            hash.UpdateHash();
            field.MakeMove(startX + 10, startY + 2);
            hash.UpdateHash();

            field.MakeMove(startX + 3, startY);
            hash.UpdateHash();
            field.MakeMove(startX + 10, startY + 3);
            hash.UpdateHash();

            field.MakeMove(startX + 2, startY - 1);
            hash.UpdateHash();
            field.MakeMove(startX + 10, startY + 4);
            hash.UpdateHash();

            field.MakeMove(startX + 1, startY - 1);
            hash.UpdateHash();
            field.MakeMove(startX + 1, startY);
            hash.UpdateHash();

            while (field.States.Count() != 0)
            {
                field.UnmakeMove();
                hash.UpdateHash();
            }

            Assert.AreEqual(initKey, hash.Key);
        }

        [Test]
        public void CalculateHash_VeryLongGame()
        {
            var field = new Field(39, 32);
            ZobristHashField hash = new ZobristHashField(field, 0);

            ulong initKey = hash.Key;

            GameMove[] moves = TestUtils.LoadMovesFromPointsXt("VeryLongGame.sav");

            foreach (var move in moves)
            {
                Assert.AreEqual(true, field.MakeMove(move.Column, move.Row));
                hash.UpdateHash();
            }

            while (field.States.Count() != 0)
            {
                field.UnmakeMove();
                hash.UpdateHash();
            }

            Assert.AreEqual(initKey, hash.Key);
        }

        private void PutBigBase(Field field, ZobristHashField hash)
        {
            int startX = 5;
            int startY = 2;

            // top.
            field.MakeMove(startX, startY);
            hash.UpdateHash();
            field.MakeMove(startX + 11, startY);
            hash.UpdateHash();

            field.MakeMove(startX + 1, startY);
            hash.UpdateHash();
            field.MakeMove(startX + 11, startY + 1);
            hash.UpdateHash();

            field.MakeMove(startX + 2, startY);
            hash.UpdateHash();
            field.MakeMove(startX + 11, startY + 2);
            hash.UpdateHash();

            field.MakeMove(startX + 3, startY);
            hash.UpdateHash();
            field.MakeMove(startX + 11, startY + 3);
            hash.UpdateHash();

            field.MakeMove(startX + 4, startY + 1);
            hash.UpdateHash();
            field.MakeMove(startX + 11, startY + 4);
            hash.UpdateHash();

            // right.
            field.MakeMove(startX + 5, startY + 2);
            hash.UpdateHash();
            field.MakeMove(startX + 12, startY);
            hash.UpdateHash();

            field.MakeMove(startX + 5, startY + 3);
            hash.UpdateHash();
            field.MakeMove(startX + 12, startY + 1);
            hash.UpdateHash();

            field.MakeMove(startX + 5, startY + 4);
            hash.UpdateHash();
            field.MakeMove(startX + 12, startY + 2);
            hash.UpdateHash();

            field.MakeMove(startX + 4, startY + 5);
            hash.UpdateHash();
            field.MakeMove(startX + 12, startY + 3);
            hash.UpdateHash();

            // bottom
            field.MakeMove(startX + 3, startY + 6);
            hash.UpdateHash();
            field.MakeMove(startX + 13, startY);
            hash.UpdateHash();

            field.MakeMove(startX + 2, startY + 6);
            hash.UpdateHash();
            field.MakeMove(startX + 13, startY + 1);
            hash.UpdateHash();

            field.MakeMove(startX + 1, startY + 6);
            hash.UpdateHash();
            field.MakeMove(startX + 13, startY + 2);
            hash.UpdateHash();

            field.MakeMove(startX, startY + 6);
            hash.UpdateHash();
            field.MakeMove(startX + 13, startY + 3);
            hash.UpdateHash();

            field.MakeMove(startX - 1, startY + 5);
            hash.UpdateHash();
            field.MakeMove(startX + 13, startY + 4);
            hash.UpdateHash();

            // left.
            field.MakeMove(startX - 2, startY + 4);
            hash.UpdateHash();
            field.MakeMove(startX + 14, startY);
            hash.UpdateHash();

            field.MakeMove(startX - 2, startY + 3);
            hash.UpdateHash();
            field.MakeMove(startX + 14, startY + 1);
            hash.UpdateHash();

            field.MakeMove(startX - 2, startY + 2);
            hash.UpdateHash();
            field.MakeMove(startX + 14, startY + 2);
            hash.UpdateHash();

            field.MakeMove(startX - 1, startY + 1);
            hash.UpdateHash();
            field.MakeMove(startX + 14, startY + 3);
            hash.UpdateHash();
        }
    }
}
