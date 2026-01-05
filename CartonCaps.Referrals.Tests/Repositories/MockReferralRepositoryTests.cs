using System;
using System.Linq;
using System.Threading.Tasks;
using CartonCaps.Referrals.Core.Entities;
using CartonCaps.Referrals.Infrastructure.Data;
using FluentAssertions;
using Xunit;

namespace CartonCaps.Referrals.Tests.Repositories
{
    public class MockReferralRepositoryTests
    {
        [Fact]
        public async Task GetCountByReferrerIdSinceDateAsync_ShouldOnlyCountReferralsWithinTimeWindow()
        {
            // Arrange
            var repo = new MockReferralRepository();
            var userId = "user_456";

            // Add an old referral (2 days ago)
            var oldReferral = new Referral
            {
                ReferrerUserId = userId,
                ReferralCode = "OLD123",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            };
            await repo.AddAsync(oldReferral);

            // Add a recent referral (1 hour ago)
            var recentReferral = new Referral
            {
                ReferrerUserId = userId,
                ReferralCode = "NEW123",
                CreatedAt = DateTime.UtcNow.AddHours(-1)
            };
            await repo.AddAsync(recentReferral);

            // Act - count only last 24 hours
            var yesterday = DateTime.UtcNow.AddDays(-1);
            var count = await repo.GetCountByReferrerIdSinceDateAsync(userId, yesterday);

            // Assert - should only count the recent one
            count.Should().Be(1);
        }

        [Fact]
        public async Task GetByReferralCodeAsync_ShouldBeCaseInsensitive()
        {
            // Arrange
            var repo = new MockReferralRepository();
            var referral = new Referral
            {
                ReferrerUserId = "user_789",
                ReferralCode = "ABC123"
            };
            await repo.AddAsync(referral);

            // Act - search with different casing
            var found = await repo.GetByReferralCodeAsync("abc123");

            // Assert
            found.Should().NotBeNull();
            found!.ReferralCode.Should().Be("ABC123");
        }

        [Fact]
        public async Task GetByReferralCodeAsync_ShouldReturnNull_WhenCodeDoesNotExist()
        {
            // Arrange
            var repo = new MockReferralRepository();

            // Act
            var result = await repo.GetByReferralCodeAsync("NONEXISTENT");

            // Assert
            result.Should().BeNull();
        }
    }
}
