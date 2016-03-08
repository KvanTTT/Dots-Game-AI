using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotsGame;

namespace DotsGame.AI
{
	public class Crosswise
	{
		#region Readonly & Fields

		public readonly DotState Player;
		public readonly int Position;
		public readonly enmCrosswiseOrientation Orientation;
		public readonly int Number;

		#endregion

		#region Constructors

		public Crosswise(DotState player, int pos, enmCrosswiseOrientation orientation, int number)
		{
			Player = player;
			Position = pos;
			Orientation = orientation;
			Number = number;
		}

		#endregion

		#region Public Methods

		

		#endregion
	}
}
