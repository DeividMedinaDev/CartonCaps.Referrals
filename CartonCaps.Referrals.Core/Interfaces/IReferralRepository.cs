using System.Collections.Generic;
using System.Threading.Tasks;
using CartonCaps.Referrals.Core.Entities;

namespace CartonCaps.Referrals.Core.Interfaces
{
    public interface IReferralRepository
    {
        Task<Referral> AddAsync(Referral referral);
        Task<IEnumerable<Referral>> GetByReferrerIdAsync(string referrerUserId);
        Task<Referral?> GetByIdAsync(Guid id);
        Task<Referral?> GetByReferralCodeAsync(string referralCode);
        
        // Abuse mitigation/validation
        Task<int> GetCountByReferrerIdSinceDateAsync(string referrerUserId, DateTime since);
    }
}
