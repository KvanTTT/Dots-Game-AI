using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotsGame;

namespace DotsGame.AI
{
	public abstract class CMoveGenerator
	{
		#region Constructors
		
		public CMoveGenerator(Field field)
		{
			Field = field;
		}

		#endregion

		#region Abstract
		
		public abstract void GenerateMoves(Dot player, int depth = 0);
		public abstract void UpdateMoves();

		#endregion

		#region Properties
		
		public Field Field
		{
			get;
			protected set;
		}

		public List<int> Moves
		{
			get;
			protected set;
		}

		public int MaxDepth
		{
			get;
			set;
		}

		#endregion
	}
}
