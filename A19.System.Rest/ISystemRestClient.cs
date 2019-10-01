using System.Threading.Tasks;
using A19.System.Common;
using Mrh.Monad;

namespace A19.System.Rest
{
    public interface ISystemRestClient
    {
        Task<IResultMonad<SystemLoginRs>> Login(SystemLoginRq systemLoginRq);
        Task<IResultMonad<ExtendSystemSessionRs>> Extend(ExtendSystemSessionRq request);
    }
}