using System;
using Mrh.Monad;

namespace A19.Security.Web
{
    public interface ISecureTokenJsonGenerator
    {
        Span<byte> CreateToken<T>(
            T msg,
            DateTime expires);

        IResultMonad<T> DecodeToken<T>(byte[] value);
    }
}