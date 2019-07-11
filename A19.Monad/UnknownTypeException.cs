using System;

namespace Mrh.Monad
{
    public class UnknownTypeException : Exception
    {

        public readonly Type Type;
        
        public UnknownTypeException(Type type, string message) : base(message)
        {
            this.Type = type;
        }
    }
}