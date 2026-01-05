using System;
using System.Threading.Tasks;
using CartonCaps.Referrals.API.Controllers;
using CartonCaps.Referrals.API.DTOs;
using CartonCaps.Referrals.Core.Entities;
using CartonCaps.Referrals.Core.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CartonCaps.Referrals.Tests.Controllers
{
    public class ReferralsControllerTests
    {
        private readonly Mock<IReferralRepository> _mockRepo;
        private readonly Mock<IDeepLinkService> _mockDeepLinkService;
        private readonly ReferralsController _controller;

        public ReferralsControllerTests()
        {
            _mockRepo = new Mock<IReferralRepository>();
            _mockDeepLinkService = new Mock<IDeepLinkService>();
            _controller = new ReferralsController(_mockRepo.Object, _mockDeepLinkService.Object);
        }

        [Fact]
        public async Task CreateReferral_ShouldReturnCreated_WhenRequestIsValid()
        {
            // Arrange
            var request = new CreateReferralRequest { CampaignId = "summer-sale" };
            var expectedLink = "https://example.com/link";
            
            _mockRepo.Setup(r => r.GetCountByReferrerIdSinceDateAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(0);
            _mockDeepLinkService.Setup(s => s.GenerateDeepLinkAsync(It.IsAny<Referral>()))
                .ReturnsAsync(expectedLink);

            // Act
            var result = await _controller.CreateReferral(request);

            // Assert
            var actionResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            var response = actionResult.Value.Should().BeOfType<ReferralCreatedResponse>().Subject;

            response.ShareUrl.Should().Be(expectedLink);
            response.ReferralCode.Should().NotBeNullOrEmpty();
            
            // Verify repo interaction
            _mockRepo.Verify(r => r.AddAsync(It.Is<Referral>(x => x.CampaignId == "summer-sale")), Times.Once);
        }

        [Fact]
        public async Task CreateReferral_ShouldAllowNullCampaignId()
        {
            // Arrange
            var request = new CreateReferralRequest { CampaignId = null };
            
            _mockRepo.Setup(r => r.GetCountByReferrerIdSinceDateAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(0);
            _mockDeepLinkService.Setup(s => s.GenerateDeepLinkAsync(It.IsAny<Referral>()))
                .ReturnsAsync("https://link.test");

            // Act
            var result = await _controller.CreateReferral(request);

            // Assert
            result.Result.Should().BeOfType<CreatedAtActionResult>();
        }

        [Fact]
        public async Task CreateReferral_ShouldReturnBadRequest_WhenDailyLimitExceeded()
        {
            // Arrange
            var request = new CreateReferralRequest { CampaignId = "test" };
            
            // Mock: user already has 5 referrals in last 24 hours
            _mockRepo.Setup(r => r.GetCountByReferrerIdSinceDateAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(5);

            // Act
            var result = await _controller.CreateReferral(request);

            // Assert
            var badRequest = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().Be("Daily referral limit reached.");
        }

        [Fact]
        public async Task CreateReferral_ShouldAllowReferral_WhenWithinDailyLimit()
        {
            // Arrange
            var request = new CreateReferralRequest { CampaignId = "test" };
            
            // User has 4 referrals (below the 5 limit)
            _mockRepo.Setup(r => r.GetCountByReferrerIdSinceDateAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(4);
            _mockDeepLinkService.Setup(s => s.GenerateDeepLinkAsync(It.IsAny<Referral>()))
                .ReturnsAsync("https://link.test");

            // Act
            var result = await _controller.CreateReferral(request);

            // Assert
            result.Result.Should().BeOfType<CreatedAtActionResult>();
        }

        [Fact]
        public async Task ResolveReferral_ShouldReturnBadRequest_WhenCodeIsNull()
        {
            // Act
            var result = await _controller.ResolveReferral(null!);

            // Assert
            var badRequest = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().Be("Code required");
        }

        [Fact]
        public async Task ResolveReferral_ShouldReturnBadRequest_WhenCodeIsEmpty()
        {
            // Act
            var result = await _controller.ResolveReferral("");

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ResolveReferral_ShouldReturnBadRequest_WhenCodeIsWhitespace()
        {
            // Act
            var result = await _controller.ResolveReferral("   ");

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ResolveReferral_ShouldReturnInvalid_WhenCodeDoesNotExist()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByReferralCodeAsync("NOTFOUND"))
                .ReturnsAsync((Referral?)null);

            // Act
            var result = await _controller.ResolveReferral("NOTFOUND");

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<ReferralContextResponse>().Subject;
            response.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task ResolveReferral_ShouldReturnValid_WhenCodeExists()
        {
            // Arrange
            var existingReferral = new Referral
            {
                ReferrerUserId = "user_123",
                ReferralCode = "VALID123"
            };
            _mockRepo.Setup(r => r.GetByReferralCodeAsync("VALID123"))
                .ReturnsAsync(existingReferral);

            // Act
            var result = await _controller.ResolveReferral("VALID123");

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<ReferralContextResponse>().Subject;
            response.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task GetMyReferrals_ShouldReturnEmptyList_WhenNoReferralsExist()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByReferrerIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<Referral>());

            // Act
            var result = await _controller.GetMyReferrals();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var referrals = okResult.Value.Should().BeAssignableTo<IEnumerable<Referral>>().Subject;
            referrals.Should().BeEmpty();
        }

        [Fact]
        public async Task GetMyReferrals_ShouldReturnReferrals_WhenTheyExist()
        {
            // Arrange
            var mockReferrals = new List<Referral>
            {
                new Referral { ReferrerUserId = "user_123", ReferralCode = "REF1" },
                new Referral { ReferrerUserId = "user_123", ReferralCode = "REF2" }
            };
            _mockRepo.Setup(r => r.GetByReferrerIdAsync("user_123"))
                .ReturnsAsync(mockReferrals);

            // Act
            var result = await _controller.GetMyReferrals();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var referrals = okResult.Value.Should().BeAssignableTo<IEnumerable<Referral>>().Subject;
            referrals.Should().HaveCount(2);
        }
    }
}
