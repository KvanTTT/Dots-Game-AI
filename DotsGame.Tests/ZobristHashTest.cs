using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using DotsGame;
using DotsGame.AI;

namespace DotsGame.Tests
{
	[TestFixture]
	public class ZobristHashTest
	{
		[Test]
		public void SimpleSequenceTest()
		{
			int startX = 16;
			int startY = 16;

			var field = new Field(39, 32);
			ZobristHashField hash = new ZobristHashField(field, 0);

			ulong initKey = hash.Key;

			field.MakeMove(startX, startY);
			hash.UpdateHash();

			field.MakeMove(startX + 1, startY);
			hash.UpdateHash();

			field.MakeMove(startX + 1, startY + 1);
			hash.UpdateHash();
			
			field.MakeMove(startX, startY + 1);
			hash.UpdateHash();

			while (field.DotsSequanceStates.Count() != 0)
			{
				field.UnmakeMove();
				hash.UpdateHash();
			}

			Assert.AreEqual(hash.Key, initKey);
		}

		[Test]
		public void SimpleBaseTest()
		{
			int startX = 16;
			int startY = 16;
			
			var field = new Field(39, 32);
			ZobristHashField hash = new ZobristHashField(field, 0);

			ulong initKey = hash.Key;

			field.MakeMove(startX, startY);
			hash.UpdateHash();
			field.MakeMove(startX + 1, startY);
			hash.UpdateHash();

			field.MakeMove(startX + 1, startY + 1);
			hash.UpdateHash();
			field.MakeMove(startX + 2, startY + 1);
			hash.UpdateHash();

			field.MakeMove(startX + 1, startY - 1);
			hash.UpdateHash();
			field.MakeMove(startX + 2, startY - 1);
			hash.UpdateHash();

			field.MakeMove(startX + 2, startY);
			hash.UpdateHash();

			while (field.DotsSequanceStates.Count() != 0)
			{
				field.UnmakeMove();
				hash.UpdateHash();
			}

			Assert.AreEqual(hash.Key, initKey);
		}

		[Test]
		public void DifferentBaseCreationOrderTest()
		{
			int startX = 16;
			int startY = 16;

			var field = new Field(39, 32);
			ZobristHashField hash = new ZobristHashField(field, 0);

			ulong initKey = hash.Key;

			field.MakeMove(startX, startY);
			hash.UpdateHash();
			field.MakeMove(startX + 1, startY);
			hash.UpdateHash();

			field.MakeMove(startX + 1, startY + 1);
			hash.UpdateHash();
			field.MakeMove(startX + 2, startY + 1);
			hash.UpdateHash();

			field.MakeMove(startX + 1, startY - 1);
			hash.UpdateHash();
			field.MakeMove(startX + 2, startY - 1);
			hash.UpdateHash();

			field.MakeMove(startX + 2, startY);
			hash.UpdateHash();
			field.MakeMove(startX + 3, startY);
			hash.UpdateHash();

			field.MakeMove(startX - 2, startY);
			hash.UpdateHash();
			field.MakeMove(startX - 2, startY - 1);
			hash.UpdateHash();

			ulong firstHash = hash.Key;

			while (field.DotsSequanceStates.Count() != 0)
			{
				field.UnmakeMove();
				hash.UpdateHash();
			}

			Assert.AreEqual(hash.Key, initKey);


			field.MakeMove(startX - 2, startY);
			hash.UpdateHash();
			field.MakeMove(startX + 1, startY);
			hash.UpdateHash();

			field.MakeMove(startX, startY);
			hash.UpdateHash();
			field.MakeMove(startX + 2, startY + 1);
			hash.UpdateHash();

			field.MakeMove(startX + 1, startY + 1);
			hash.UpdateHash();
			field.MakeMove(startX + 2, startY - 1);
			hash.UpdateHash();

			field.MakeMove(startX + 1, startY - 1);
			hash.UpdateHash();
			field.MakeMove(startX + 3, startY);
			hash.UpdateHash();

			field.MakeMove(startX + 2, startY);
			hash.UpdateHash();
			field.MakeMove(startX - 2, startY - 1);
			hash.UpdateHash();

			ulong secondHash = hash.Key;

			Assert.AreEqual(firstHash, secondHash);

			while (field.DotsSequanceStates.Count() != 0)
			{
				field.UnmakeMove();
				hash.UpdateHash();
			}

			Assert.AreEqual(initKey, hash.Key);
		}

		[Test]
		public void BaseInBaseTest()
		{
			int startX = 5;
			int startY = 2;

			var field = new Field(39, 32);
			ZobristHashField hash = new ZobristHashField(field, 0);

			ulong initKey = hash.Key;

			// center.
			field.MakeMove(startX, startY + 3);
			hash.UpdateHash();
			field.MakeMove(startX, startY + 1);
			hash.UpdateHash();

			field.MakeMove(startX + 10, startY + 1);
			hash.UpdateHash();
			field.MakeMove(startX + 1, startY + 3);
			hash.UpdateHash();

			field.MakeMove(startX + 1, startY + 2);
			hash.UpdateHash();
			field.MakeMove(startX + 10, startY + 2);
			hash.UpdateHash();

			field.MakeMove(startX + 2, startY + 3);
			hash.UpdateHash();
			field.MakeMove(startX + 10, startY + 3);
			hash.UpdateHash();

			field.MakeMove(startX + 1, startY + 4);
			hash.UpdateHash();
			field.MakeMove(startX + 10, startY + 4);
			hash.UpdateHash();

			PutBigBase(field, hash);

			ulong firstHash = hash.Key;

			while (field.DotsSequanceStates.Count() != 0)
			{
				field.UnmakeMove();
				hash.UpdateHash();
			}

			Assert.AreEqual(initKey, hash.Key);

			// center.
			field.MakeMove(startX + 1, startY + 3);
			hash.UpdateHash();
			field.MakeMove(startX, startY + 1);
			hash.UpdateHash();

			field.MakeMove(startX + 10, startY + 1);
			hash.UpdateHash();
			field.MakeMove(startX + 2, startY + 3);
			hash.UpdateHash();

			field.MakeMove(startX + 2, startY + 2);
			hash.UpdateHash();
			field.MakeMove(startX + 10, startY + 2);
			hash.UpdateHash();

			field.MakeMove(startX + 3, startY + 3);
			hash.UpdateHash();
			field.MakeMove(startX + 10, startY + 3);
			hash.UpdateHash();

			field.MakeMove(startX + 4, startY + 4);
			hash.UpdateHash();
			field.MakeMove(startX + 10, startY + 4);
			hash.UpdateHash();

			PutBigBase(field, hash);

			ulong secondHash = hash.Key;

			Assert.AreEqual(firstHash, secondHash);

			while (field.DotsSequanceStates.Count() != 0)
			{
				field.UnmakeMove();
				hash.UpdateHash();
			}

			Assert.AreEqual(initKey, hash.Key);

			startX = 16;
			startY = 16;
		}

		[Test]
		public void EnemyBaseInBaseTest()
		{
			int startX = 5;
			int startY = 2;

			var field = new Field(39, 32);
			ZobristHashField hash = new ZobristHashField(field, 0);

			ulong initKey = hash.Key;

			// center.
			field.MakeMove(startX + 1, startY + 3);
			hash.UpdateHash();
			field.MakeMove(startX, startY + 3);
			hash.UpdateHash();

			field.MakeMove(startX + 10, startY + 2);
			hash.UpdateHash();
			field.MakeMove(startX + 1, startY + 2);
			hash.UpdateHash();

			field.MakeMove(startX + 10, startY + 3);
			hash.UpdateHash();
			field.MakeMove(startX + 2, startY + 2);
			hash.UpdateHash();

			field.MakeMove(startX + 10, startY + 4);
			hash.UpdateHash();
			field.MakeMove(startX + 3, startY + 3);
			hash.UpdateHash();

			field.MakeMove(startX + 10, startY + 5);
			hash.UpdateHash();
			field.MakeMove(startX + 2, startY + 4);
			hash.UpdateHash();

			field.MakeMove(startX + 10, startY + 6);
			hash.UpdateHash();
			field.MakeMove(startX + 1, startY + 4);
			hash.UpdateHash();

			PutBigBase(field, hash);

			ulong firstHash = hash.Key;

			while (field.DotsSequanceStates.Count() != 0)
			{
				field.UnmakeMove();
				hash.UpdateHash();
			}

			Assert.AreEqual(initKey, hash.Key);

			startX = 16;
			startY = 16;
		}

		private void PutBigBase(Field field, ZobristHashField hash)
		{
			int startX = 5;
			int startY = 2;

			// top.
			field.MakeMove(startX, startY);
			hash.UpdateHash();
			field.MakeMove(startX + 11, startY);
			hash.UpdateHash();

			field.MakeMove(startX + 1, startY);
			hash.UpdateHash();
			field.MakeMove(startX + 11, startY + 1);
			hash.UpdateHash();

			field.MakeMove(startX + 2, startY);
			hash.UpdateHash();
			field.MakeMove(startX + 11, startY + 2);
			hash.UpdateHash();

			field.MakeMove(startX + 3, startY);
			hash.UpdateHash();
			field.MakeMove(startX + 11, startY + 3);
			hash.UpdateHash();

			field.MakeMove(startX + 4, startY + 1);
			hash.UpdateHash();
			field.MakeMove(startX + 11, startY + 4);
			hash.UpdateHash();

			// right.
			field.MakeMove(startX + 5, startY + 2);
			hash.UpdateHash();
			field.MakeMove(startX + 12, startY);
			hash.UpdateHash();

			field.MakeMove(startX + 5, startY + 3);
			hash.UpdateHash();
			field.MakeMove(startX + 12, startY + 1);
			hash.UpdateHash();

			field.MakeMove(startX + 5, startY + 4);
			hash.UpdateHash();
			field.MakeMove(startX + 12, startY + 2);
			hash.UpdateHash();

			field.MakeMove(startX + 4, startY + 5);
			hash.UpdateHash();
			field.MakeMove(startX + 12, startY + 3);
			hash.UpdateHash();

			// bottom
			field.MakeMove(startX + 3, startY + 6);
			hash.UpdateHash();
			field.MakeMove(startX + 13, startY);
			hash.UpdateHash();

			field.MakeMove(startX + 2, startY + 6);
			hash.UpdateHash();
			field.MakeMove(startX + 13, startY + 1);
			hash.UpdateHash();

			field.MakeMove(startX + 1, startY + 6);
			hash.UpdateHash();
			field.MakeMove(startX + 13, startY + 2);
			hash.UpdateHash();

			field.MakeMove(startX, startY + 6);
			hash.UpdateHash();
			field.MakeMove(startX + 13, startY + 3);
			hash.UpdateHash();

			field.MakeMove(startX - 1, startY + 5);
			hash.UpdateHash();
			field.MakeMove(startX + 13, startY + 4);
			hash.UpdateHash();

			// left.
			field.MakeMove(startX - 2, startY + 4);
			hash.UpdateHash();
			field.MakeMove(startX + 14, startY);
			hash.UpdateHash();

			field.MakeMove(startX - 2, startY + 3);
			hash.UpdateHash();
			field.MakeMove(startX + 14, startY + 1);
			hash.UpdateHash();

			field.MakeMove(startX - 2, startY + 2);
			hash.UpdateHash();
			field.MakeMove(startX + 14, startY + 2);
			hash.UpdateHash();

			field.MakeMove(startX - 1, startY + 1);
			hash.UpdateHash();
			field.MakeMove(startX + 14, startY + 3);
			hash.UpdateHash();
		}

		[Test]
		public void EmptyBaseTest()
		{
			int startX = 16;
			int startY = 16;

			var field = new Field(39, 32);
			ZobristHashField hash = new ZobristHashField(field, 0);

			ulong initKey = hash.Key;

			field.MakeMove(startX, startY);
			hash.UpdateHash();
			field.MakeMove(startX + 10, startY);
			hash.UpdateHash();

			field.MakeMove(startX + 1, startY + 1);
			hash.UpdateHash();
			field.MakeMove(startX + 10, startY + 1);
			hash.UpdateHash();

			field.MakeMove(startX + 2, startY + 1);
			hash.UpdateHash();
			field.MakeMove(startX + 10, startY + 2);
			hash.UpdateHash();

			field.MakeMove(startX + 3, startY);
			hash.UpdateHash();
			field.MakeMove(startX + 10, startY + 3);
			hash.UpdateHash();

			field.MakeMove(startX + 2, startY - 1);
			hash.UpdateHash();
			field.MakeMove(startX + 10, startY + 4);
			hash.UpdateHash();

			field.MakeMove(startX + 1, startY - 1);
			hash.UpdateHash();
			field.MakeMove(startX + 1, startY);
			hash.UpdateHash();

			while (field.DotsSequanceStates.Count() != 0)
			{
				field.UnmakeMove();
				hash.UpdateHash();
			}

			Assert.AreEqual(initKey, hash.Key);
		}
		public void VeryulongGameTest()
		{
			var field = new Field(39, 32);
			ZobristHashField hash = new ZobristHashField(field, 0);

			ulong initKey = hash.Key;

			var buffer = DotsGame.Tests.Properties.Resources.VeryLongGame;

			for (var i = 58; i < buffer.Length; i += 13)
			{
				Assert.AreEqual(field.MakeMove(buffer[i] + 1, buffer[i + 1] + 1), true);
				hash.UpdateHash();
			}

			while (field.DotsSequanceStates.Count() != 0)
			{
				field.UnmakeMove();
				hash.UpdateHash();
			}

			Assert.AreEqual(initKey, hash.Key);
		}
	}
}
