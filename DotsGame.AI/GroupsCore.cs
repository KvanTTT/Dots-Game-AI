using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotsGame.AI
{
    public class GroupsCore : Core
    {
        public GroupsCore(Field gameField)
            : base(gameField)
        {
        }

        public Dot[] GetFragileGroups()
        {
            throw new NotImplementedException();
        }

        public Dot[] GetStrongGroups()
        {
            throw new NotImplementedException();
        }
    }
}
