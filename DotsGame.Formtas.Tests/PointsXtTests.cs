﻿using System.Collections.Generic;
using System.IO;
using DotsGame.Formats;
using NUnit.Framework;

namespace DotsGame.Formtas.Tests
{
    [TestFixture]
    public class PointsXtTests
    {
        [Test]
        public void Parse_PointsXtSimple()
        {
            var fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, "PointsXtSimple.sav");
            var parser = new PointsXtParser();
            var gameInfo = parser.Parse(File.ReadAllBytes(fileName));
            IList<GameTree> moves = gameInfo.GameTree.GetDefaultSequence();
            Assert.AreEqual(5, moves.Count);
            Assert.IsTrue(moves[0].Root);
            Assert.AreEqual(17, moves[1].Move.Row);
            Assert.AreEqual(19, moves[1].Move.Column);
            Assert.AreEqual(17, moves[2].Move.Row);
            Assert.AreEqual(20, moves[2].Move.Column);
            Assert.AreEqual(16, moves[3].Move.Row);
            Assert.AreEqual(20, moves[3].Move.Column);
            Assert.AreEqual(16, moves[4].Move.Row);
            Assert.AreEqual(19, moves[4].Move.Column);
        }
    }
}
