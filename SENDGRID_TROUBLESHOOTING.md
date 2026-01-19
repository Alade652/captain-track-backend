# SendGrid "Unauthorized" Error Troubleshooting Guide

## Error Message
```
SendGrid API error: Unauthorized
{"errors":[{"message":"The provided authorization grant is invalid, expired, or revoked"}]}
```

## Common Causes & Solutions

### 1. **API Key Not Set in Render** ✅ Most Common

**Problem**: The `SENDGRID_API_KEY` environment variable is not set or is empty.

**Solution**:
1. Go to your Render dashboard
2. Select your service
3. Go to **Environment** tab
4. Check if `SENDGRID_API_KEY` exists
5. If missing, add it:
   ```
   Key: SENDGRID_API_KEY
   Value: SG.your-actual-api-key-here
   ```
6. Click **Save Changes**
7. Wait for automatic redeploy

**Verify**: Check Render logs after redeploy. You should see:
```
SendGrid API key found (length: XX chars). Using SendGrid API.
```

---

### 2. **API Key Copied Incorrectly** ✅ Very Common

**Problem**: The API key has extra spaces, missing characters, or was copied incorrectly.

**Solution**:
1. Go to SendGrid Dashboard → Settings → API Keys
2. If you see your key listed, you can't view it again (SendGrid hides it)
3. **Create a NEW API key**:
   - Click **"Create API Key"**
   - Name it: "CaptainTrack API - Production"
   - Select **"Restricted Access"** → **"Mail Send"** permission
   - Click **"Create & View"**
   - **Copy the ENTIRE key immediately** (starts with `SG.` and is ~70 characters)
4. Update Render environment variable with the NEW key
5. Delete the old API key if you're sure it's wrong

**Important**: 
- API key should start with `SG.`
- Should be approximately 70 characters long
- No spaces before or after
- Copy the entire key

---

### 3. **API Key Revoked or Expired**

**Problem**: The API key was deleted, revoked, or expired in SendGrid.

**Solution**:
1. Go to SendGrid Dashboard → Settings → API Keys
2. Check if your API key still exists
3. If it's missing or shows as "Revoked":
   - Create a new API key (see step 2 above)
   - Update Render environment variable
   - Redeploy

---

### 4. **API Key Missing Permissions**

**Problem**: The API key doesn't have "Mail Send" permission.

**Solution**:
1. Go to SendGrid Dashboard → Settings → API Keys
2. Find your API key
3. Click on it to view permissions
4. Ensure **"Mail Send"** permission is enabled
5. If not, you need to create a new API key with proper permissions:
   - Delete the old one
   - Create new with "Mail Send" permission
   - Update Render

---

### 5. **Wrong Environment Variable Name**

**Problem**: The environment variable name is incorrect.

**Solution**:
- **Correct name**: `SENDGRID_API_KEY` (all caps, underscore)
- **Incorrect names**: 
  - `SendGrid_ApiKey` ❌
  - `SENDGRIDAPIKEY` ❌
  - `sendgrid_api_key` ❌ (case-sensitive in some systems)

**Verify in Render**:
1. Environment tab
2. Look for exact match: `SENDGRID_API_KEY`
3. Value should be: `SG.xxxxxxxxxxxxxxxx...`

---

### 6. **API Key Contains Special Characters**

**Problem**: The API key might have been corrupted when copying/pasting.

**Solution**:
1. Create a new API key in SendGrid
2. Copy it directly (don't edit it)
3. Paste directly into Render environment variable
4. Don't add quotes around it in Render
5. Save and redeploy

---

## Step-by-Step Fix Checklist

Follow these steps in order:

- [ ] **Step 1**: Verify API key exists in SendGrid Dashboard
  - Go to: https://app.sendgrid.com/settings/api_keys
  - Check if your API key is listed and active

- [ ] **Step 2**: Create new API key (if needed)
  - Click "Create API Key"
  - Name: "CaptainTrack API"
  - Permission: "Restricted Access" → "Mail Send"
  - Copy the ENTIRE key (starts with `SG.`)

- [ ] **Step 3**: Verify Render Environment Variable
  - Go to Render Dashboard → Your Service → Environment
  - Check `SENDGRID_API_KEY` exists
  - Value should start with `SG.` and be ~70 chars
  - No extra spaces or quotes

- [ ] **Step 4**: Verify Sender Email is Verified
  - Go to: https://app.sendgrid.com/settings/sender_auth
  - Ensure your sender email is verified
  - Check `SENDGRID_FROM_EMAIL` matches verified email

- [ ] **Step 5**: Redeploy and Test
  - Save changes in Render (auto-redeploys)
  - Wait for deployment to complete
  - Test OTP email: `POST /api/auth/genarateOTP?email=test@example.com`
  - Check Render logs for SendGrid messages

- [ ] **Step 6**: Check SendGrid Activity Feed
  - Go to: https://app.sendgrid.com/activity
  - Look for your email attempts
  - Check status (delivered, bounced, blocked, etc.)

---

## Quick Test Commands

### Test if API Key is Set (in Render Logs)
After deployment, check logs for:
```
SendGrid API key found (length: XX chars). Using SendGrid API.
```

If you see:
```
SendGrid API key not configured. Falling back to SMTP.
```
→ API key is not set in Render

### Test API Key Format
The API key should:
- Start with `SG.`
- Be approximately 70 characters long
- Contain only letters, numbers, and dots
- Have no spaces

---

## Still Not Working?

### Check Render Logs
Look for these log messages:
1. `"Sending email to {email} via SendGrid API"` → API key found
2. `"SendGrid API key format is invalid"` → Wrong format
3. `"SendGrid API key not configured"` → Not set in Render
4. `"SendGrid API error: Unauthorized"` → Invalid/revoked key

### Verify SendGrid Account Status
1. Log into SendGrid dashboard
2. Check account status (not suspended)
3. Verify you're on the free tier (100 emails/day)
4. Check if you've exceeded daily limit

### Contact SendGrid Support
If nothing works:
- SendGrid Support: https://support.sendgrid.com/
- Include error message and API key name (NOT the key itself!)

---

## Fallback to SMTP

If SendGrid continues to fail, the system will automatically fall back to SMTP. To force SMTP:

1. Remove or comment out `SENDGRID_API_KEY` in Render
2. Ensure SMTP settings are configured:
   ```
   SMTP_HOST=smtp.gmail.com
   SMTP_PORT=587
   SMTP_USERNAME=your-email@gmail.com
   SMTP_PASSWORD=your-app-password
   SMTP_FROM_EMAIL=your-email@gmail.com
   ```

---

## Prevention Tips

1. ✅ **Always copy API keys immediately** - SendGrid hides them after creation
2. ✅ **Use Restricted Access** - Only grant "Mail Send" permission
3. ✅ **Name your keys clearly** - "Production", "Development", etc.
4. ✅ **Rotate keys periodically** - For security
5. ✅ **Never commit keys to git** - Use environment variables only
6. ✅ **Test after setting** - Verify emails work after configuration

---

## Summary

The "Unauthorized" error almost always means:
1. API key not set in Render environment variables, OR
2. API key is incorrect/invalid, OR  
3. API key was revoked/deleted

**Quick Fix**: Create a new API key in SendGrid and update Render environment variable.
