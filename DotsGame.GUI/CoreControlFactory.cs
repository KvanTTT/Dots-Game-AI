using System;
using Avalonia.Controls;
using DotsGame.AI;

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
