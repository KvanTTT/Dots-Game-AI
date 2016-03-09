﻿using DotsGame.Formats;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotsGame.Tests
{
    public static class TestUtils
    {
        public static GameMove[] LoadMovesFromPointsXt(string fileName)
        {
            var fullFileName = Path.Combine(TestContext.CurrentContext.TestDirectory, fileName);
            var parser = new PointsXtParser();
            var gameInfo = parser.Parse(File.ReadAllBytes(fullFileName));
            GameMove[] moves = gameInfo.GameTree.GetDefaultSequence().Select(tree => tree.Move).Skip(1).ToArray();
            return moves;
        }
    }
}