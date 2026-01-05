using System;

namespace CartonCaps.Referrals.API.DTOs
{
    public class ReferralCreatedResponse
    {
        public Guid Id { get; set; }
        public required string ShareUrl { get; set; }
        public required string ReferralCode { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
