using System.Threading.Tasks;
using CartonCaps.Referrals.Core.Entities;
using CartonCaps.Referrals.Infrastructure.Services;
using FluentAssertions;
using Xunit;

namespace CartonCaps.Referrals.Tests.Services
{
    public class DeepLinkServiceTests
    {
        [Fact]
        public async Task GenerateDeepLinkAsync_ShouldReturnValidUrl_WhenReferralIsProvided()
        {
            // Arrange
            var service = new MockDeepLinkService();
            var referral = new Referral 
            { 
                ReferralCode = "TESTCODE",
                ReferrerUserId = "User1"
            };

            // Act
            var link = await service.GenerateDeepLinkAsync(referral);

            // Assert
            link.Should().StartWith("https://");
            link.Should().Contain("TESTCODE");
            link.Should().Contain(referral.Id.ToString());
        }
    }
}
