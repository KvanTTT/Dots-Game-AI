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

		private Dot[] Dots_;

		private List<LinkedGroup> Groups_;
		private List<LinkedGroup> OwnGroups_;
		private List<LinkedGroup> EnemyGroups_;
		private int[] DotsGroups;

		private SortedList<int, Crosswise> Crosswises_;

		#endregion

		#region Constructors

		public StrategicMovesAnalyzer(Field field)
		{
			Field = field;
			Dots_ = Field.CloneDots();
			DotsGroups = new int[Field.RealDotsCount];
		}

		#endregion

		#region Public Methods

		#endregion

		#region Helpers

		public void GenerateGroups()
		{
			Groups_ = new List<LinkedGroup>();
			OwnGroups_ = new List<LinkedGroup>();
			EnemyGroups_ = new List<LinkedGroup>();
			var ownPlayer = Field.CurrentPlayer.NextPlayer();
			var enemyPlayer = Field.CurrentPlayer;
			LinkedGroup group;

			for (int i = 1; i <= Field.Width; i++)
				for (int j = 1; j <= Field.Height; j++)
				{
					int ind = Field.GetPosition(i, j);

					if (!Dots_[ind].IsTagged())
					{
						if (Dots_[ind].IsPlayerPutted(ownPlayer))
						{
							group = new LinkedGroup(ownPlayer, Groups_.Count + 1, FillDiagVertHorizLinkedDots(ind, Groups_.Count + 1));
							OwnGroups_.Add(group);
							Groups_.Add(group);
						}
						else if (Dots_[ind].IsPlayerPutted(enemyPlayer))
						{
							group = new LinkedGroup(enemyPlayer, Groups_.Count + 1, FillDiagVertHorizLinkedDots(ind, Groups_.Count + 1));
							EnemyGroups_.Add(group);
							Groups_.Add(group);
						}
					}
				}

			ClearAllTags();
		}

		public void GenerateCrosswises()
		{
			var ownPlayer = Field.CurrentPlayer.NextPlayer();
			var enemyPlayer = Field.CurrentPlayer;
			Crosswises_ = new SortedList<int, Crosswise>();

			foreach (var group in OwnGroups_)
				foreach (var pos in group.Positions)
				{
					if (Field[pos + Field.RealWidth].IsPlayerPutted(enemyPlayer))
					{
						if (Field[pos + Field.RealWidth + 1].IsPlayerPutted(ownPlayer) &&
							Field[pos + 1].IsPlayerPutted(enemyPlayer))
							Crosswises_.Add(Crosswises_.Count + 1,
								new Crosswise(ownPlayer, pos, enmCrosswiseOrientation.BottomRight, Crosswises_.Count + 1));

						if (Field[pos + Field.RealWidth - 1].IsPlayerPutted(ownPlayer) &&
							Field[pos - 1].IsPlayerPutted(enemyPlayer))
							Crosswises_.Add(Crosswises_.Count + 1,
								new Crosswise(ownPlayer, pos, enmCrosswiseOrientation.BottomLeft, Crosswises_.Count + 1));
					}
				}
		}

		private List<int> FillDiagVertHorizLinkedDots(int pos, int currentGroupNumber)
		{
			var result = new List<int>();

			Dot player = Dots_[pos].GetPlayer();
			var tempStack = new Stack<int>();
			Dots_[pos] |= Dot.Tagged;
			tempStack.Push(pos);
			result.Add(pos);

			List<Dot> dots = new List<Dot>();
			while (tempStack.Count != 0)
			{
				pos = tempStack.Pop();

				for (int i = 0; i < Field.DiagVertHorizDeltas.Length; i++)
				{
					int newPos = pos + Field.DiagVertHorizDeltas[i];
					if (Dots_[newPos].IsPlayerPutted(player) && !Dots_[newPos].IsTagged())
					{
						Dots_[newPos] |= Dot.Tagged;
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
					Dots_[j * Field.RealWidth + i].ClearTag();
		}

		#endregion

		#region Properties

		public IEnumerable<LinkedGroup> OwnGroups
		{
			get
			{
				return OwnGroups_;
			}
		}

		public IEnumerable<LinkedGroup> EnemyGroups
		{
			get
			{
				return EnemyGroups_;
			}
		}

		public IEnumerable<LinkedGroup> Groups
		{
			get
			{
				return Groups_;
			}
		}

		public SortedList<int, Crosswise> Crosswises
		{
			get
			{
				return Crosswises_;
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
