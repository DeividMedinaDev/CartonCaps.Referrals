# REST API Specification: Referral Feature

## Overview
This document serves as the contract for the new Referral Feature. It details the endpoints required for the frontend (Mobile App) to integrate the "Share" and "Onboarding" flows.

## Global Error States
All endpoints may return the following standard HTTP errors:
- `400 Bad Request`: Validation failure (body missing, invalid fields).
- `401 Unauthorized`: User authentication token missing or invalid.
- `429 Too Many Requests`: Rate limit exceeded (Abuse mitigation).
- `500 Internal Server Error`: Unexpected server application error.

---

## 1. Create Referral Link
Generates a unique shareable deep link for the current user.

**Endpoint:** `POST /api/referrals`
**Auth required:** Yes (Bearer Token)

### Request Body
```json
{
  "campaignId": "string (optional)" // Context, e.g., "holiday_promo_2025"
}
```

### Response (201 Created)
```json
{
  "id": "uuid",
  "shareUrl": "https://cartoncaps.app.link/xYz123", // The Deep Link to share
  "referralCode": "DAN123",
  "createdAt": "2025-01-01T12:00:00Z"
}
```

### Business Rules & Abuse Mitigation
- **Rate Limit:** Users are limited to generating **5 referrals per day** to prevent spam/abuse.
- **Error 400:** Returns message "Daily referral limit reached" if limit is exceeded.

---

## 2. List User Referrals
Retrieves the history of referrals made by the current user.

**Endpoint:** `GET /api/referrals`
**Auth required:** Yes

### Response (200 OK)
```json
[
  {
    "id": "uuid",
    "referralCode": "DAN123",
    "status": "Pending | Installed | Redeemed",
    "generatedLink": "https://cartoncaps.app.link/...",
    "createdAt": "2025-01-01T12:00:00Z",
    "installedAt": null,
    "redeemedAt": null
  }
]
```

---

## 3. Resolve Referral (Onboarding)
Called by the Mobile App immediately after a fresh install if a deferred deep link context is found. This endpoint determines the personalized experience for the new user.

**Endpoint:** `GET /api/referrals/resolve/{code}`
**Auth required:** No (Public endpoint used during onboarding)

### Parameters
- `code` (Path): The referral code or token extracted from the deep link SDK.

### Response (200 OK)
```json
{
  "isValid": true,
  "referrerName": "John Doe",
  "welcomeMessage": "Your friend invited you to Carton Caps! Get $5 off.",
  "obfuscatedReferrerId": "user_***"
}
```

### Integration Flow (Frontend)
1. App installs.
2. App initializes Deep Link SDK (e.g., Branch/Firebase).
3. If SDK returns a `referral_code`:
    - Call `GET /api/referrals/resolve/{code}`.
    - If `isValid: true`: Show "Welcome via {referrerName}" screen.
    - If `isValid: false`: Show standard signup screen.

---

## Considerations
- **Abuse Mitigation:** Implemented strict daily limits on link generation.
- **Deep Linking:** The backend assumes an external provider generates the links. The API acts as the orchestrator to persist the relationship.
