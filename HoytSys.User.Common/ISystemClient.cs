using System.Threading.Tasks;
using Mrh.Monad;

namespace A19.User.Common
{
    public interface ISystemClient
    {
        Task<IResultMonad<SystemLoginRs>> Login(SystemLoginRq systemLoginRq);
        Task<IResultMonad<ExtendSystemSessionRs>> Extend(ExtendSystemSessionRq request);
    }
}