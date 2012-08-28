using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotsGame;

namespace DotsGame.AI
{
	public class StrategicMovesAnalyzer
	{
		#region Fields

		private Dot[] _dots;

		private List<LinkedGroup> _groups_;
		private List<LinkedGroup> _ownGroups;
		private List<LinkedGroup> _enemyGroups;
		private int[] DotsGroups;

		private SortedList<int, Crosswise> _crosswises;

		#endregion

		#region Constructors

		public StrategicMovesAnalyzer(Field field)
		{
			Field = field;
			_dots = Field.CloneDots();
			DotsGroups = new int[Field.RealDotsCount];
		}

		#endregion

		#region Public Methods

		#endregion

		#region Helpers

		public void GenerateGroups()
		{
			_groups_ = new List<LinkedGroup>();
			_ownGroups = new List<LinkedGroup>();
			_enemyGroups = new List<LinkedGroup>();
			var ownPlayer = Field.CurrentPlayer.NextPlayer();
			var enemyPlayer = Field.CurrentPlayer;
			LinkedGroup group;

			for (int i = 1; i <= Field.Width; i++)
				for (int j = 1; j <= Field.Height; j++)
				{
					int ind = Field.GetPosition(i, j);

					if (!_dots[ind].IsTagged())
					{
						if (_dots[ind].IsPlayerPutted(ownPlayer))
						{
							group = new LinkedGroup(ownPlayer, _groups_.Count + 1, FillDiagVertHorizLinkedDots(ind, _groups_.Count + 1));
							_ownGroups.Add(group);
							_groups_.Add(group);
						}
						else if (_dots[ind].IsPlayerPutted(enemyPlayer))
						{
							group = new LinkedGroup(enemyPlayer, _groups_.Count + 1, FillDiagVertHorizLinkedDots(ind, _groups_.Count + 1));
							_enemyGroups.Add(group);
							_groups_.Add(group);
						}
					}
				}

			ClearAllTags();
		}

		public void GenerateCrosswises()
		{
			var ownPlayer = Field.CurrentPlayer.NextPlayer();
			var enemyPlayer = Field.CurrentPlayer;
			_crosswises = new SortedList<int, Crosswise>();

			foreach (var group in _ownGroups)
				foreach (var pos in group.Positions)
				{
					if (Field[pos + Field.RealWidth].IsPlayerPutted(enemyPlayer))
					{
						if (Field[pos + Field.RealWidth + 1].IsPlayerPutted(ownPlayer) &&
							Field[pos + 1].IsPlayerPutted(enemyPlayer))
							_crosswises.Add(_crosswises.Count + 1,
								new Crosswise(ownPlayer, pos, enmCrosswiseOrientation.BottomRight, _crosswises.Count + 1));

						if (Field[pos + Field.RealWidth - 1].IsPlayerPutted(ownPlayer) &&
							Field[pos - 1].IsPlayerPutted(enemyPlayer))
							_crosswises.Add(_crosswises.Count + 1,
								new Crosswise(ownPlayer, pos, enmCrosswiseOrientation.BottomLeft, _crosswises.Count + 1));
					}
				}
		}

		private List<int> FillDiagVertHorizLinkedDots(int pos, int currentGroupNumber)
		{
			var result = new List<int>();

			Dot player = _dots[pos].GetPlayer();
			var tempStack = new Stack<int>();
			_dots[pos] |= Dot.Tagged;
			tempStack.Push(pos);
			result.Add(pos);

			List<Dot> dots = new List<Dot>();
			while (tempStack.Count != 0)
			{
				pos = tempStack.Pop();

				for (int i = 0; i < Field.DiagVertHorizDeltas.Length; i++)
				{
					int newPos = pos + Field.DiagVertHorizDeltas[i];
					if (_dots[newPos].IsPlayerPutted(player) && !_dots[newPos].IsTagged())
					{
						_dots[newPos] |= Dot.Tagged;
						tempStack.Push(newPos);
						result.Add(newPos);

						DotsGroups[newPos] = currentGroupNumber;
					}
				}
			}

			return result;
		}

		private void ClearAllTags()
		{
			for (int i = 1; i <= Field.Width; i++)
				for (int j = 1; j <= Field.Height; j++)
					_dots[j * Field.RealWidth + i].ClearTag();
		}

		#endregion

		#region Properties

		public IEnumerable<LinkedGroup> OwnGroups
		{
			get
			{
				return _ownGroups;
			}
		}

		public IEnumerable<LinkedGroup> EnemyGroups
		{
			get
			{
				return _enemyGroups;
			}
		}

		public IEnumerable<LinkedGroup> Groups
		{
			get
			{
				return _groups_;
			}
		}

		public SortedList<int, Crosswise> Crosswises
		{
			get
			{
				return _crosswises;
			}
		}

		public Field Field
		{
			get;
			set;
		}

		#endregion
	}
}
