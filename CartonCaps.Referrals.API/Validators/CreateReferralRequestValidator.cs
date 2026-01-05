using System;
using CartonCaps.Referrals.API.DTOs;
using FluentValidation;

namespace CartonCaps.Referrals.API.Validators
{
    public class CreateReferralRequestValidator : AbstractValidator<CreateReferralRequest>
    {
        public CreateReferralRequestValidator()
        {
            RuleFor(x => x.CampaignId)
                .MaximumLength(50).WithMessage("CampaignId cannot exceed 50 characters.")
                .Matches("^[a-zA-Z0-9-_]*$").WithMessage("CampaignId must be alphanumeric (dashes and underscores allowed).")
                .When(x => !string.IsNullOrEmpty(x.CampaignId));
        }
    }
}
