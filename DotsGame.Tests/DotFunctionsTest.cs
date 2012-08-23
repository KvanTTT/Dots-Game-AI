using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using DotsGame;

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
			Field field = new Field(39, 32);

			var buffer = DotsGame.Tests.Properties.Resources.DotFunctionsTest;

			for (var i = 58; i < buffer.Length; i += 13)
			{
				field.MakeMove(buffer[i] + 1, buffer[i + 1] + 1);
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
	}
}
