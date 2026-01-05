using System;
using System.Threading.Tasks;
using CartonCaps.Referrals.Core.Entities;
using CartonCaps.Referrals.Core.Interfaces;

namespace CartonCaps.Referrals.Infrastructure.Services
{
    public class MockDeepLinkService : IDeepLinkService
    {
        public Task<string> GenerateDeepLinkAsync(Referral referral)
        {
            // Simulate external service call (e.g. Branch.io)
            var deepLink = $"https://cartoncaps.mock.link/r?code={referral.ReferralCode}&refId={referral.Id}";
            return Task.FromResult(deepLink);
        }
    }
}
