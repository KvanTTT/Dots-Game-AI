using System;
using System.IO;
using NUnit.Framework;
using DotsGame;

namespace DotsGame.Tests
{
	[TestFixture]
	public class FieldTests
	{
		#region Tests
		
		[Test]
		public virtual void SimpleSequenceTest()
		{
			SimpleSequenceTest(new Field(39, 32));
		}

		[Test]
		public virtual void BlueSurroundFirstTest()
		{
			BlueSurroundFirstTest(new Field(39, 32));
		}

		[Test]
		public virtual void OneBaseTest()
		{
			OneBaseTest(new Field(39, 32));
		}

		[Test]
		public virtual void BaseInBaseTest()
		{
			BaseInBaseTest(new Field(39, 32));
		}

		[Test]
		public virtual void ComplexBaseInBaseTest()
		{
			ComplexBaseInBaseTest(new Field(39, 32));
		}

		[Test]
		public virtual void EmptyBaseTest()
		{
			EmptyBaseTest(new Field(39, 32));
		}

		[Test]
		public virtual void BlueSurroundFirstInEmptyBase()
		{
			BlueSurroundFirstInEmptyBase(new Field(39, 32));
		}

		[Test]
		public virtual void ThreeAdjacentBasesTest()
		{
			ThreeAdjacentBasesTest(new Field(39, 32));
		}

		[Test]
		public virtual void BigEmptyBaseTest()
		{
			BigEmptyBaseTest(new Field(39, 32));
		}

		[Test]
		public virtual void UCTBagEmptyBaseTest()
		{
			UCTBagEmptyBaseTest(new Field(39, 32));
		}

		[Test]
		public virtual void VerylongGameTest()
		{
			VerylongGameTest(new Field(39, 32));
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
			Assert.AreEqual(0, field.RedCaptureCount + field.BlueCaptureCount);
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

			Assert.AreEqual(0, field.RedCaptureCount);
			Assert.AreEqual(1, field.BlueCaptureCount);

			field.UnmakeAllMoves();

			Assert.IsTrue(field.IsEmpty);
			Assert.AreEqual(0, field.RedCaptureCount + field.BlueCaptureCount);
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

			Assert.AreEqual(1, field.RedCaptureCount);

			field.UnmakeAllMoves();

			Assert.IsTrue(field.IsEmpty);
			Assert.AreEqual(0, field.RedCaptureCount + field.BlueCaptureCount);
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

			Assert.AreEqual(0, field.RedCaptureCount);
			Assert.AreEqual(1, field.BlueCaptureCount);

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

			Assert.AreEqual(4, field.RedCaptureCount);
			Assert.AreEqual(0, field.BlueCaptureCount);

			field.UnmakeAllMoves();

			Assert.IsTrue(field.IsEmpty);
			Assert.AreEqual(0, field.RedCaptureCount + field.BlueCaptureCount);
		}

		protected void ComplexBaseInBaseTest(Field field)
		{
			var buffer = DotsGame.Tests.Properties.Resources.DotFunctionsTest;

			for (var i = 58; i < buffer.Length; i += 13)
			{
				if (field.DotsSequenceCount == 24)
				{
					field.MakeMove(buffer[i], buffer[i + 1]);
					Assert.AreEqual(1, field.RedCaptureCount);
					Assert.AreEqual(0, field.BlueCaptureCount);
				}
				else
					field.MakeMove(buffer[i], buffer[i + 1]);
			}

			Assert.AreEqual(0, field.RedCaptureCount);
			Assert.AreEqual(16, field.BlueCaptureCount);

			field.UnmakeAllMoves();

			Assert.AreEqual(0, field.RedCaptureCount);
			Assert.AreEqual(0, field.BlueCaptureCount);
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

			Assert.AreEqual(1, field.RedCaptureCount);
			Assert.AreEqual(0, field.BlueCaptureCount);

			field.UnmakeAllMoves();

			Assert.IsTrue(field.IsEmpty);
			Assert.AreEqual(0, field.RedCaptureCount + field.BlueCaptureCount);
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

			Assert.AreEqual(0, field.RedCaptureCount);
			Assert.AreEqual(1, field.BlueCaptureCount);

			field.UnmakeAllMoves();

			Assert.IsTrue(field.IsEmpty);
			Assert.AreEqual(0, field.RedCaptureCount + field.BlueCaptureCount);
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

			Assert.AreEqual(3, field.RedCaptureCount);
			Assert.AreEqual(0, field.BlueCaptureCount);

			field.UnmakeAllMoves();

			Assert.IsTrue(field.IsEmpty);
			Assert.AreEqual(0, field.RedCaptureCount + field.BlueCaptureCount);
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

			Assert.AreEqual(1, field.RedCaptureCount);
			Assert.AreEqual(0, field.BlueCaptureCount);

			field.UnmakeAllMoves();

			Assert.IsTrue(field.IsEmpty);
			Assert.AreEqual(0, field.RedCaptureCount + field.BlueCaptureCount);
		}

		protected void UCTBagEmptyBaseTest(Field field)
		{
			int startX = 16;
			int startY = 16;

			field.MakeMove(startX + 2, startY + 2, Dot.RedPlayer);

			field.MakeMove(startX + 2, startY, Dot.RedPlayer);
			field.MakeMove(startX + 3, startY, Dot.RedPlayer);
			field.MakeMove(startX + 4, startY, Dot.RedPlayer);

			field.MakeMove(startX + 5, startY + 1, Dot.RedPlayer);
			field.MakeMove(startX + 5, startY + 2, Dot.RedPlayer);
			field.MakeMove(startX + 5, startY + 3, Dot.RedPlayer);

			field.MakeMove(startX + 4, startY + 4, Dot.RedPlayer);
			field.MakeMove(startX + 3, startY + 4, Dot.RedPlayer);
			field.MakeMove(startX + 2, startY + 4, Dot.RedPlayer);
			field.MakeMove(startX + 1, startY + 4, Dot.RedPlayer);

			field.MakeMove(startX, startY + 3, Dot.RedPlayer);
			field.MakeMove(startX, startY + 2, Dot.RedPlayer);
			field.MakeMove(startX + 1, startY + 1, Dot.RedPlayer);

			// Put opponent point.
			field.MakeMove(startX + 4, startY + 1, Dot.BluePlayer);

			Assert.AreEqual(1, field.RedCaptureCount);
			Assert.AreEqual(0, field.BlueCaptureCount);
		}

		protected void VerylongGameTest(Field field)
		{
			var buffer = DotsGame.Tests.Properties.Resources.VeryLongGame;
			for (var i = 58; i < buffer.Length; i += 13)
				Assert.IsTrue(field.MakeMove(buffer[i] + 1, buffer[i + 1] + 1));

			Assert.AreEqual(179, field.RedCaptureCount);
			Assert.AreEqual(20, field.BlueCaptureCount);

			field.UnmakeAllMoves();

			Assert.AreEqual(0, field.RedCaptureCount);
			Assert.AreEqual(0, field.BlueCaptureCount);
			Assert.IsTrue(field.IsEmpty);
		}

		#endregion
	}
}
