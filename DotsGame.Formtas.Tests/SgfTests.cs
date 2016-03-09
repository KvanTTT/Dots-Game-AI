using DotsGame.Formats;
using DotsGame.Sgf;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotsGame.Formtas.Tests
{
    [TestFixture]
    public class SgfTests
    {
        [Test]
        public void Parse_SgfSimpleCrosswise()
        {
            var testData = "(;AP[Спортивные Точки (playdots.ru)]GM[40]FF[4]CA[UTF-8]SZ[39:32]RU[russian]PB[Первый игрок]PW[Второй игрок];B[bE];W[cE];B[cD];W[bD])";
            var parser = new SgfParser();
            var gameInfo = parser.Parse(Encoding.UTF8.GetBytes(testData));
            CheckCrosswise(gameInfo);
            Assert.IsFalse(gameInfo.FromUrl);
        }

        [TestCase("5547")]
        [TestCase("https://game.playdots.ru/practice/redirect?game_id=5547")]
        public void DownloadParse_SgfFromVkCrosswise(string fileNameOrPath)
        {
            var extractor = new GameInfoExtractor();
            GameInfo info = extractor.DetectFormatAndOpen(fileNameOrPath);
            CheckCrosswise(info);
            Assert.IsTrue(info.FromUrl);
        }

        [TestCase("874744")]
        [TestCase("https://game.playdots.ru/game/redirect?game_id=874744")]
        public void DownloadParse_SgfFromVkComplex(string fileNameOrPath)
        {
            var extractor = new GameInfoExtractor();
            GameInfo info = extractor.DetectFormatAndOpen(fileNameOrPath);
            Assert.IsTrue(info.FromUrl);
            Assert.AreEqual(new DateTime(2016, 02, 08, 13, 49, 30), info.Date);
            Assert.AreEqual("Сергей Чернобровин", info.Player2Name);
            Assert.AreEqual("Ксюша Димарчук", info.Player1Name);
            Assert.AreEqual("без территории, с заземлением, двойной скрест в центре, 25 сек на ход, 4 мин на партию", info.Description);
        }

        [Test]
        public void Parse_SgfTree()
        {
            string sgfData = @"
                (;GM[40]FF[4]AP[Drago:4.22.02]SZ[19]CA[UTF-8]
                (;B[jj];W[kh])
                (;B[gi]
                 (;W[fm])
                 (;W[mj];B[ki])
                )
                (;B[pl];W[pi]))";
            var parser = new SgfParser();
            var gameInfo = parser.Parse(Encoding.UTF8.GetBytes(sgfData));

            Assert.AreEqual(19, gameInfo.Width);
            Assert.AreEqual(19, gameInfo.Height);
            GameTree tree = gameInfo.GameTree;

            Assert.AreEqual(10, tree.Childs[0].Move.Row);
            Assert.AreEqual(10, tree.Childs[0].Move.Column);
            Assert.AreEqual(9, tree.Childs[1].Move.Row);
            Assert.AreEqual(7, tree.Childs[1].Move.Column);
            Assert.AreEqual(12, tree.Childs[2].Move.Row);
            Assert.AreEqual(16, tree.Childs[2].Move.Column);

            Assert.AreEqual(tree, tree.Childs[0].Parent);
            Assert.AreEqual(tree, tree.Childs[1].Parent);
            Assert.AreEqual(tree, tree.Childs[2].Parent);

            tree = gameInfo.GameTree.Childs[0];
            Assert.AreEqual(8, tree.Childs[0].Move.Row);
            Assert.AreEqual(11, tree.Childs[0].Move.Column);
            Assert.AreEqual(tree, tree.Childs[0].Parent);

            tree = gameInfo.GameTree.Childs[1];
            Assert.AreEqual(13, tree.Childs[0].Move.Row);
            Assert.AreEqual(6, tree.Childs[0].Move.Column);
            Assert.AreEqual(tree, tree.Childs[0].Parent);
            Assert.AreEqual(10, tree.Childs[1].Move.Row);
            Assert.AreEqual(13, tree.Childs[1].Move.Column);
            Assert.AreEqual(tree, tree.Childs[1].Parent);

            tree = gameInfo.GameTree.Childs[2];
            Assert.AreEqual(9, tree.Childs[0].Move.Row);
            Assert.AreEqual(16, tree.Childs[0].Move.Column);
            Assert.AreEqual(tree, tree.Childs[0].Parent);
        }

        private static void CheckCrosswise(GameInfo gameInfo)
        {
            Assert.AreEqual("Спортивные Точки (playdots.ru)", gameInfo.AppName);
            Assert.AreEqual(GameType.Kropki, gameInfo.GameType);
            Assert.AreEqual(Encoding.UTF8, gameInfo.Encoding);
            Assert.AreEqual(39, gameInfo.Width);
            Assert.AreEqual(32, gameInfo.Height);
            Assert.AreEqual("Первый игрок", gameInfo.Player2Name); // TODO: Fix
            Assert.AreEqual("Второй игрок", gameInfo.Player1Name);
            GameTree nextTree = gameInfo.GameTree;
            Assert.IsTrue(nextTree.Move.IsRoot);
            nextTree = nextTree.Childs.First();
            Assert.AreEqual(31, nextTree.Move.Row);
            Assert.AreEqual(2, nextTree.Move.Column);
            nextTree = nextTree.Childs.First();
            Assert.AreEqual(31, nextTree.Move.Row);
            Assert.AreEqual(3, nextTree.Move.Column);
            nextTree = nextTree.Childs.First();
            Assert.AreEqual(30, nextTree.Move.Row);
            Assert.AreEqual(3, nextTree.Move.Column);
            nextTree = nextTree.Childs.First();
            Assert.AreEqual(30, nextTree.Move.Row);
            Assert.AreEqual(2, nextTree.Move.Column);
            Assert.AreEqual(0, nextTree.Childs.Count);
        }
    }
}
