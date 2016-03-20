using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotsGame.Formats
{
    public enum WinReason
    {
        Score,
        Resign,
        Time,
        Forfeit,
        Void,
        Unknown,
        Draw,
    }
}
