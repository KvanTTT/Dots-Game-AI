using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotsGame.AI
{
	public class MovesSequence
	{
		public List<short> ChainPositions;
		public List<short> SurroundedPositions;
		public List<short> CapturePositions;
	}

	public class ForcedAttackMoveGenerator : CMoveGenerator
	{
		struct GroupsPosition
		{
			public int Position;
			public List<int> Groups;
		}

		public ForcedAttackMoveGenerator(FieldWithGroups field) 
			: base(field)
		{
		}

		public override void GenerateMoves(Dot player, int depth = 0)
		{
			throw new NotImplementedException();
		}

		public override void UpdateMoves()
		{
			throw new NotImplementedException();
		}

		public void ForcedAttack(Dot player)
		{
			var enemyPlayer = player.NextPlayer();

			int bestMove = 0;
			float score = 0;
			int depth = 0;


			var capturesMovesSequences = FindCapturesMoves(player, 2);
			foreach (var seq in capturesMovesSequences)
			{



				Field.MakeMove(seq.CapturePositions[0], player);

				Field.MakeMove(seq.CapturePositions[1], player.NextPlayer());

				var surroundMoves = FindCapturesMoves(player);

				Field.UnmakeMove();

				Field.UnmakeMove();
			}
		}
		
		public List<MovesSequence> FindCapturesMoves(Dot player, int depth = 1)
		{
			if (depth == 1)
				return FindCapturesMovesDepthOne(player);
			else if (depth == 2)
				return FindCapturesMovesDepthTwo(player);
			throw new NotImplementedException();
		}

		public List<MovesSequence> FindDefenceMoves(Dot player)
		{
			return FindCapturesMoves(player.NextPlayer(), 1);
		}

		private List<MovesSequence> FindCapturesMovesDepthOne(Dot player)
		{
			var result = new List<MovesSequence>();
			var visitedPositions = new HashSet<int>();

			for (int i = 0; i < Field.DotsSequenceCount; i++)
			{
				var state = Field.GetState(i);
				if (Field[state.Move.Position].IsPlayerPutted(player))
					foreach (var d in Field.DiagDeltas)
					{
						var pos = Field.GetState(i).Move.Position + d;
						if (!visitedPositions.Contains(pos))
						{
							var inputDots = Field.GetInputDots(pos, player);
							if (Field[pos].IsNotPutted() && IsConnectIdenticalGroups(inputDots))
							{
								Field.MakeMove(pos, player);
								if (Field.LastMoveCaptureCount > 0)
									result.Add(new MovesSequence()
									{
										CapturePositions = new List<short> { (short)pos },
										SurroundedPositions = Field.LastState.Base.SurroundPoistions,
										ChainPositions = Field.LastState.Base.ChainPositions
									});
								Field.UnmakeMove();
							}

							visitedPositions.Add(pos);
						}
					}
			}

			return result;
		}

		private List<MovesSequence> FindCapturesMovesDepthTwo(Dot player)
		{
			var result = new List<MovesSequence>();
			var visitedPositions = new HashSet<int>();

			List<GroupsPosition> groupsPositions = new List<GroupsPosition>();

			for (int i = 0; i < Field.DotsSequenceCount; i++)
			{
				var state = Field.GetState(i);
				if (Field[state.Move.Position].IsPlayerPutted(player))
					foreach (var d in Field.DiagDeltas)
					{
						var pos = Field.GetState(i).Move.Position + d;
						if (!visitedPositions.Contains(pos))
						{
							if (Field[pos].IsNotPutted())
							{
								var inputDots = Field.GetInputDots(pos, player); // count must be always greather or equal to 1
								if (inputDots.Count == 1)
								{
									Field.MakeMove(pos, player);

									var inputDot = inputDots[0];
									var inputDotCCW = inputDots[0];
									Field.GetFirstNextPos(pos, ref inputDot);
									Field.GetFirstNextPosCCW(pos, ref inputDotCCW);

									do
									{
										var inputDots2 = Field.GetInputDots(inputDot, player);
										if (Field[inputDot].IsNotPutted() && IsConnectIdenticalGroups(inputDots2))
										{
											Field.MakeMove(inputDot, player);
											if (Field.LastMoveCaptureCount > 0)
											{
												var capturePositions = new List<short> { (short)pos, (short)inputDot };
												if (result.Where(item => item.CapturePositions.Except(capturePositions).Count() == 0).Count() == 0)
													result.Add(new MovesSequence
													{
														CapturePositions = capturePositions,
														SurroundedPositions = new List<short>(Field.LastState.Base.SurroundPoistions),
														ChainPositions = new List<short>(Field.LastState.Base.ChainPositions)
													});
											}
											Field.UnmakeMove();
										}

										Field.GetNextPos(pos, ref inputDot);
									}
									while (inputDot != inputDotCCW);

									Field.UnmakeMove();
								}
								else if (!IsConnectIdenticalGroups(inputDots))
								{
									groupsPositions.Add(new GroupsPosition
									{
										Position = pos,
										Groups = inputDots.Select(inputDot => (int)Field[inputDot].GetDiagGroupNumber()).ToList()
									});
								}
								else
								{

								}
							}

							visitedPositions.Add(pos);
						}
					}
			}

			for (int i = 0; i < groupsPositions.Count; i++)
				for (int j = i + 1; j < groupsPositions.Count; j++)
				{
					if (groupsPositions[i].Groups.Union(groupsPositions[j].Groups).Count() ==
						Math.Max(groupsPositions[i].Groups.Count, groupsPositions[j].Groups.Count))
					{
						Field.MakeMove(groupsPositions[i].Position, player);
						Field.MakeMove(groupsPositions[j].Position, player);

						if (Field.LastMoveCaptureCount > 0)
						{
							var capturePositions = new List<short> { (short)groupsPositions[i].Position, (short)groupsPositions[j].Position };
							if (result.Where(item => item.CapturePositions.Except(capturePositions).Count() == 0).Count() == 0)
								result.Add(new MovesSequence
								{
									CapturePositions = capturePositions,
									SurroundedPositions = new List<short>(Field.LastState.Base.SurroundPoistions),
									ChainPositions = new List<short>(Field.LastState.Base.ChainPositions)
								});
						}

						Field.UnmakeMove();
						Field.UnmakeMove();
					}
				}

			return result;
		}

		private bool IsConnectIdenticalGroups(List<int> inputDots)
		{
			if (inputDots.Count <= 1)
				return false;
			for (int i = 0; i < inputDots.Count; i++)
			{
				var groupNumber = Field[inputDots[i]].GetDiagGroupNumber();
				for (int j = i + 1; j < inputDots.Count; j++)
					if (groupNumber != Field[inputDots[j]].GetDiagGroupNumber())
						return false;
			}
			return true;
		}
	}
}
