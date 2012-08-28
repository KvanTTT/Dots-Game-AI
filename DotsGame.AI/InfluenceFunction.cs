using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotsGame.AI
{
	public class InfluenceFunction
	{
		Field _field;
		float[] influence;

		public InfluenceFunction(Field field)
		{
			_field = field;
		}
	}
}
