using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotsGame.AI
{
    public interface IMoveGenerator
    {
        Move[] Generate();
    }
}
