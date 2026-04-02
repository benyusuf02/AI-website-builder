# API Documentation

## Authentication

### POST /api/auth/login
Login to the system.

**Request:**
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

**Response:**
```json
{
  "token": "eyJhbGc...",
  "expiresAt": "2024-12-31T12:00:00Z"
}
```

## Sites

### GET /api/sites
Get all sites for the current user.

**Headers:**
- Authorization: Bearer {token}

**Response:**
```json
{
  "data": [
    {
      "id": 1,
      "name": "mysite",
      "domain": "mysite.ydeveloper.com",
      "isActive": true
    }
  ],
  "success": true
}
```

### POST /api/sites
Create a new site.

**Request:**
```json
{
  "name": "My New Site",
  "subdomain": "mynewsite",
  "packageType": "pro"
}
```

## Rate Limiting

- **Global:** 100 requests/minute
- **API:** 30 requests/minute  
- **Auth:** 5 requests/minute

## Error Codes

- `400` Bad Request
- `401` Unauthorized
- `403` Forbidden
- `404` Not Found
- `429` Too Many Requests
- `500` Internal Server Error
