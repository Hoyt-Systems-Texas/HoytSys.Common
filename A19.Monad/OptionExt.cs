namespace Mrh.Monad
{
    public static class OptionExt
    {
        public static Option<T> Create<T>(T val)
        {
            return new Option<T>(val, Option<T>.OptionType.Some);
        }

        public static Option<T> None<T>()
        {
            return new Option<T>(default, Option<T>.OptionType.None);
        }
    }
}