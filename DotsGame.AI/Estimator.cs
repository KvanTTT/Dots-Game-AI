using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotsGame;

namespace DotsGame.AI
{
	public class Estimator
	{
		#region Constructors

		public Estimator(Field field)
		{
			Field = field;
		}

		#endregion

		#region Properties

		public readonly Field Field;

		#endregion

		#region Public Methods

		public float Estimate(Dot player)
		{
			if (player == Dot.RedPlayer)
				return (Field.RedCaptureCount - Field.BlueCaptureCount) +
					   (Field.RedSquare - Field.BlueSquare) * AiSettings.SquareCoef;
			else
				return (Field.BlueCaptureCount - Field.RedCaptureCount) +
					   (Field.BlueSquare - Field.RedSquare) * AiSettings.SquareCoef;
		}

		#endregion
	}
}
