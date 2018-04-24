using System;

namespace DotsGame.AI
{
    public class BasicCore : Core, IMoveGenerator
    {
        public readonly Field DotsField;

        public BasicCore(Field dotsField)
            : base(dotsField)
        {
        }

        public Move[] Generate()
        {
            throw new NotImplementedException();
        }
    }
}
