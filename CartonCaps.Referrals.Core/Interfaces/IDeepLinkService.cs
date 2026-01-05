using System.Threading.Tasks;
using CartonCaps.Referrals.Core.Entities;

namespace CartonCaps.Referrals.Core.Interfaces
{
    public interface IDeepLinkService
    {
        Task<string> GenerateDeepLinkAsync(Referral referral);
    }
}
