using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace DotsGame
{
	[StructLayout(LayoutKind.Sequential, Pack=1)]
	public struct DotPosition
	{
		public Dot Dot
		{
			get;
			set;
		}

		public short Position
		{
			get;
			set;
		}

		public DotPosition(int pos, Dot point)
			: this()
		{
			Position = (short)pos;
			Dot = point;
		}

		public override string ToString()
		{
			return Position + ", " + Dot.ToString();
		}
	}
}
