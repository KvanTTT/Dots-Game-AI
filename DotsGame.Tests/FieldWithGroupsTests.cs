using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace DotsGame.Tests
{
	[TestFixture]
	public class FieldWithGroupsTests : FieldTests
	{
		[Test]
		public override void SimpleSequenceTest()
		{
			var field = new FieldWithGroups(39, 32);
			SimpleSequenceTest(field);
			Assert.AreEqual(0, field.DiagonalLinkedGroupsCount);
		}

		[Test]
		public override void BlueSurroundFirstTest()
		{
			var field = new FieldWithGroups(39, 32);
			BlueSurroundFirstTest(field);
			Assert.AreEqual(0, field.DiagonalLinkedGroupsCount);
		}

		[Test]
		public override void OneBaseTest()
		{
			var field = new FieldWithGroups(39, 32);
			OneBaseTest(field);
			Assert.AreEqual(0, field.DiagonalLinkedGroupsCount);
		}

		[Test]
		public override void BaseInBaseTest()
		{
			var field = new FieldWithGroups(39, 32);
			BaseInBaseTest(field);
			Assert.AreEqual(0, field.DiagonalLinkedGroupsCount);
		}

		[Test]
		public override void ComplexBaseInBaseTest()
		{
			var field = new FieldWithGroups(39, 32);
			ComplexBaseInBaseTest(field);
			Assert.AreEqual(0, field.DiagonalLinkedGroupsCount);
		}

		[Test]
		public override void EmptyBaseTest()
		{
			var field = new FieldWithGroups(39, 32);
			EmptyBaseTest(field);
			Assert.AreEqual(0, field.DiagonalLinkedGroupsCount);
		}

		[Test]
		public override void BlueSurroundFirstInEmptyBase()
		{
			var field = new FieldWithGroups(39, 32);
			BlueSurroundFirstInEmptyBase(field);
			Assert.AreEqual(0, field.DiagonalLinkedGroupsCount);
		}

		[Test]
		public override void ThreeAdjacentBasesTest()
		{
			var field = new FieldWithGroups(39, 32);
			ThreeAdjacentBasesTest(field);
			Assert.AreEqual(0, field.DiagonalLinkedGroupsCount);
		}

		[Test]
		public override void BigEmptyBaseTest()
		{
			var field = new FieldWithGroups(39, 32);
			BigEmptyBaseTest(field);
			Assert.AreEqual(0, field.DiagonalLinkedGroupsCount);
		}

		[Test]
		public override void UCTBagEmptyBaseTest()
		{
			var field = new FieldWithGroups(39, 32);
			UCTBagEmptyBaseTest(field);
			Assert.AreEqual(0, field.DiagonalLinkedGroupsCount);
		}

		[Test]
		public override void VerylongGameTest()
		{
			var field = new FieldWithGroups(39, 32);
			VerylongGameTest(field);
			Assert.AreEqual(0, field.DiagonalLinkedGroupsCount);
		}

		[Test]
		public void DesintegrationAfterSurrounding()
		{
			int startX = 16;
			int startY = 16;
			var field = new FieldWithGroups(39, 32);

			field.MakeMove(startX, startY);
			field.MakeMove(startX - 1, startY);

			field.MakeMove(startX + 1, startY + 1);
			field.MakeMove(startX, startY + 1);

			field.MakeMove(startX, startY + 2);
			field.MakeMove(startX - 1, startY + 2);

			Assert.AreEqual(1, (int)field.GetDot(startX, startY).GetDiagonalGroupNumber() >> (int)Dot.DiagonalGroupMaskShift);
			Assert.AreEqual(1, (int)field.GetDot(startX + 1, startY + 1).GetDiagonalGroupNumber() >> (int)Dot.DiagonalGroupMaskShift);
			Assert.AreEqual(1, (int)field.GetDot(startX, startY + 2).GetDiagonalGroupNumber() >> (int)Dot.DiagonalGroupMaskShift);

			Assert.AreEqual(2, (int)field.GetDot(startX - 1, startY).GetDiagonalGroupNumber() >> (int)Dot.DiagonalGroupMaskShift);
			Assert.AreEqual(2, (int)field.GetDot(startX, startY + 1).GetDiagonalGroupNumber() >> (int)Dot.DiagonalGroupMaskShift);
			Assert.AreEqual(2, (int)field.GetDot(startX - 1, startY + 2).GetDiagonalGroupNumber() >> (int)Dot.DiagonalGroupMaskShift);

			field.MakeMove(startX - 1, startY + 1);

			Assert.AreEqual(1, (int)field.GetDot(startX - 1, startY + 1).GetDiagonalGroupNumber() >> (int)Dot.DiagonalGroupMaskShift);
			Assert.AreEqual(0, (int)field.GetDot(startX, startY + 1).GetDiagonalGroupNumber() >> (int)Dot.DiagonalGroupMaskShift);

			Assert.AreEqual(3, (int)field.GetDot(startX - 1, startY).GetDiagonalGroupNumber() >> (int)Dot.DiagonalGroupMaskShift);
			Assert.AreEqual(4, (int)field.GetDot(startX - 1, startY + 2).GetDiagonalGroupNumber() >> (int)Dot.DiagonalGroupMaskShift);

			field.UnmakeAllMoves();

			Assert.AreEqual(0, field.DiagonalLinkedGroupsCount);
		}
	}
}
