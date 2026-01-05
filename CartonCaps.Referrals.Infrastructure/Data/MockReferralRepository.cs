using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CartonCaps.Referrals.Core.Entities;
using CartonCaps.Referrals.Core.Interfaces;

namespace CartonCaps.Referrals.Infrastructure.Data
{
    public class MockReferralRepository : IReferralRepository
    {
        // Thread-safe collection since Singleton instance will be accessed concurrently
        private static readonly ConcurrentBag<Referral> _referrals = new ConcurrentBag<Referral>();

        public Task<Referral> AddAsync(Referral referral)
        {
            _referrals.Add(referral);
            return Task.FromResult(referral);
        }

        public Task<IEnumerable<Referral>> GetByReferrerIdAsync(string referrerUserId)
        {
            var results = _referrals.Where(r => r.ReferrerUserId == referrerUserId).ToList();
            return Task.FromResult<IEnumerable<Referral>>(results);
        }

        public Task<Referral?> GetByIdAsync(Guid id)
        {
            var referral = _referrals.FirstOrDefault(r => r.Id == id);
            return Task.FromResult(referral);
        }



        public Task<int> GetCountByReferrerIdSinceDateAsync(string referrerUserId, DateTime since)
        {
            var count = _referrals.Count(r => r.ReferrerUserId == referrerUserId && r.CreatedAt >= since);
            return Task.FromResult(count);
        }

        public Task<Referral?> GetByReferralCodeAsync(string referralCode)
        {
            var referral = _referrals.FirstOrDefault(r => 
                r.ReferralCode.Equals(referralCode, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(referral);
        }
    }
}
