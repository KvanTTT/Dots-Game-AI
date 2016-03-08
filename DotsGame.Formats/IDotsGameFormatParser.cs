using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotsGame
{
    public interface IDotsGameFormatParser
    {
        GameInfo Parse(byte[] text);

        byte[] Save(GameInfo gameInfo);
    }
}
