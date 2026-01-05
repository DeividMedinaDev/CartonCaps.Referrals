namespace CartonCaps.Referrals.API.DTOs
{
    public class CreateReferralRequest
    {
        // In a real app, User ID comes from Token, but for simplicity/mocking context, 
        // we might rely on the token extraction. 
        // For this DTO, we might allow passing 'campaign' context.
        public string? CampaignId { get; set; }
    }
}
