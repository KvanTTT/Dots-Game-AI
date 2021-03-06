﻿using DotsGame.AI;
using NUnit.Framework;

namespace DotsGame.Tests
{
    [TestFixture]
    public class AlphaBetaAlgoritmTest
    {
        [Test]
        public void AlphaBeta_SimpleAttack()
        {
            int startX = 16;
            int startY = 16;
            var field = new Field(39, 32);

            field.MakeMove(startX, startY);
            field.MakeMove(startX + 1, startY);

            field.MakeMove(startX + 1, startY + 1);
            field.MakeMove(startX, startY + 1);

            field.MakeMove(startX + 1, startY - 1);
            field.MakeMove(startX, startY - 1);

            int expectedBestMove = Field.GetPosition(startX + 2, startY);

            var alphaBetaAlgoritm = new AlphaBetaAlgoritm(field);
            int alphaBetaBestMove = alphaBetaAlgoritm.SearchBestMove(1);

            Assert.AreEqual(expectedBestMove, alphaBetaBestMove);

            alphaBetaAlgoritm = new AlphaBetaAlgoritm(field);
            alphaBetaBestMove = alphaBetaAlgoritm.SearchBestMove(2);

            Assert.AreEqual(expectedBestMove, alphaBetaBestMove);
        }

        [Test]
        public void AlphaBeta_SimpleDefense()
        {
            int startX = 16;
            int startY = 16;
            var field = new Field(39, 32);

            field.MakeMove(startX, startY);
            field.MakeMove(startX + 1, startY);

            field.MakeMove(startX + 1, startY + 1);
            field.MakeMove(startX, startY + 1);

            field.MakeMove(startX + 10, startY - 1);
            field.MakeMove(startX, startY - 1);

            int expectedBestMove = Field.GetPosition(startX - 1, startY);

            var alphaBetaAlgoritm = new AlphaBetaAlgoritm(field);
            int alphaBetaBestMove = alphaBetaAlgoritm.SearchBestMove(2);

            Assert.AreEqual(expectedBestMove, alphaBetaBestMove);

            alphaBetaAlgoritm = new AlphaBetaAlgoritm(field);
            alphaBetaBestMove = alphaBetaAlgoritm.SearchBestMove(3);

            Assert.AreEqual(expectedBestMove, alphaBetaBestMove);
        }

        [Test]
        public void AlphaBeta_Square()
        {
            int startX = 16;
            int startY = 16;
            var field = new Field(39, 32);

            field.MakeMove(startX, startY);
            field.MakeMove(startX + 1, startY);

            field.MakeMove(startX + 1, startY - 1);
            field.MakeMove(startX + 10, startY + 1);

            field.MakeMove(startX + 2, startY - 1);
            field.MakeMove(startX + 10, startY + 2);

            field.MakeMove(startX + 3, startY - 1);
            field.MakeMove(startX + 10, startY + 3);

            field.MakeMove(startX + 1, startY + 1);
            field.MakeMove(startX + 11, startY + 1);

            field.MakeMove(startX + 2, startY + 1);
            field.MakeMove(startX + 11, startY + 2);

            field.MakeMove(startX + 3, startY + 1);
            field.MakeMove(startX + 11, startY + 3);

            int expectedBestMove = Field.GetPosition(startX + 4, startY);

            var alphaBetaAlgoritm = new AlphaBetaAlgoritm(field);
            int alphaBetaBestMove = alphaBetaAlgoritm.SearchBestMove(1);

            Assert.AreEqual(expectedBestMove, alphaBetaBestMove);

            alphaBetaAlgoritm = new AlphaBetaAlgoritm(field);
            alphaBetaBestMove = alphaBetaAlgoritm.SearchBestMove(2);

            Assert.AreEqual(expectedBestMove, alphaBetaBestMove);
        }

        [Test]
        public void AlphaBeta_SimpleBlueAttack()
        {
            int startX = 16;
            int startY = 16;
            var field = new Field(39, 32);

            field.MakeMove(startX, startY);
            field.MakeMove(startX + 1, startY);

            field.MakeMove(startX + 1, startY + 1);
            field.MakeMove(startX, startY + 1);

            field.MakeMove(startX + 1, startY - 1);
            field.MakeMove(startX, startY - 1);

            field.MakeMove(startX + 10, startY);

            int expectedDestMove = Field.GetPosition(startX - 1, startY);

            var alphaBetaAlgoritm = new AlphaBetaAlgoritm(field);
            int alphaBetaBestMove = alphaBetaAlgoritm.SearchBestMove(1);

            Assert.AreEqual(expectedDestMove, alphaBetaBestMove);

            alphaBetaAlgoritm = new AlphaBetaAlgoritm(field);
            alphaBetaBestMove = alphaBetaAlgoritm.SearchBestMove(2);

            Assert.AreEqual(expectedDestMove, alphaBetaBestMove);
        }

        [Test]
        public void AlphaBeta_SimpleBlueDefense()
        {
            int startX = 16;
            int startY = 16;
            var field = new Field(39, 32);

            field.MakeMove(startX, startY);
            field.MakeMove(startX + 1, startY);

            field.MakeMove(startX + 1, startY + 1);
            field.MakeMove(startX, startY + 1);

            field.MakeMove(startX + 1, startY - 1);
            field.MakeMove(startX + 10, startY - 1);

            field.MakeMove(startX - 2, startY);

            int expectedDestMove = Field.GetPosition(startX + 2, startY);

            var alphaBetaAlgoritm = new AlphaBetaAlgoritm(field);
            int alphaBetaBestMove = alphaBetaAlgoritm.SearchBestMove(2);

            Assert.AreEqual(expectedDestMove, alphaBetaBestMove);

            alphaBetaAlgoritm = new AlphaBetaAlgoritm(field);
            alphaBetaBestMove = alphaBetaAlgoritm.SearchBestMove(3);

            Assert.AreEqual(expectedDestMove, alphaBetaBestMove);
        }
    }
}