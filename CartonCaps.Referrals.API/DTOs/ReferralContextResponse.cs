namespace CartonCaps.Referrals.API.DTOs
{
    public class ReferralContextResponse
    {
        public bool IsValid { get; set; }
        public string? ReferrerName { get; set; } // Mocked
        public string? WelcomeMessage { get; set; }
        public string? ObfuscatedReferrerId { get; set; }
    }
}
