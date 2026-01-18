# 🚀 Postman Testing Guide - CaptainTrack API

Complete step-by-step guide to test all endpoints in Postman.

## 📋 Prerequisites

1. **Start your API server:**
   ```bash
   cd CaptainTrackBackend.Host
   dotnet run
   ```
   Your API will run on: `http://localhost:5077`

2. **Open Postman** and create a new Collection named "CaptainTrack API"

3. **Set up Environment Variables in Postman:**
   - Create a new Environment: "CaptainTrack Local"
   - Add variables:
     - `baseUrl` = `http://localhost:5077`
     - `token` = (will be set after login)
     - `userId` = (will be set after registration)
     - `customerId` = (will be set after registration)
     - `email` = (your test email)
     - `otp` = (will be set after OTP generation)

---

## 📝 STEP 1: Customer Registration

### Endpoint: Register a New Customer

**Request:**
- **Method:** `POST`
- **URL:** `{{baseUrl}}/api/customer/register`
- **Headers:**
  ```
  Content-Type: application/json
  ```
- **Body (raw JSON):**
  ```json
  {
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "password": "Password123!",
    "phoneNumber": "+2348012345678",
    "address": "123 Main Street, Lagos, Nigeria"
  }
  ```

**Expected Response (200 OK):**
```json
{
  "message": "Customer registered, awaiting otp verification",
  "success": true,
  "data": {
    "id": "guid-here",
    "userId": "guid-here",
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "phoneNumber": "+2348012345678",
    "address": "123 Main Street, Lagos, Nigeria"
  }
}
```

**What to do:**
1. Copy the `userId` from response → Save to Postman variable `userId`
2. Copy the `id` (customerId) from response → Save to Postman variable `customerId`
3. Check your email for OTP code

**Postman Script (Tests tab):**
```javascript
if (pm.response.code === 200) {
    var jsonData = pm.response.json();
    if (jsonData.success && jsonData.data) {
        pm.environment.set("userId", jsonData.data.userId);
        pm.environment.set("customerId", jsonData.data.id);
        pm.environment.set("email", jsonData.data.email);
    }
}
```

---

## 📧 STEP 2: Generate OTP

### Endpoint: Generate OTP for Email Verification

**Request:**
- **Method:** `POST`
- **URL:** `{{baseUrl}}/api/auth/genarateOTP`
- **Headers:**
  ```
  Content-Type: application/json
  ```
- **Body (raw JSON):**
  ```json
  {
    "userId": "{{userId}}",
    "email": "{{email}}"
  }
  ```
  OR use query parameters:
  ```
  {{baseUrl}}/api/auth/genarateOTP?userId={{userId}}&email={{email}}
  ```

**Expected Response (200 OK):**
```json
{
  "message": "OTP generated and sent successfully",
  "success": true,
  "data": "123456"
}
```

**What to do:**
1. Copy the OTP from response → Save to Postman variable `otp`
2. OR check your email for the OTP code

**Postman Script (Tests tab):**
```javascript
if (pm.response.code === 200) {
    var jsonData = pm.response.json();
    if (jsonData.success && jsonData.data) {
        pm.environment.set("otp", jsonData.data.toString());
    }
}
```

---

## ✅ STEP 3: Verify OTP

### Endpoint: Verify OTP

**Request:**
- **Method:** `POST`
- **URL:** `{{baseUrl}}/api/auth/verifyOTP`
- **Headers:**
  ```
  Content-Type: application/json
  ```
- **Body (raw JSON):**
  ```json
  {
    "otp": "{{otp}}",
    "userId": "{{userId}}",
    "email": "{{email}}"
  }
  ```
  OR use query parameters:
  ```
  {{baseUrl}}/api/auth/verifyOTP?otp={{otp}}&userId={{userId}}&email={{email}}
  ```

**Expected Response (200 OK):**
```json
{
  "message": "OTP verified successfully",
  "success": true,
  "data": true
}
```

**What to do:**
- If successful, your email is now verified and you can login

---

## 🔐 STEP 4: Login

### Endpoint: User Login

**Request:**
- **Method:** `POST`
- **URL:** `{{baseUrl}}/api/auth/login`
- **Headers:**
  ```
  Content-Type: application/json
  ```
- **Body (raw JSON):**
  ```json
  {
    "email": "{{email}}",
    "password": "Password123!"
  }
  ```
  OR use query parameters:
  ```
  {{baseUrl}}/api/auth/login?email={{email}}&password=Password123!
  ```

**Expected Response (200 OK):**
```json
{
  "message": "Login successful",
  "success": true,
  "data": {
    "id": "guid-here",
    "customerId": "guid-here",
    "email": "john.doe@example.com",
    "role": "Customer",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "serviceproviderRole": null,
    "services": []
  }
}
```

**What to do:**
1. Copy the `token` from response → Save to Postman variable `token`
2. This token will be used for authenticated requests

**Postman Script (Tests tab):**
```javascript
if (pm.response.code === 200) {
    var jsonData = pm.response.json();
    if (jsonData.success && jsonData.data && jsonData.data.token) {
        pm.environment.set("token", jsonData.data.token);
    }
}
```

---

## 👤 STEP 5: Get Customer Details

### Endpoint: Get Customer by ID

**Request:**
- **Method:** `GET`
- **URL:** `{{baseUrl}}/api/customer/get/{{customerId}}`
- **Headers:**
  ```
  Authorization: Bearer {{token}}
  ```

**Expected Response (200 OK):**
```json
{
  "message": "Customer retrieved successfully",
  "success": true,
  "data": {
    "id": "guid-here",
    "userId": "guid-here",
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "phoneNumber": "+2348012345678",
    "address": "123 Main Street, Lagos, Nigeria"
  }
}
```

---

### Endpoint: Get Customer by User ID

**Request:**
- **Method:** `GET`
- **URL:** `{{baseUrl}}/api/customer/getByUserId/{{userId}}`
- **Headers:**
  ```
  Authorization: Bearer {{token}}
  ```

---

### Endpoint: Get Customer by Email

**Request:**
- **Method:** `GET`
- **URL:** `{{baseUrl}}/api/customer/getByEmail/{{email}}`
- **Headers:**
  ```
  Authorization: Bearer {{token}}
  ```

---

## ✏️ STEP 6: Update Customer

### Endpoint: Update Customer Information

**Request:**
- **Method:** `PATCH`
- **URL:** `{{baseUrl}}/api/customer/update/{{customerId}}`
- **Headers:**
  ```
  Authorization: Bearer {{token}}
  Content-Type: application/json
  ```
- **Body (raw JSON):**
  ```json
  {
    "firstName": "John",
    "lastName": "Smith",
    "email": "john.smith@example.com",
    "phoneNumber": "+2348012345679",
    "address": "456 New Street, Lagos, Nigeria"
  }
  ```
  Note: All fields are optional - only include what you want to update

**Expected Response (200 OK):**
```json
{
  "message": "Customer updated successfully",
  "success": true,
  "data": {
    "id": "guid-here",
    "firstName": "John",
    "lastName": "Smith",
    "email": "john.smith@example.com",
    ...
  }
}
```

---

## 🔑 STEP 7: Reset Password

### Endpoint: Reset Password

**Request:**
- **Method:** `POST`
- **URL:** `{{baseUrl}}/api/auth/resetPassword/{{email}}`
- **Headers:**
  ```
  Content-Type: application/json
  ```
- **Body (raw JSON):**
  ```json
  {
    "password": "NewPassword123!"
  }
  ```
  OR use query parameter:
  ```
  {{baseUrl}}/api/auth/resetPassword/{{email}}?password=NewPassword123!
  ```

**Expected Response (200 OK):**
```json
{
  "message": "Password Updated",
  "success": true,
  "data": true
}
```

---

## 🚪 STEP 8: Logout

### Endpoint: Logout

**Request:**
- **Method:** `POST`
- **URL:** `{{baseUrl}}/api/auth/logout`
- **Headers:**
  ```
  Authorization: Bearer {{token}}
  Content-Type: application/json
  ```
- **Body (raw JSON):**
  ```json
  "{{token}}"
  ```

**Expected Response (200 OK):**
```json
{
  "message": "Logged out successfully",
  "success": true,
  "data": "Token blacklisted"
}
```

---

## 🗑️ STEP 9: Delete User (Admin Only)

### Endpoint: Delete User

**Request:**
- **Method:** `DELETE`
- **URL:** `{{baseUrl}}/api/auth/deleteUser/{{userId}}`
- **Headers:**
  ```
  Authorization: Bearer {{token}}
  ```
  Note: Requires Admin role

---

## 📦 Additional Endpoints

### Payment Endpoints

#### Initiate Payment
- **Method:** `POST`
- **URL:** `{{baseUrl}}/api/payment/initiate/{{userId}}`
- **Headers:**
  ```
  Authorization: Bearer {{token}}
  Content-Type: application/json
  ```
- **Body:**
  ```json
  {
    "amount": 5000,
    "redirectUrl": "https://yourdomain.com/payment/callback",
    "paymentTitle": "Service Payment",
    "paymentDescription": "Payment for service booking"
  }
  ```

#### Payment Callback
- **Method:** `GET`
- **URL:** `{{baseUrl}}/api/payment/callback?transaction_id=TRANSACTION_ID`

---

### Rating Endpoints

#### Get Ratings
- **Method:** `GET`
- **URL:** `{{baseUrl}}/api/ratings`
- **Headers:**
  ```
  Authorization: Bearer {{token}}
  ```

---

## 🎯 Testing Tips

1. **Save Responses:** Use Postman's "Save Response" feature to keep track of IDs and tokens

2. **Use Collection Variables:** Create a collection with pre-filled variables for easier testing

3. **Test Error Cases:**
   - Try registering with existing email (should fail)
   - Try login with wrong password (should fail)
   - Try accessing protected endpoints without token (should return 401)

4. **Check Swagger:** Visit `http://localhost:5077/swagger` to see all available endpoints

5. **Monitor Logs:** Check your console/terminal for any error messages

---

## 🔄 Complete Flow Example

1. ✅ Register Customer → Get `userId` and `customerId`
2. ✅ Generate OTP → Get OTP code
3. ✅ Verify OTP → Email verified
4. ✅ Login → Get `token`
5. ✅ Get Customer Details → Verify data
6. ✅ Update Customer → Modify information
7. ✅ Logout → Invalidate token

---

## 📝 Notes

- All timestamps are in UTC
- GUIDs are used for IDs
- Passwords are hashed (BCrypt) - never returned in responses
- Tokens expire after 2 hours (configurable)
- OTP codes are typically 6 digits

---

## 🐛 Troubleshooting

**401 Unauthorized:**
- Check if token is valid and not expired
- Ensure token is in format: `Bearer <token>`
- Try logging in again to get a new token

**400 Bad Request:**
- Check request body format (must be valid JSON)
- Verify all required fields are present
- Check email format is valid

**404 Not Found:**
- Verify the endpoint URL is correct
- Check if the resource (ID) exists in database
- Ensure server is running

**500 Internal Server Error:**
- Check server logs for detailed error
- Verify database connection
- Check if all required services are running

---

**Happy Testing! 🚀**

