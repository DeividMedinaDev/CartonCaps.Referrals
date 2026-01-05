using System;

namespace CartonCaps.Referrals.Core.Entities
{
    public class Referral
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string ReferrerUserId { get; set; }
        public required string ReferralCode { get; set; }
        public string? CampaignId { get; set; }
        public ReferralStatus Status { get; set; } = ReferralStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? InstalledAt { get; set; }
        public DateTime? RedeemedAt { get; set; }

        public string? GeneratedLink { get; set; } 
    }
}
