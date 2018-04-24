using System.Collections.Generic;
using System.Threading;

namespace DotsGame.AI
{
    public class TranspositionTable
	{
		#region Fields

		private Field Field_;
		public static HashEntry[] HashEntries_;

		#endregion

		#region Constructors

		static TranspositionTable()
		{
			HashEntries_ = new HashEntry[AiSettings.HashTableSize];
		}

		public TranspositionTable(Field field)
		{
			Field_ = field;
		}

		#endregion

		#region Public

		public unsafe void RecordHash(byte depth, float score, HashEntryData type, ulong key, ushort move)
		{
			fixed (HashEntry* entry = &HashEntries_[key % AiSettings.HashTableSize])
			{
				var entryType = entry->GetMoveType();

				if (type == HashEntryData.AlphaType &&
					(entryType == HashEntryData.ExactType || entryType == HashEntryData.BetaType))
					return;

				if (entry->GetDepth() <= depth)
				{
					ulong data = HashEntry.PackData(move, score, depth, type);

					Interlocked.Exchange(ref *(long*)&entry->HashKey, (long)(key ^ data));
					Interlocked.Exchange(ref *(long*)&entry->Data, (long)data);
				}
			}
		}

		public void RecordRealHash(float score, ulong key)
		{
		}

		public static void Clear()
		{
			for (int i = 0; i < HashEntries_.Length; i++)
				HashEntries_[i].Data = 0;
		}

		public IEnumerable<HashEntry> HashEntries
		{
			get
			{
				return HashEntries_;
			}
		}

		#endregion
	}
}