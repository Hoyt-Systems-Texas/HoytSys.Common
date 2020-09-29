using System;

namespace Mrh.Monad
{
    public struct Option<T>
    {
        public readonly OptionType OptionT;
        public readonly T Value;

        public Option(
            T value,
            OptionType v)
        {
            OptionT = v;
            Value = value;
        }

        public enum OptionType
        {
            Some = 1,
            None = 2
        }

        public TR Handle<TR>(Func<T, TR> some, Func<TR> none)
        {
            switch (OptionT)
            {
                case OptionType.None:
                    return none();
                
                default:
                    return some(Value);
            }
        }
    }
}