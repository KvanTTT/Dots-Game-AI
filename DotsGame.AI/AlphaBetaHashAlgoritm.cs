using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotsGame;
using System.Threading;

namespace DotsGame.AI
{
	public class AlphaBetaHashAlgoritm
	{
		#region Fields

		private ushort CurrentMove_;

		#endregion

		#region Constructors

		public AlphaBetaHashAlgoritm(Field field, CMoveGenerator moveGenerator = null, Estimator estimator = null,
			ZobristHashField hashField = null, TranspositionTable transpositionTable = null)
		{
			Field = field;
			MoveGenerator = moveGenerator ?? new MoveGenerator(field);
			Estimator = estimator ?? new Estimator(field);
			if (hashField == null)
				HashField = new ZobristHashField(field, 0);
			else
			{
				HashField = hashField;
				HashField.Field = Field;
			}
			TranspositionTable = transpositionTable ?? new TranspositionTable(field);
		}

		public AlphaBetaHashAlgoritm()
		{
			// TODO: Complete member initialization
		}

		#endregion

		#region Public Methods

		public int SearchBestMove(byte depth = 4)
		{
			return SearchBestMove(depth, Field.CurrentPlayer, -AiSettings.InfinityScore, AiSettings.InfinityScore);
		}

		public int SearchBestMove(byte depth, Dot player, float alpha, float beta)
		{
			int bestMove = 0;

			TranspositionTable.Clear();
			CalculatedPositionCount = 0;

			MoveGenerator.MaxDepth = depth;
			MoveGenerator.GenerateMoves(player, depth);
			Dot nextPlayer = player.NextPlayer();

			foreach (var move in MoveGenerator.Moves)
			{
				if (alpha < beta)
				{
					if (Field.MakeMove(move))
					{
						CalculatedPositionCount++;
						HashField.UpdateHash();
						float tmp = -EvaluatePosition((byte)(depth - 1), nextPlayer, -beta, -alpha, HashField.Key);
						Field.UnmakeMove();
						HashField.UpdateHash();
						if (tmp > alpha)
						{
							alpha = tmp;
							bestMove = move;
						}
					}
				}
			}

			return bestMove;
		}

		#endregion

		#region Helpers

		private float EvaluatePosition(byte depth, Dot player, float alpha, float beta, ulong key)
		{
			float oldAlpha = alpha;

			float score = CheckCollision(player, depth, alpha, beta, key);
			if (score >= 0)
				return score;

			if (depth == 0)
				return Estimator.Estimate(player);

			MoveGenerator.GenerateMoves(player, depth);
			Dot nextPlayer = player.NextPlayer();

			foreach (var move in MoveGenerator.Moves)
			{
				if (Field.MakeMove(move))
				{
					CalculatedPositionCount++;
					HashField.UpdateHash();
					float tmp = -EvaluatePosition((byte)(depth - 1), nextPlayer, -beta, -alpha, HashField.Key);
					Field.UnmakeMove();
					HashField.UpdateHash();

					if (tmp > alpha)
					{
						TranspositionTable.RecordHash(
							(byte)depth, tmp, tmp < beta ? HashEntryData.ExactType : HashEntryData.BetaType, HashField.Key, (ushort)move);

						alpha = tmp;
						if (alpha >= beta)
							return beta;
					}
				}
			}

			if (alpha == oldAlpha)
				TranspositionTable.RecordHash((byte)depth, alpha, HashEntryData.AlphaType, HashField.Key, 0);

			return alpha;
		}

		private unsafe float CheckCollision(Dot player, byte depth, float alpha, float beta, ulong key)
		{
			fixed (HashEntry* hashEntry = &TranspositionTable.HashEntries_[key % AiSettings.HashTableSize])
			{
				if ((Interlocked.Read(ref *(long*)&hashEntry->HashKey) ^
					 Interlocked.Read(ref *(long*)&hashEntry->Data)) == *(long*)&key)
				{
					if (hashEntry->GetDepth() >= depth)
					{
						float score = hashEntry->GetScore();

						if (hashEntry->GetMoveType() == HashEntryData.AlphaType)
						{
							if (score <= alpha)
								return alpha;
							/*if (score < beta)
								beta = score;
							if (beta <= alpha)
								return alpha;*/
						}
						else
						{
							if (score > alpha)
								alpha = score;
							if (alpha >= beta)
								return beta;
						}
					}
					if (hashEntry->GetMoveType() != HashEntryData.AlphaType && depth != 0)
					{
						if (Field.MakeMove(hashEntry->GetBestMove()))
						{
							CalculatedPositionCount++;
							HashField.UpdateHash();
							float tmp = -EvaluatePosition((byte)(depth - 1), player.NextPlayer(), -beta, -alpha, HashField.Key);
							Field.UnmakeMove();
							HashField.UpdateHash();

							if (tmp > alpha)
							{
								TranspositionTable.RecordHash(depth, tmp,
									tmp < beta ? HashEntryData.ExactType : HashEntryData.BetaType, key, hashEntry->GetBestMove());
								alpha = tmp;
								if (alpha >= beta)
									return beta;
							}
						}
					}
				}
			}

			return -1;
		}

		public IEnumerable<HashEntry> NonEmptyHashEntries
		{
			get
			{
				var nonEmptyHashEntries = new List<HashEntry>();
				foreach (var entry in TranspositionTable.HashEntries)
					if (entry.GetMoveType() != HashEntryData.EmptyType)
						nonEmptyHashEntries.Add(entry);
				return nonEmptyHashEntries;
			}
		}

		#endregion

		#region Properties

		public Field Field
		{
			get;
			set;
		}

		public CMoveGenerator MoveGenerator
		{
			get;
			set;
		}

		public Estimator Estimator
		{
			get;
			set;
		}

		public ZobristHashField HashField
		{
			get;
			set;
		}

		public TranspositionTable TranspositionTable
		{
			get;
			set;
		}

		public long CalculatedPositionCount
		{
			get;
			private set;
		}

		#endregion
	}
}
