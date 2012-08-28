﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotsGame
{
	public class FieldWithGroups : Field
	{
		#region Helpers

		protected override void CheckClosure()
		{
			if (_dots[LastPosition].IsInEmptyBase())
				CheckForEmptyBase();
			else
				ProcessLastDot(LastPosition);
		}

		protected override void CheckForEmptyBase()
		{
			// Check player of territoty.
			var chainPosition = LastPosition - 1;
			while (!_dots[chainPosition].IsPutted())
				chainPosition--;

			// If put in own empty base.
			if ((_dots[chainPosition] & Dot.Player) == (_dots[LastPosition] & Dot.Player))
			{
				// Подумать, т.к. здесь могут быть сложности.
				_dots[LastPosition] &= ~Dot.EmptyBase;
				int groupNumber;
				if (GetInputDots(LastPosition) == 0)
				{
					if (_dots[LastPosition].GetEnabledCondition() != _dots[LastPosition - 1].GetEnabledCondition())
					{
						// Add new group
						DiagonalLinkedGroupsCount++;
						groupNumber = DiagonalLinkedGroupsCount << (int)Dot.DiagonalGroupMaskShift;
					}
					else
						groupNumber = (int)_dots[LastPosition - 1].GetDiagonalGroupNumber();
					_dots[LastPosition] = _dots[LastPosition].SetDiagonalGroupNumber(groupNumber);
				}
				else
				{
					groupNumber = GetMinimalGroupNumber();
					FillWithNewGroupNumber(LastPosition, groupNumber);
				}
				return;
			}

			if (SurroundCondition == enmSurroundCondition.Standart)
			{
				if (GetInputDots(LastPosition) > 1)
					ProcessLastDot(LastPosition);
				// If surround anything.
				if (_chainPositions.Count != 0)
				{
					RemoveEmptyBaseFlags(LastPosition);
					return;
				}
			}

			// Find first opponent dot.
			chainPosition++;
			var opponentEnableCondition = Dot.Putted | CurrentPlayer.NextPlayer();
			var nextPlayer = CurrentPlayer.NextPlayer();
			do
			{
				chainPosition--;
				while (!_dots[chainPosition].IsEnable(opponentEnableCondition))
					chainPosition--;

				if (GetInputDots(chainPosition) > 1)
				{
					_chainPositions.Clear();
					_surroundPositions.Clear();
					_chainDotsPositions.Clear();
					_surroundDotsPositions.Clear();

					ProcessLastDot(chainPosition);
				}
			}
			while (_dots[LastPosition].IsZeroSurroundLevel());

			LastMoveCaptureCount = -1;
		}

		private void ProcessLastDot(int pos)
		{
			int groupNumber = 0;
			var inputDotsCount = GetInputDots(pos);
			if (inputDotsCount == 0)
			{
				// ?o?
				// oxo
				// ?o?
				if (_dots[pos].GetEnabledCondition() != _dots[pos - 1].GetEnabledCondition())
				{
					// Add new group
					DiagonalLinkedGroupsCount++;
					groupNumber = DiagonalLinkedGroupsCount << (int)Dot.DiagonalGroupMaskShift;
				}
				else
					groupNumber = (int)_dots[pos - 1].GetDiagonalGroupNumber();
				_dots[pos] = _dots[pos].SetDiagonalGroupNumber(groupNumber);
			}
			else if (inputDotsCount == 1)
			{
				// Append to existing group
				groupNumber = (int)_dots[_inputChainDots[0]].GetDiagonalGroupNumber();
				_dots[pos] = _dots[pos].SetDiagonalGroupNumber(groupNumber);
			}
			else
			{
				groupNumber = GetMinimalGroupNumber();
				FindAndMarkChainAndSurroundedDots(pos, groupNumber);
				//FillWithNewGroupNumber(pos, groupNumber);
			}
		}

		protected void FindAndMarkChainAndSurroundedDots(int position, int newGroup)
		{
			int pos;
			var inputDotsCount = _inputChainDots.Count;

			var dotColor = _dots[position] & Dot.Player;
			var enabledCondition = _dots[position].GetEnabledCondition();
			Dot enemyPlayer = CurrentPlayer.NextPlayer();
			bool negativeSquare = true;

			List<int> negativeSquareDotsPositions = new List<int>();

			for (var i = 0; i < _inputChainDots.Count; i++)
			{
				var previousChainDotsCount = _chainPositions.Count;
				_chainPositions.Add(position);
				pos = _inputChainDots[i];
				var centerPos = position;

				// Returns square bounded by the triangle with vertexes (0, centerPos, pos)
				int tempSquare = GetSquare(centerPos, pos);
				do
				{
					/*if ((_chainPositions.Count > previousChainDotsCount + 1) && (pos == _chainPositions[_chainPositions.Count - 2]))
						_chainPositions.RemoveAt(_chainPositions.Count - 1);
					else*/
					_chainPositions.Add(pos);

					int t = pos;
					pos = centerPos;
					centerPos = t;

					GetFirstNextPos(centerPos, ref pos);
					while (!_dots[pos].IsEnable(enabledCondition))
						GetNextPos(centerPos, ref pos);

					tempSquare += GetSquare(centerPos, pos);
				}
				while (pos != position);

				// Bypass territory only couter-clockwise (territiry with negative square)
				if (tempSquare <= 0)
				{
					for (var j = previousChainDotsCount; j < _chainPositions.Count; j++)
						negativeSquareDotsPositions.Add(_chainPositions[j]);

					/*for (var j = previousChainDotsCount; j < _chainPositions.Count; j++)
					{
						_chainDotsPositions.Add(new DotPosition(_chainPositions[j], _dots[_chainPositions[j]]));
						_dots[_chainPositions[j]] = _dots[_chainPositions[j]].SetDiagonalGroupNumber(newGroup);
					}
					//negativeSquare = false;*/
					_chainPositions.RemoveRange(previousChainDotsCount, _chainPositions.Count - previousChainDotsCount);
				}
				else
				{
					var crosswiseGroups = new Dictionary<int, List<int[]>>();
					int groupNumber;

					for (var j = previousChainDotsCount; j < _chainPositions.Count; j++)
					{
						// Save chain dot states for further rollback.
						_chainDotsPositions.Add(new DotPosition(_chainPositions[j], _dots[_chainPositions[j]]));

						// Mark surrounded dots by flag "Bound".
						_dots[_chainPositions[j]] |= Dot.Bound;
						//_dots[_chainPositions[j]] = _dots[_chainPositions[j]].SetDiagonalGroupNumber(newGroup);
						_dots[_chainPositions[j]] &= ~Dot.DiagonalGroupMask;

						var chainPositionJMinus1 = j > previousChainDotsCount ?
							_chainPositions[j - 1] :
							_chainPositions[_chainPositions.Count - 1];
						int dif = _chainPositions[j] - chainPositionJMinus1;

						if (dif == Field.RealWidth - 1)
						{
							if (_dots[chainPositionJMinus1 - 1].IsPlayerPutted(enemyPlayer) &&
								_dots[chainPositionJMinus1 + Field.RealWidth].IsPlayerPutted(enemyPlayer))
							{
								groupNumber = (int)_dots[chainPositionJMinus1 - 1].GetDiagonalGroupNumber();
								if (!crosswiseGroups.ContainsKey(groupNumber))
									crosswiseGroups.Add(groupNumber, new List<int[]>() { new int[] { chainPositionJMinus1, _chainPositions[j], chainPositionJMinus1 - 1 } });
								else
									crosswiseGroups[groupNumber].Add(new int[] { chainPositionJMinus1, _chainPositions[j], chainPositionJMinus1 - 1 });
							}
						}
						else if (dif == Field.RealWidth + 1)
						{
							if (_dots[chainPositionJMinus1 + 1].IsPlayerPutted(enemyPlayer) &&
								_dots[chainPositionJMinus1 + Field.RealWidth].IsPlayerPutted(enemyPlayer))
							{
								groupNumber = (int)_dots[chainPositionJMinus1 + 1].GetDiagonalGroupNumber();
								if (!crosswiseGroups.ContainsKey(groupNumber))
									crosswiseGroups.Add(groupNumber, new List<int[]>() { new int[] { chainPositionJMinus1, _chainPositions[j], chainPositionJMinus1 + Field.RealWidth } });
								else
									crosswiseGroups[groupNumber].Add(new int[] { chainPositionJMinus1, _chainPositions[j], chainPositionJMinus1 + Field.RealWidth });
							}
						}
						else if (dif == -Field.RealWidth + 1)
						{
							if (_dots[chainPositionJMinus1 + 1].IsPlayerPutted(enemyPlayer) &&
								_dots[chainPositionJMinus1 - Field.RealWidth].IsPlayerPutted(enemyPlayer))
							{
								groupNumber = (int)_dots[chainPositionJMinus1 + 1].GetDiagonalGroupNumber();
								if (!crosswiseGroups.ContainsKey(groupNumber))
									crosswiseGroups.Add(groupNumber, new List<int[]>() { new int[] { chainPositionJMinus1, _chainPositions[j], chainPositionJMinus1 + 1 } });
								else
									crosswiseGroups[groupNumber].Add(new int[] { chainPositionJMinus1, _chainPositions[j], chainPositionJMinus1 + 1 });
							}
						}
						else if (dif == -Field.RealWidth - 1)
						{
							if (_dots[chainPositionJMinus1 - 1].IsPlayerPutted(enemyPlayer) &&
								_dots[chainPositionJMinus1 - Field.RealWidth].IsPlayerPutted(enemyPlayer))
							{
								groupNumber = (int)_dots[chainPositionJMinus1 - 1].GetDiagonalGroupNumber();
								if (!crosswiseGroups.ContainsKey(groupNumber))
									crosswiseGroups.Add(groupNumber, new List<int[]>() { new int[] { chainPositionJMinus1, _chainPositions[j], chainPositionJMinus1 - Field.RealWidth } });
								else
									crosswiseGroups[groupNumber].Add(new int[] { chainPositionJMinus1, _chainPositions[j], chainPositionJMinus1 - Field.RealWidth });
							}
						}
					}

					var previousSuroundDotsCount = _surroundPositions.Count;

					// Find array of dots, bounded by ChainPositions
					AddCapturedDots(_inputSurroundedDots[i], dotColor);

					// Changing "RedCapturedCount" and "BlackCaptureCount".
					AddCapturedFreedCount(dotColor);

					// If capture not empty base or turned on an option "Surround all".
					if ((LastMoveCaptureCount != 0) || (SurroundCondition == enmSurroundCondition.Always))
					{
						for (var j = previousSuroundDotsCount; j < _surroundPositions.Count; j++)
						{
							// Clear tag.
							_dots[_surroundPositions[j]] &= ~Dot.Tagged;

							// Save surrounded dot states for further rollback.
							_surroundDotsPositions.Add(new DotPosition(_surroundPositions[j], _dots[_surroundPositions[j]]));

							_dots[_surroundPositions[j]] &= ~Dot.DiagonalGroupMask;
							SetCaptureFreeState(_surroundPositions[j], dotColor);
						}
					}
					// If capture empty base.
					else
					{
						for (var j = previousChainDotsCount; j < _chainPositions.Count; j++)
						{
							// Clear "Bound" flag and set special flag "EmptyBound".
							_dots[_chainPositions[j]] &= ~Dot.Bound;
							_dots[_chainPositions[j]] |= Dot.EmptyBound;
						}

						for (var j = previousSuroundDotsCount; j < _surroundPositions.Count; j++)
						{
							// Clear tag.
							_dots[_surroundPositions[j]] &= ~Dot.Tagged;

							// Save surrounded dot states for further rollback.
							_surroundDotsPositions.Add(new DotPosition(_surroundPositions[j], _dots[_surroundPositions[j]]));

							_dots[_surroundPositions[j]] &= ~Dot.DiagonalGroupMask;
							// If dot is not putted in empty territory then set flag "EmptyBase"
							if ((_dots[_surroundPositions[j]] & Dot.Putted) == 0)
								_dots[_surroundPositions[j]] |= Dot.EmptyBase;
						}

						if (dotColor == Dot.RedPlayer)
							RedSquare -= _lastSquareCaptureCount;
						else
							BlueSquare -= _lastSquareCaptureCount;

						_chainPositions.RemoveRange(previousChainDotsCount, _chainPositions.Count - previousChainDotsCount);
						_surroundPositions.RemoveRange(previousSuroundDotsCount, _surroundPositions.Count - previousSuroundDotsCount);
					}

					foreach (var crosswiseGroup in crosswiseGroups)
					{
						if (crosswiseGroup.Value.Count > 1)
						{
							// disintegration
							foreach (var crosswise in crosswiseGroup.Value)
							{
								DiagonalLinkedGroupsCount++;
								FillWithNewGroupNumber(crosswise[2], crosswiseGroup.Key, DiagonalLinkedGroupsCount << (int)Dot.DiagonalGroupMaskShift);
							}
						}
					}

					//if ((i == _inputChainDots.Count - 2) && negativeSquare)
					//	break;
				}
			}

			// count must always not equals zero

			for (var j = 0; j < negativeSquareDotsPositions.Count; j++)
			{
				_chainDotsPositions.Add(new DotPosition(negativeSquareDotsPositions[j], _dots[negativeSquareDotsPositions[j]]));
				_dots[negativeSquareDotsPositions[j]] = _dots[negativeSquareDotsPositions[j]].SetDiagonalGroupNumber(newGroup);
			}
		}

		private bool IsAllFromIdenticalGroups()
		{
			for (int i = 0; i < _inputChainDots.Count; i++)
				for (int j = i + 1; j < _inputChainDots.Count; j++)
					if (_dots[i] != _dots[j])
						return false;
			return true;
		}

		private int GetMinimalGroupNumber()
		{
			return (int)_inputChainDots.Min(pos => _dots[pos].GetDiagonalGroupNumber());
		}

		private void FillWithNewGroupNumber(int pos, int newGroupNumber)
		{
			_tempList.Clear();
			var enableCond = _dots[pos].GetEnabledCondition();

			var lastSurroundDotsPositionsCount = _surroundDotsPositions.Count;
			if (_dots[pos].GetEnabledCondition() == enableCond && !_dots[pos].IsTagged())
			{
				_surroundDotsPositions.Add(new DotPosition(pos, _dots[pos]));
				_dots[pos] = _dots[pos].SetDiagonalGroupNumber(newGroupNumber) | Dot.Tagged;
				_tempList.Add(pos);
			}

			int newPos;
			while (_tempList.Count != 0)
			{
				newPos = _tempList.Last();
				_tempList.RemoveAt(_tempList.Count - 1);

				for (int i = 0; i < Field.DiagVertHorizDeltas.Length; i++)
				{
					newPos = pos + Field.DiagVertHorizDeltas[i];
					if (_dots[newPos].GetEnabledCondition() == enableCond && !_dots[newPos].IsTagged())
					{
						_surroundDotsPositions.Add(new DotPosition(newPos, _dots[newPos]));
						_dots[newPos] = _dots[newPos].SetDiagonalGroupNumber(newGroupNumber) | Dot.Tagged;
						_tempList.Add(newPos);
					}
				}
			}

			for (int i = lastSurroundDotsPositionsCount; i < _surroundDotsPositions.Count; i++)
				_dots[_surroundDotsPositions[i].Position] &= ~Dot.Tagged;
		}

		private void FillWithNewGroupNumber(int pos, int oldGroupNumber, int newGroupNumber)
		{
			_tempList.Clear();

			if (_dots[pos].GetDiagonalGroupNumber() == (Dot)oldGroupNumber)
			{
				_surroundDotsPositions.Add(new DotPosition(pos, _dots[pos]));
				_dots[pos] = _dots[pos].SetDiagonalGroupNumber(newGroupNumber);
				_tempList.Add(pos);
			}

			while (_tempList.Count != 0)
			{
				pos = _tempList.Last();
				_tempList.RemoveAt(_tempList.Count - 1);

				for (int i = 0; i < Field.DiagVertHorizDeltas.Length; i++)
				{
					int newPos = pos + Field.DiagVertHorizDeltas[i];
					if (_dots[newPos].GetDiagonalGroupNumber() == (Dot)oldGroupNumber)
					{
						_surroundDotsPositions.Add(new DotPosition(newPos, _dots[newPos]));
						_dots[newPos] = _dots[newPos].SetDiagonalGroupNumber(newGroupNumber);
						_tempList.Add(newPos);
					}
				}
			}
		}

		#endregion

		#region Constructors

		protected FieldWithGroups()
			: base()
		{
		}

		public FieldWithGroups(int width, int height, enmSurroundCondition surroundCondition = enmSurroundCondition.Standart)
			: base(width, height, surroundCondition)
		{
		}

		#endregion
	}
}
