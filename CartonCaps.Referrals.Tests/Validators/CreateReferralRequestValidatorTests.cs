using CartonCaps.Referrals.API.DTOs;
using CartonCaps.Referrals.API.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace CartonCaps.Referrals.Tests.Validators
{
    public class CreateReferralRequestValidatorTests
    {
        private readonly CreateReferralRequestValidator _validator;

        public CreateReferralRequestValidatorTests()
        {
            _validator = new CreateReferralRequestValidator();
        }

        [Fact]
        public void Should_Not_Have_Error_When_CampaignId_Is_Null()
        {
            var model = new CreateReferralRequest { CampaignId = null };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.CampaignId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_CampaignId_Is_Valid()
        {
            var model = new CreateReferralRequest { CampaignId = "summer-sale-2025" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.CampaignId);
        }

        [Fact]
        public void Should_Have_Error_When_CampaignId_Is_Too_Long()
        {
            var model = new CreateReferralRequest { CampaignId = new string('a', 51) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.CampaignId)
                  .WithErrorMessage("CampaignId cannot exceed 50 characters.");
        }

        [Fact]
        public void Should_Have_Error_When_CampaignId_Contains_Special_Characters()
        {
            var model = new CreateReferralRequest { CampaignId = "invalid@char" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.CampaignId);
        }
    }
}
