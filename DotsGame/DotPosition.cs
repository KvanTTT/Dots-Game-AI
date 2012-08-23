using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotsGame
{
	public struct DotPosition
	{
		public Dot Dot
		{
			get;
			set;
		}

		public int Position
		{
			get;
			set;
		}

		public DotPosition(int pos, Dot point) : this()
		{
			Position = pos;
			Dot = point;
		}

		public override string ToString()
		{
			return Position + ", " + Dot.ToString();
		}
	}
}
