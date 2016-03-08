using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotsGame.AI
{
    public abstract class Core
    {
        public readonly Field GameField;

        public Core(Field gameField)
        {
            GameField = gameField;
        }
    }
}
