using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotsGame.AI
{
    public abstract class Core
    {
        public readonly Field GameField;

        protected DotState[][] _dots;

        public Core(Field gameField)
        {
            GameField = gameField;
            _dots = new DotState[gameField.Width][];

            for (int x = 1; x <= gameField.Width; x++)
            {
                _dots[x - 1] = new DotState[gameField.Height];
                for (int y = 1; y <= gameField.Height; y++)
                {
                    _dots[x - 1][y - 1] = gameField[Field.GetPosition(x, y)];
                }
            }
        }
    }
}
