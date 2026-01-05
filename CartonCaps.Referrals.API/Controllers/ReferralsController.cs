using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CartonCaps.Referrals.API.DTOs;
using CartonCaps.Referrals.Core.Entities;
using CartonCaps.Referrals.Core.Interfaces;

namespace CartonCaps.Referrals.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReferralsController : ControllerBase
    {
        private const string MockUserId = "user_123";
        private readonly IReferralRepository _repository;
        private readonly IDeepLinkService _deepLinkService;

        public ReferralsController(IReferralRepository repository, IDeepLinkService deepLinkService)
        {
            _repository = repository;
            _deepLinkService = deepLinkService;
        }

        [HttpPost]
        public async Task<ActionResult<ReferralCreatedResponse>> CreateReferral([FromBody] CreateReferralRequest request)
        {
            // Rate limiting: max 5 referrals per day
            var yesterday = DateTime.UtcNow.AddDays(-1);
            int recentReferrals = await _repository.GetCountByReferrerIdSinceDateAsync(MockUserId, yesterday);
            
            if (recentReferrals >= 5)
            {
                return BadRequest("Daily referral limit reached.");
            }

            var referral = new Referral
            {
                ReferrerUserId = MockUserId,
                ReferralCode = GenerateMockReferralCode(),
                CampaignId = request.CampaignId,
                Status = ReferralStatus.Pending
            };

            referral.GeneratedLink = await _deepLinkService.GenerateDeepLinkAsync(referral);

            await _repository.AddAsync(referral);

            var response = new ReferralCreatedResponse
            {
                Id = referral.Id,
                ShareUrl = referral.GeneratedLink,
                ReferralCode = referral.ReferralCode,
                CreatedAt = referral.CreatedAt
            };

            return CreatedAtAction(nameof(GetMyReferrals), new { }, response);
        }

        [HttpGet("resolve/{code}")]
        public async Task<ActionResult<ReferralContextResponse>> ResolveReferral(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return BadRequest("Code required");

            var referral = await _repository.GetByReferralCodeAsync(code);
            
            if (referral == null)
            {
                return Ok(new ReferralContextResponse { IsValid = false });
            }

            return Ok(new ReferralContextResponse
            {
                IsValid = true,
                ReferrerName = "Friend Request",
                WelcomeMessage = "Your friend invited you to Carton Caps! Get $5 off.",
                ObfuscatedReferrerId = "user_***"
            });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Referral>>> GetMyReferrals()
        {
            var referrals = await _repository.GetByReferrerIdAsync(MockUserId);
            return Ok(referrals);
        }

        private static string GenerateMockReferralCode()
        {
            return $"REF{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
        }
    }
}
