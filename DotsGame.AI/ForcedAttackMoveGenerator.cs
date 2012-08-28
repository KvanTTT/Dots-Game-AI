using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotsGame.AI
{
	public class MovesSequence
	{
		List<int> ChainPositions;
		List<int> SurroundedPositions;
		List<int> CapturePositions;
	}

	public class ForcedAttackMoveGenerator : CMoveGenerator
	{
		public ForcedAttackMoveGenerator(Field field) 
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
			MovesSequence ForcedAttackSequence;

			int bestMove = 0;
			float score = 0;
			int depth = 0;
		}
		
		public MovesSequence FindCapturesMoves(Dot curPlayer, int depth = 1)
		{
			throw new NotImplementedException();
		}

		public MovesSequence FindDefenceMoves(Dot player)
		{
			return FindCapturesMoves(player.NextPlayer(), 1);
		}
	}
}
