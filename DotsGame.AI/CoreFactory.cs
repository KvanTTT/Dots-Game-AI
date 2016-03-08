using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotsGame.AI
{
    public static class CoreFactory
    {
        public static Core Create(CoreType coreType, Field field)
        {
            switch (coreType)
            {
                case CoreType.BasicCore:
                    return new BasicCore(field);
                case CoreType.GroupsCore:
                    return new GroupsCore(field);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
