using System;
using System.Collections.Generic;
using System.Linq;

namespace DotsGame.AI
{
    public class MovesSequence
    {
        public List<short> ChainPositions;
        public List<short> SurroundedPositions;
        public List<short> CapturePositions;

        public bool Continious;
    }

    public class ForcedAttackMoveGenerator : MoveGenerator
    {
        struct GroupsPosition
        {
            public int Position;
            public List<int> Groups;
        }

        public ForcedAttackMoveGenerator(FieldWithGroups field, Estimator estimator = null)
            : base(field)
        {
            Estimator = estimator ?? new Estimator(field);
            HashEntries = new List<ulong>(Field.DotsSequenceCount * 2);
        }

        public override void GenerateMoves(DotState player, int depth = 0)
        {
            HashEntries.Clear();
            var curPlayer = Field.CurrentPlayer;

            var capturesMovesSequences = FindCapturesMoves(player, 2, null, false);
            foreach (var seq in capturesMovesSequences)
            {
                ForcedAttack(player, seq, seq, 0, false, false);
                ForcedAttack(player, seq, seq, 0, true, true);
            }

            foreach (var searchedMove in HashEntries)
            {
                var move = searchedMove.GetMove();
                if (!Moves.Contains(move))
                    Moves.Add(move);
            }

            Field.CurrentPlayer = curPlayer;
        }

        private void ForcedAttack(DotState player, MovesSequence firstMovesSequence, MovesSequence curMovesSequence, int depth, bool reversed, bool firstReversed)
        {
            int ind = reversed ? 1 : 0;
            int ind2 = 1 - ind;
            Field.MakeMove(curMovesSequence.CapturePositions[ind], player);

            bool enemySurround = false;
            /*var enemyCaptureMoveSequences = FindCapturesMoves(player.NextPlayer(), 1, curMovesSequence.ChainPositions);
			foreach (var enemyCaptureMove in enemyCaptureMoveSequences)
			{
				foreach (var chainPos in curMovesSequence.ChainPositions)
					if (enemyCaptureMove.SurroundedPositions.Contains(chainPos))
					{
						enemySurround = true;
						break;
					}
			}*/

            if (!enemySurround)
            {
                Field.MakeMove(curMovesSequence.CapturePositions[ind2], player.NextPlayer());

                var newMoveSequences = FindCapturesMoves(player, 2,
                    curMovesSequence.Continious ?
                    new List<short>() { curMovesSequence.CapturePositions[ind] } :
                    new List<short>() { curMovesSequence.CapturePositions[ind], curMovesSequence.CapturePositions[ind2] });
                foreach (var moveSequence in newMoveSequences)
                {
                    if (moveSequence.CapturePositions.Count == 1)
                    {
                        Field.MakeMove(moveSequence.CapturePositions[0], player);

                        if (Field.LastMoveCaptureCount > 0 &&
                            Field.LastState.Base.SurroundPositions.Contains(firstMovesSequence.SurroundedPositions[0]))
                        {
                            HashEntries.Add(HashEntry.PackData(
                                (ushort)firstMovesSequence.CapturePositions[firstReversed ? 1 : 0],
                                Estimator.Estimate(player),
                                (byte)(depth + 3)));
                        }

                        Field.UnmakeMove();
                    }
                    else
                    {
                        ForcedAttack(player, firstMovesSequence, moveSequence, depth + 2, false, firstReversed);
                        ForcedAttack(player, firstMovesSequence, moveSequence, depth + 2, true, firstReversed);
                    }
                }

                Field.UnmakeMove();
            }

            Field.UnmakeMove();
        }

        public List<MovesSequence> FindCapturesMoves(DotState player, int depth = 1, List<short> centerPositions = null, bool findWithLessDepth = true)
        {
            if (depth == 1)
                return FindCapturesMovesDepthOne(player, centerPositions);
            else if (depth == 2)
                return FindCapturesMovesDepthTwo(player, findWithLessDepth, centerPositions);
            throw new NotImplementedException();
        }

        public List<MovesSequence> FindDefenceMoves(DotState player)
        {
            return FindCapturesMoves(player.NextPlayer(), 1, null);
        }

        private List<MovesSequence> FindCapturesMovesDepthOne(DotState player, List<short> centerPositions)
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
                            if ((centerPositions != null ? (centerPositions.Any(cp => Field.Distance(pos, cp) < 2)) : true)
                                && Field[pos].IsNotPutted())
                            {
                                var inputDots = Field.GetInputDots(pos, player);
                                if (HasIdenticalGroups(inputDots))
                                {
                                    Field.MakeMove(pos, player);
                                    if (Field.LastMoveCaptureCount > 0)
                                        result.Add(new MovesSequence()
                                        {
                                            CapturePositions = new List<short> { (short)pos },
                                            SurroundedPositions = new List<short>(Field.LastState.Base.SurroundPositions),
                                            ChainPositions = new List<short>(Field.LastState.Base.ChainPositions),
                                            Continious = true
                                        });
                                    Field.UnmakeMove();
                                }
                            }

                            visitedPositions.Add(pos);
                        }
                    }
            }

            return result;
        }

        private List<MovesSequence> FindCapturesMovesDepthTwo(DotState player, bool findWithLessDepth, List<short> centerPositions)
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
                            if ((centerPositions != null ? (centerPositions.Any(cp => Field.Distance(pos, cp) < 3)) : true)
                                && Field[pos].IsNotPutted())
                            {
                                var inputDots = Field.GetInputDots(pos, player); // count must be always greather or equal to 1
                                if (inputDots.Count > 0)
                                {
                                    Field.MakeMove(pos, player);

                                    var inputDot = inputDots[0];
                                    var inputDotCCW = inputDots[0];
                                    Field.GetFirstNextPos(pos, ref inputDot);
                                    Field.GetFirstNextPosCCW(pos, ref inputDotCCW);

                                    do
                                    {
                                        if ((centerPositions != null ? (centerPositions.Any(cp => Field.Distance(inputDot, cp) < 3)) : true)
                                            && Field[inputDot].IsNotPutted())
                                        {
                                            var inputDots2 = Field.GetInputDots(inputDot, player);
                                            if (HasIdenticalGroups(inputDots2))
                                            {
                                                Field.MakeMove(inputDot, player);
                                                if (Field.LastMoveCaptureCount > 0)
                                                {
                                                    var capturePositions = new List<short> { (short)pos, (short)inputDot };
                                                    if (result.Where(item => item.CapturePositions.Except(capturePositions).Count() == 0).Count() == 0)
                                                        result.Add(new MovesSequence
                                                        {
                                                            CapturePositions = capturePositions,
                                                            SurroundedPositions = new List<short>(Field.LastState.Base.SurroundPositions),
                                                            ChainPositions = new List<short>(Field.LastState.Base.ChainPositions),
                                                            Continious = true
                                                        });
                                                }
                                                Field.UnmakeMove();
                                            }
                                        }

                                        Field.GetNextPos(pos, ref inputDot);
                                    }
                                    while (inputDot != inputDotCCW);

                                    Field.UnmakeMove();
                                }

                                if (HasIdenticalGroups(inputDots))
                                {
                                    if (Field[pos].IsNotPutted() && findWithLessDepth)
                                    {
                                        Field.MakeMove(pos, player);
                                        if (Field.LastMoveCaptureCount > 0)
                                            result.Add(new MovesSequence()
                                            {
                                                CapturePositions = new List<short> { (short)pos },
                                                SurroundedPositions = new List<short>(Field.LastState.Base.SurroundPositions),
                                                ChainPositions = new List<short>(Field.LastState.Base.ChainPositions),
                                                Continious = true
                                            });
                                        Field.UnmakeMove();
                                    }
                                }
                                else if (inputDots.Count > 1)
                                    groupsPositions.Add(new GroupsPosition
                                    {
                                        Position = pos,
                                        Groups = inputDots.Select(dot => (int)Field[dot].GetDiagGroupNumber()).ToList()
                                    });
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
                                    SurroundedPositions = new List<short>(Field.LastState.Base.SurroundPositions),
                                    ChainPositions = new List<short>(Field.LastState.Base.ChainPositions),
                                    Continious = false
                                });
                        }

                        Field.UnmakeMove();
                        Field.UnmakeMove();
                    }
                }

            return result;
        }

        private bool HasIdenticalGroups(List<int> inputDots)
        {
            if (inputDots.Count <= 1)
                return false;
            else if (inputDots.Count == 2)
                return Field[inputDots[0]].GetDiagGroupNumber() == Field[inputDots[1]].GetDiagGroupNumber();
            else
                for (int i = 0; i < inputDots.Count; i++)
                {
                    var groupNumber = Field[inputDots[i]].GetDiagGroupNumber();
                    for (int j = i + 1; j < inputDots.Count; j++)
                        if (groupNumber == Field[inputDots[j]].GetDiagGroupNumber())
                            return true;
                }

            return false;
        }

        #region Properties

        public Estimator Estimator
        {
            get;
            private set;
        }

        #endregion
    }
}
