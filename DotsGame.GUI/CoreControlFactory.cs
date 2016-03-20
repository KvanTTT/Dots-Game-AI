using DotsGame.AI;
using Perspex.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotsGame.GUI
{
    public static class CoreControlFactory
    {
        public static UserControl Create(CoreType coreType)
        {
            switch (coreType)
            {
                case CoreType.SgfCore:
                    return new SgfCoreControl();
                case CoreType.GroupsCore:
                    return new GroupsCoreControl();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
