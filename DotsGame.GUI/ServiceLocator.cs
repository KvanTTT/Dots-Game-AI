using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotsGame.GUI
{
    public class ServiceLocator
    {
        internal static DotsFieldViewModel DotsFieldViewModel { get; set; }

        internal static GameTreeViewModel GameTreeViewModel { get; set; }

        internal static SgfCoreControViewModel BasicCoreControViewModel { get; set; } = new SgfCoreControViewModel();

        internal static Settings Settings { get; set; } = Settings.Load();
    }
}
