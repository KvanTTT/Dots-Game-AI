using System;
using System.IO;
using System.Linq;
using System.Text;
using DotsGame.Formats;
using DotsGame.Sgf;
using NUnit.Framework;

namespace DotsGame.Formtas.Tests
{
    [TestFixture]
    public class SgfTests
    {
        private string SimpleCrosswiseSgf = "(;AP[Спортивные Точки (playdots.ru)]GM[40]FF[4]CA[UTF-8]SZ[39:32]DT[2016-03-10 21:13:42]RU[russian]PB[Первый игрок]PW[Второй игрок];B[bE];W[cE];B[cD];W[bD])";
        private string TreeSgf = @"
                (;GM[40]FF[4]AP[Drago:4.22.02]SZ[19]CA[UTF-8]
                (;B[jj];W[kh])
                (;B[gi]
                 (;W[fm])
                 (;W[mj];B[ki])
                )
                (;B[pl];W[pi]))";

        [Test]
        public void Parse_SimpleCrosswiseSgf()
        {
            var parser = new SgfParser();
            GameInfo gameInfo = parser.Parse(Encoding.UTF8.GetBytes(SimpleCrosswiseSgf));
            CheckCrosswise(gameInfo);
            Assert.IsFalse(gameInfo.FromUrl);
        }

        [Test]
        public void Serialize_SimpleCrosswiseSgf()
        {
            var parser = new SgfParser();
            GameInfo gameInfo = parser.Parse(Encoding.UTF8.GetBytes(SimpleCrosswiseSgf));
            byte[] serialzied = parser.Serialize(gameInfo);
            GameInfo deserialized = parser.Parse(serialzied);
            CheckCrosswise(deserialized);
        }

        [TestCase("874744", Ignore = "app is down")]
        [TestCase("https://game.playdots.ru/game/redirect?game_id=874744", Ignore = "playdots app is down")]
        public void DownloadParse_SgfFromVkComplex(string fileNameOrPath)
        {
            var extractor = new GameInfoExtractor();
            GameInfo info = extractor.DetectFormatAndOpen(fileNameOrPath);
            Assert.IsTrue(info.FromUrl);
            Assert.AreEqual(new DateTime(2016, 02, 08, 13, 49, 30), info.Date);
            Assert.AreEqual("Сергей Чернобровин", info.Player1Name);
            Assert.AreEqual("Ксюша Димарчук", info.Player2Name);
            Assert.AreEqual("without territory, with landing, double crossing in the center, 25 sec for a move, 4 min for a round, spectators 25, average score 9.0, duration 52:05", info.Description);
        }

        [Test]
        public void Parse_SgfTree()
        {
            var parser = new SgfParser();
            var gameInfo = parser.Parse(Encoding.UTF8.GetBytes(TreeSgf));
            CheckTree(gameInfo);
        }

        [Test]
        public void Serialize_SgfTree()
        {
            var parser = new SgfParser { NewLines = true };
            GameInfo gameInfo = parser.Parse(Encoding.UTF8.GetBytes(TreeSgf));
            byte[] serialized = parser.Serialize(gameInfo);
            GameInfo deserialized = parser.Parse(serialized);
            CheckTree(deserialized);
        }

        [Test]
        public void Parse_FullSgfWithStop()
        {
            var fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, "FullSgfWithStop.sgf");
            var parser = new SgfParser();
            GameInfo gameInfo = parser.Parse(File.ReadAllBytes(fileName));
            CheckFullInfo(gameInfo);
        }

        [Test]
        public void Serialize_FullSgfWithStop()
        {
            var fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, "FullSgfWithStop.sgf");
            var parser = new SgfParser();
            GameInfo gameInfo = parser.Parse(File.ReadAllBytes(fileName));
            byte[] serialized = parser.Serialize(gameInfo);
            GameInfo deserialized = parser.Parse(serialized);
            string str = Encoding.UTF8.GetString(serialized);
            CheckFullInfo(deserialized);
        }

        private static void CheckCrosswise(GameInfo gameInfo, bool checkDate = true)
        {
            Assert.AreEqual("Спортивные Точки (playdots.ru)", gameInfo.AppName);
            if (checkDate)
            {
                Assert.AreEqual(new DateTime(2016, 03, 10, 21, 13, 42), gameInfo.Date);
            }
            Assert.AreEqual(GameType.Kropki, gameInfo.GameType);
            Assert.AreEqual(39, gameInfo.Width);
            Assert.AreEqual(32, gameInfo.Height);
            Assert.AreEqual("Первый игрок", gameInfo.Player1Name);
            Assert.AreEqual("Второй игрок", gameInfo.Player2Name);
            GameTree nextTree = gameInfo.GameTree;
            Assert.IsTrue(nextTree.Root);
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

        private static void CheckTree(GameInfo gameInfo)
        {
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

        private static void CheckFullInfo(GameInfo gameInfo)
        {
            Assert.AreEqual("Спортивные Точки (playdots.ru)", gameInfo.AppName);
            Assert.AreEqual(GameType.Kropki, gameInfo.GameType);
            Assert.AreEqual(39, gameInfo.Width);
            Assert.AreEqual(32, gameInfo.Height);
            Assert.AreEqual("russian", gameInfo.Rules);
            Assert.AreEqual("Синий игрок", gameInfo.Player1Name);
            Assert.AreEqual("Красный игрок", gameInfo.Player2Name);
            Assert.AreEqual(Rank.Grandmaster, gameInfo.Player1Rank);
            Assert.AreEqual(2022, gameInfo.Player1Rating);
            Assert.AreEqual(Rank.Master, gameInfo.Player2Rank);
            Assert.AreEqual(1823, gameInfo.Player2Rating);
            Assert.AreEqual(new DateTime(2016, 03, 13, 01, 14, 30), gameInfo.Date);
            Assert.AreEqual("rating", gameInfo.Event);
            Assert.AreEqual("https://playdots.ru/game-info/?id=962686", gameInfo.Source);
            Assert.AreEqual(240, gameInfo.TimeLimits.TotalSeconds);
            Assert.AreEqual("20 sec / move", gameInfo.OverTime);
            Assert.AreEqual(0, gameInfo.WinPlayerNumber);
            Assert.AreEqual(WinReason.Score, gameInfo.WinReason);
            Assert.AreEqual(1, gameInfo.WinScore);
            Assert.AreEqual("B+1", gameInfo.Result);
            Assert.AreEqual("without territory, with landing, single crossing in the center, 20 sec for a move, 4 min for a round, spectators 5, average score 8.5, duration 13:08", gameInfo.Description);

            Assert.AreEqual(new GameMove(0, 7, 19), gameInfo.GameTree.GameMoves[0]);
            Assert.AreEqual(new GameMove(0, 8, 20), gameInfo.GameTree.GameMoves[1]);
            Assert.AreEqual(new GameMove(1, 7, 20), gameInfo.GameTree.GameMoves[2]);
            Assert.AreEqual(new GameMove(1, 8, 19), gameInfo.GameTree.GameMoves[3]);

            var lastStopMove = gameInfo.GameTree.GetDefaultSequence().Last();

            Assert.AreEqual(new GameMove(1, 8, 17), lastStopMove.GameMoves[0]);
            Assert.AreEqual(new GameMove(1, 9, 17), lastStopMove.GameMoves[1]);
        }
    }
}
