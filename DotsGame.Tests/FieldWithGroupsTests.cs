using NUnit.Framework;

namespace DotsGame.Tests
{
    [TestFixture]
    public class FieldWithGroupsTests : FieldTests
    {
        [Test]
        public override void Play_SimpleSequence()
        {
            var field = new FieldWithGroups(39, 32);
            SimpleSequenceTest(field);
            Assert.AreEqual(0, field.DiagonalLinkedGroupsCount);
        }

        [Test]
        public override void Play_BlueSurroundFirst()
        {
            var field = new FieldWithGroups(39, 32);
            BlueSurroundFirstTest(field);
            Assert.AreEqual(0, field.DiagonalLinkedGroupsCount);
        }

        [Test]
        public override void Play_OneBase()
        {
            var field = new FieldWithGroups(39, 32);
            OneBaseTest(field);
            Assert.AreEqual(0, field.DiagonalLinkedGroupsCount);
        }

        [Test]
        public override void Play_BaseInBase()
        {
            var field = new FieldWithGroups(39, 32);
            BaseInBaseTest(field);
            Assert.AreEqual(0, field.DiagonalLinkedGroupsCount);
        }

        [Test]
        public override void Play_ComplexBaseInBase()
        {
            var field = new FieldWithGroups(39, 32);
            ComplexBaseInBaseTest(field);
            Assert.AreEqual(0, field.DiagonalLinkedGroupsCount);
        }

        [Test]
        public override void Play_EmptyBase()
        {
            var field = new FieldWithGroups(39, 32);
            EmptyBaseTest(field);
            Assert.AreEqual(0, field.DiagonalLinkedGroupsCount);
        }

        [Test]
        public override void Play_BlueSurroundFirstInEmptyBase()
        {
            var field = new FieldWithGroups(39, 32);
            BlueSurroundFirstInEmptyBase(field);
            Assert.AreEqual(0, field.DiagonalLinkedGroupsCount);
        }

        [Test]
        public override void Play_ThreeAdjacentBases()
        {
            var field = new FieldWithGroups(39, 32);
            ThreeAdjacentBasesTest(field);
            Assert.AreEqual(0, field.DiagonalLinkedGroupsCount);
        }

        [Test]
        public override void Play_BigEmptyBase()
        {
            var field = new FieldWithGroups(39, 32);
            BigEmptyBaseTest(field);
            Assert.AreEqual(0, field.DiagonalLinkedGroupsCount);
        }

        [Test]
        [Ignore("Not implemented with groups")]
        public override void Play_UCTBagEmptyBase()
        {
            var field = new FieldWithGroups(39, 32);
            UCTBagEmptyBaseTest(field);
            Assert.AreEqual(0, field.DiagonalLinkedGroupsCount);
        }

        [Test]
        public override void Play_VerylongGame()
        {
            var field = new FieldWithGroups(39, 32);
            VerylongGameTest(field);
            Assert.AreEqual(0, field.DiagonalLinkedGroupsCount);
        }

        [Test]
        public void Play_DesintegrationAfterSurrounding()
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

            Assert.AreEqual(1, (int)field.GetDot(startX, startY).GetDiagGroupNumber() >> (int)DotState.DiagonalGroupMaskShift);
            Assert.AreEqual(1, (int)field.GetDot(startX + 1, startY + 1).GetDiagGroupNumber() >> (int)DotState.DiagonalGroupMaskShift);
            Assert.AreEqual(1, (int)field.GetDot(startX, startY + 2).GetDiagGroupNumber() >> (int)DotState.DiagonalGroupMaskShift);

            Assert.AreEqual(2, (int)field.GetDot(startX - 1, startY).GetDiagGroupNumber() >> (int)DotState.DiagonalGroupMaskShift);
            Assert.AreEqual(2, (int)field.GetDot(startX, startY + 1).GetDiagGroupNumber() >> (int)DotState.DiagonalGroupMaskShift);
            Assert.AreEqual(2, (int)field.GetDot(startX - 1, startY + 2).GetDiagGroupNumber() >> (int)DotState.DiagonalGroupMaskShift);

            field.MakeMove(startX - 1, startY + 1);

            Assert.AreEqual(1, (int)field.GetDot(startX - 1, startY + 1).GetDiagGroupNumber() >> (int)DotState.DiagonalGroupMaskShift);
            Assert.AreEqual(1, (int)field.GetDot(startX, startY + 1).GetDiagGroupNumber() >> (int)DotState.DiagonalGroupMaskShift);

            Assert.AreEqual(3, (int)field.GetDot(startX - 1, startY).GetDiagGroupNumber() >> (int)DotState.DiagonalGroupMaskShift);
            Assert.AreEqual(4, (int)field.GetDot(startX - 1, startY + 2).GetDiagGroupNumber() >> (int)DotState.DiagonalGroupMaskShift);

            field.UnmakeAllMoves();

            Assert.AreEqual(0, field.DiagonalLinkedGroupsCount);
        }
    }
}
