using System;
using System.Collections.Generic;
using System.Linq;
using DotsGame;

namespace DotsGame.AI
{
	public class StandartMoveGenerator : MoveGenerator
	{
		#region Constructors

		public StandartMoveGenerator(Field field) :
			base(field)
		{
		}

		#endregion

		#region Overrides

		public override void GenerateMoves(Dot player, int depth = 0)
		{
			if (depth == MaxDepth)
			{
				Moves.Clear();

				foreach (var dotState in Field.DotsSequanceStates)
					AddRemoveEmptyPositions(dotState.Move.Position);
			}
		}

		#endregion

		#region Helpers

		private void AddRemoveEmptyPositions(int pos)
		{
			var position = pos - Field.RealWidth - 1;
			for (int i = 0; i < 3; i++)
			{
				for (int j = position; j < position + 3; j++)
					if (Field.IsValidPos(j) && Field[j].IsPuttingAllowed() && !Moves.Contains(j))
						Moves.Add(j);

				position += Field.RealWidth;
			}
		}

		#endregion
	}
}
