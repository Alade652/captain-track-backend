# 🧺 Dry Cleaning Postman Collection Guide

## 📥 Import the Collection

1. Open Postman
2. Click **Import** button (top left)
3. Select the file: `CaptainTrack_DryCleaning.postman_collection.json`
4. The collection will be imported with all folders and endpoints

## 🔧 Setup Environment Variables

After importing, the collection includes default variables. You can override them:

### Collection Variables (Auto-managed)
These are automatically set by test scripts:
- `token` - Authentication token (set after login)
- `userId` - Customer user ID (set after registration)
- `customerId` - Customer ID (set after registration)
- `providerUserId` - Service provider user ID (set after registration)
- `dryCleanerId` - Dry cleaner business ID (set after registration)
- `itemId` - Laundry item ID (set when getting items)
- `packageId` - Package ID (set when getting packages)
- `dryCleaningId` - Booking ID (set when creating booking)

### Manual Variables
- `baseUrl` - Default: `http://localhost:5077` (change if your API runs on different port)
- `email` - Default: `john.doe@example.com`
- `password` - Default: `Password123!`

## 📋 Testing Flow

### Step 1: Authentication
1. **Customer Registration** - Register a customer account
   - Response automatically saves `userId` and `customerId`
2. **Service Provider Registration** - Register a service provider account
   - Response automatically saves `providerUserId`
3. **Login** - Login to get authentication token
   - Response automatically saves `token`

### Step 2: Register Dry Cleaner
1. **Register Store Owner** or **Register Freelancer**
   - Upload a file for business license/profile photo
   - Response automatically saves `dryCleanerId`

### Step 3: Setup Items & Packages
1. **Get All Items** - Get available laundry items
   - Response automatically saves first `itemId`
2. **Add Item to Dry Cleaner** - Add item with custom price
3. **Get All Packages** - Get available packages
   - Response automatically saves first `packageId`
4. **Add Package** - Create a new package

### Step 4: Create Booking
1. **Init Book (With Specific Dry Cleaner)** - Book with store owner
   OR
   **Init Book (Freelancer)** - Book without specific provider
   - Response automatically saves `dryCleaningId`
   - Update the `deliveryDate` in the body to a future date
   - Update `location` to your test location
   - Update `itemId` values if needed

2. **Book (Confirm Booking)** - Confirm the booking
   - If dryCleanerId exists: Status → `Booked`
   - If no dryCleanerId: Status → `Pending` (notifies providers)

### Step 5: Provider Actions
1. **Get Pending Bookings** - View available bookings (within 5km)
2. **Accept Offer** - Provider makes an offer with custom amount
   - Sets status to `Booked`
3. **Raise Fare** - Customer can negotiate by raising price
   - Re-notifies providers

### Step 6: Service Execution
1. **Accept Booking** - Provider starts service
   - Status changes to `InProgress`
2. **Get All Bookings** - View all bookings for user

### Step 7: Optional Actions
1. **Cancel Booking** - Cancel a booking
   - Status changes to `Cancelled`

## 🎯 Complete Test Scenario

Here's a complete flow to test:

1. **Register Customer** → Get `customerId` and `userId`
2. **Register Service Provider** → Get `providerUserId`
3. **Login** → Get `token`
4. **Register Freelancer** → Get `dryCleanerId`
5. **Get All Items** → Get `itemId`
6. **Get All Packages** → Get `packageId`
7. **Init Book (Freelancer)** → Get `dryCleaningId`
8. **Book** → Status becomes `Pending`
9. **Get Pending Bookings** (as provider) → See the booking
10. **Accept Offer** → Status becomes `Booked`
11. **Accept Booking** → Status becomes `InProgress`
12. **Get All Bookings** → View all bookings

## 📝 Important Notes

### Request Bodies
- **JSON requests**: Use the pre-filled examples, but update:
  - `deliveryDate` - Use a future date in ISO format
  - `location` - Use a valid address
  - `itemId` and `packageId` - Use actual IDs from your database

### File Uploads
- For file upload endpoints, click the file field and select an image file
- Supported formats: JPG, PNG, PDF

### Authentication
- Most endpoints require `Authorization: Bearer {{token}}` header
- Token is automatically added from collection variables
- Token expires after 2 hours (login again if needed)

### Status Flow
```
Init → Pending → Booked → InProgress → Done
                ↓
            Cancelled/Rejected
```

### Distance Requirements
- Providers are only notified if within **5km (5000 meters)** of customer location
- Use valid addresses for location fields

## 🔍 Troubleshooting

### Variables Not Set
- Check the **Tests** tab in each request - they contain scripts that auto-save IDs
- Manually set variables if auto-save fails

### 401 Unauthorized
- Token may be expired - run **Login** again
- Check if token variable is set correctly

### 400 Bad Request
- Check request body format
- Verify IDs exist in database
- Check date format (ISO 8601: `2024-12-25T10:00:00Z`)

### No Pending Bookings
- Ensure provider location is within 5km of customer location
- Check if booking status is `Pending`
- Verify provider `IsAvailable` is `true`

## 🚀 Quick Start

1. Import collection
2. Update `baseUrl` if needed (default: `http://localhost:5077`)
3. Start with **Customer Registration**
4. Follow the flow sequentially
5. Check collection variables after each request to see auto-saved IDs

## 📚 Additional Resources

- See `POSTMAN_TESTING_GUIDE.md` for general API testing guide
- Check Swagger UI at `http://localhost:5077/swagger` for API documentation
- Review endpoint responses to understand data structures

---

**Happy Testing! 🎉**




