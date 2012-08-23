using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotsGame;

namespace DotsGame.AI
{
	public class LinkedGroup
	{
		#region Readonly & Fields

		public const int ZeroGroup = 0;
		public readonly Dot Player;
		public readonly List<int> Positions;
		public readonly int Number;
		private List<int> EnvelopePositions_;

		#endregion

		#region Constructors
		
		public LinkedGroup(Dot player, int number, List<int> positions)
		{
			Player = player;
			Positions = positions;
			Number = number;

			BuildEnvelope();
		}
		
		#endregion

		#region Public Methods
		
		public void BuildEnvelope()
		{
			EnvelopePositions_ = new List<int>(Positions);

			if (Positions.Count == 1)
				return;

			int minPos = Positions.Min();
			int minPosX, minPosY;
			Field.GetPosition(minPos, out minPosX, out minPosY);

			EnvelopePositions_.Remove(minPos);

			EnvelopePositions_.Sort((pos2, pos1) =>
			{
				int x1, y1, x2, y2;
				Field.GetPosition(pos1, out x1, out y1);
				Field.GetPosition(pos2, out x2, out y2);
				x1 -= minPosX;
				y1 -= minPosY;
				x2 -= minPosX;
				y2 -= minPosY;
				return ((float)x1 / (Math.Abs(x1) + Math.Abs(y1))).CompareTo((float)x2 / (Math.Abs(x2) + Math.Abs(y2)));
			});

			EnvelopePositions_.Insert(0, minPos);

			int m = 0;
			for (int i = 1; i < EnvelopePositions_.Count; i++)
			{
				if (i != m)
				{
					if (m > 1)
					{
						while (m >= 1 && IsCCW(EnvelopePositions_[m - 1], EnvelopePositions_[m], EnvelopePositions_[i]) <= 0)
							m--;
					}
				}
				m++;
				int t = EnvelopePositions_[m];
				EnvelopePositions_[m] = EnvelopePositions_[i];
				EnvelopePositions_[i] = t;
			}

			if (m + 1 != EnvelopePositions_.Count)
				EnvelopePositions_.RemoveRange(m + 1, EnvelopePositions_.Count - m - 1);
		}

		#endregion

		#region Helpers

		private float IsCCW(int p1, int p2, int p3)
		{
			int x1, y1, x2, y2, x3, y3;
			Field.GetPosition(p1, out x1, out y1);
			Field.GetPosition(p2, out x2, out y2);
			Field.GetPosition(p3, out x3, out y3);
			return (x2 - x1) * (y3 - y1) - (y2 - y1) * (x3 - x1);
		}

		#endregion

		#region Properties
		
		public IEnumerable<int> EnvelopePositions
		{
			get
			{
				return EnvelopePositions_;
			}
		}

		#endregion
	}
}
