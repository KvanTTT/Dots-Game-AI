using System.Collections.Generic;

namespace DotsGame.AI
{
    public abstract class MoveGenerator
	{
		#region Constructors
		
		public MoveGenerator(Field field)
		{
			Field = field;
			Moves = new List<int>(Field.DotsSequenceCount * 2);
		}

		#endregion

		#region Abstract
		
		public abstract void GenerateMoves(DotState player, int depth = 0);
		public virtual void UpdateMoves()
		{
		}

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

		public List<ulong> HashEntries
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
