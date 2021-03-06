﻿using System;

namespace DotsGame.AI
{
    public static class CoreFactory
    {
        public static Core Create(CoreType coreType, Field field)
        {
            switch (coreType)
            {
                case CoreType.SgfCore:
                    return new BasicCore(field);
                case CoreType.GroupsCore:
                    return new GroupsCore(field);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
