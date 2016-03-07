using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using DotsGame;
using DotsGame.AI;

namespace DotsGame.Tests
{
    [TestFixture]
    public class LinkedGroupTest
    {
        [Test]
        public void EnvelopeOneDotTest()
        {
            int startX = 16;
            int startY = 16;
            Field field = new Field(39, 32);

            field.MakeMove(startX, startY, DotState.RedPlayer);

            LinkedGroup linkedGroup = new LinkedGroup(DotState.RedPlayer, 1, field.DotsSequancePositions.ToList());
            linkedGroup.BuildEnvelope();

            Assert.AreEqual(1, linkedGroup.EnvelopePositions.Count());

            var envelopePositions = linkedGroup.EnvelopePositions;
            Assert.IsTrue(envelopePositions.Contains(Field.GetPosition(startX, startY)));
        }

        [Test]
        public void EnvelopeTwoDotsTest()
        {
            int startX = 16;
            int startY = 16;
            Field field = new Field(39, 32);

            field.MakeMove(startX, startY, DotState.RedPlayer);
            field.MakeMove(startX, startY + 3, DotState.RedPlayer);

            LinkedGroup linkedGroup = new LinkedGroup(DotState.RedPlayer, 1, field.DotsSequancePositions.ToList());
            linkedGroup.BuildEnvelope();

            Assert.AreEqual(2, linkedGroup.EnvelopePositions.Count());
        }

        [Test]
        public void EnvelopeThreeDotsTest()
        {
            int startX = 16;
            int startY = 16;
            Field field = new Field(39, 32);

            field.MakeMove(startX, startY, DotState.RedPlayer);
            field.MakeMove(startX + 3, startY + 3, DotState.RedPlayer);
            field.MakeMove(startX, startY + 6, DotState.RedPlayer);

            LinkedGroup linkedGroup = new LinkedGroup(DotState.RedPlayer, 1, field.DotsSequancePositions.ToList());
            linkedGroup.BuildEnvelope();

            Assert.AreEqual(3, linkedGroup.EnvelopePositions.Count());
        }

        /// <summary>
        ///A test for BuildEnvelope
        ///</summary>
        [Test]
        public void EnvelopeManyDotsTest()
        {
            int startX = 16;
            int startY = 16;
            Field field = new Field(39, 32);

            field.MakeMove(startX, startY, DotState.RedPlayer);
            field.MakeMove(startX, startY + 1, DotState.RedPlayer);

            field.MakeMove(startX + 2, startY - 2, DotState.RedPlayer);
            field.MakeMove(startX + 2, startY, DotState.RedPlayer);
            field.MakeMove(startX + 2, startY + 1, DotState.RedPlayer);

            field.MakeMove(startX + 4, startY - 2, DotState.RedPlayer);
            field.MakeMove(startX + 4, startY, DotState.RedPlayer);

            field.MakeMove(startX + 5, startY - 1, DotState.RedPlayer);
            field.MakeMove(startX + 5, startY + 2, DotState.RedPlayer);

            field.MakeMove(startX + 6, startY - 4, DotState.RedPlayer);
            field.MakeMove(startX + 6, startY, DotState.RedPlayer);

            field.MakeMove(startX + 7, startY - 2, DotState.RedPlayer);

            LinkedGroup linkedGroup = new LinkedGroup(DotState.RedPlayer, 1, field.DotsSequancePositions.ToList());

            Assert.AreEqual(6, linkedGroup.EnvelopePositions.Count());

            var envelopePositions = linkedGroup.EnvelopePositions;
            Assert.IsTrue(envelopePositions.Contains(Field.GetPosition(startX, startY)));
            Assert.IsTrue(envelopePositions.Contains(Field.GetPosition(startX, startY + 1)));
            Assert.IsTrue(envelopePositions.Contains(Field.GetPosition(startX + 2, startY - 2)));
            Assert.IsTrue(envelopePositions.Contains(Field.GetPosition(startX + 5, startY + 2)));
            Assert.IsTrue(envelopePositions.Contains(Field.GetPosition(startX + 6, startY - 4)));
            Assert.IsTrue(envelopePositions.Contains(Field.GetPosition(startX + 7, startY - 2)));
        }

        [Test]
        public void EnvelopeManyDotsTest2()
        {
            int startX = 16;
            int startY = 16;
            Field field = new Field(39, 32);

            field.MakeMove(startX, startY, DotState.RedPlayer);
            field.MakeMove(startX + 1, startY + 1, DotState.RedPlayer);
            field.MakeMove(startX + 3, startY + 1, DotState.RedPlayer);
            field.MakeMove(startX + 2, startY + 2, DotState.RedPlayer);
            field.MakeMove(startX + 4, startY + 2, DotState.RedPlayer);
            field.MakeMove(startX + 2, startY + 3, DotState.RedPlayer);
            field.MakeMove(startX + 1, startY + 4, DotState.RedPlayer);
            field.MakeMove(startX + 1, startY + 5, DotState.RedPlayer);

            LinkedGroup linkedGroup = new LinkedGroup(DotState.RedPlayer, 1, field.DotsSequancePositions.ToList());
            linkedGroup.BuildEnvelope();

            Assert.AreEqual(4, linkedGroup.EnvelopePositions.Count());

            var envelopePositions = linkedGroup.EnvelopePositions;
            Assert.IsTrue(envelopePositions.Contains(Field.GetPosition(startX, startY)));
            Assert.IsTrue(envelopePositions.Contains(Field.GetPosition(startX + 3, startY + 1)));
            Assert.IsTrue(envelopePositions.Contains(Field.GetPosition(startX + 4, startY + 2)));
            Assert.IsTrue(envelopePositions.Contains(Field.GetPosition(startX + 1, startY + 5)));
        }
    }
}
