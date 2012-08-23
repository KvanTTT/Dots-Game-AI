using System;
using NUnit.Framework;
using System.Linq;
using DotsGame;
using DotsGame.AI;

namespace DotsGame.Tests
{
	[TestFixture]
	public class StrategicalMovesGeneratorTest
	{
		/// <summary>
		///A test for GenerateGroups
		///</summary>
		[Test]
		public void GenerateGroupsTest()
		{
			var field = new Field(39, 32);
			var buffer = DotsGame.Tests.Properties.Resources.VeryLongGame;
			for (var i = 58; i < buffer.Length; i += 13)
				field.MakeMove(buffer[i] + 1, buffer[i + 1] + 1);

			var analyzer = new StrategicMovesAnalyzer(field);
			analyzer.GenerateGroups();

			//Assert.AreEqual(analyzer.RedGroups.Count(), 5);
			//Assert.AreEqual(analyzer.BlueGroups.Count(), 16);
		}
	}
}
