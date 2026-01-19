# SendGrid Email Setup Guide

This guide will help you set up SendGrid for email delivery in your CaptainTrack API.

## Why SendGrid?

SendGrid is a cloud-based email service that:
- ✅ Works reliably from cloud hosting (no timeout issues)
- ✅ Free tier: 100 emails/day forever
- ✅ Better deliverability than SMTP
- ✅ No firewall/network restrictions
- ✅ Built-in analytics and tracking

## Step 1: Create SendGrid Account

1. Go to [https://sendgrid.com](https://sendgrid.com)
2. Click **"Start for Free"**
3. Sign up with your email address
4. Complete email verification

## Step 2: Create API Key

1. Log in to SendGrid dashboard
2. Go to **Settings** → **API Keys** (or visit: https://app.sendgrid.com/settings/api_keys)
3. Click **"Create API Key"**
4. Give it a name (e.g., "CaptainTrack API")
5. Select **"Full Access"** or **"Restricted Access"** with "Mail Send" permission
6. Click **"Create & View"**
7. **IMPORTANT**: Copy the API key immediately (you won't see it again!)
   - It will look like: `SG.xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx`

## Step 3: Verify Sender Email (Required)

SendGrid requires you to verify your sender email address:

1. Go to **Settings** → **Sender Authentication** (or visit: https://app.sendgrid.com/settings/sender_auth)
2. Click **"Verify a Single Sender"**
3. Fill in the form:
   - **From Email**: Your email (e.g., `noreply@yourdomain.com`)
   - **From Name**: Your app name (e.g., `CaptainTrack`)
   - **Reply To**: Your email
   - **Company Address**: Your address
   - **City**: Your city
   - **State**: Your state
   - **Country**: Your country
   - **Zip Code**: Your zip code
4. Click **"Create"**
5. Check your email inbox for verification email from SendGrid
6. Click the verification link in the email

**Note**: For production, consider Domain Authentication (better deliverability)

## Step 4: Configure Environment Variables on Render

### Option A: Using Render Dashboard (Recommended)

1. Go to your Render service dashboard
2. Click on your service
3. Go to **Environment** tab
4. Add these environment variables:

```
SENDGRID_API_KEY=SG.your-actual-api-key-here
SENDGRID_FROM_EMAIL=noreply@yourdomain.com
```

5. Click **"Save Changes"**
6. Render will automatically redeploy

### Option B: Using .env file (Local Development)

Add these lines to your `.env` file:

```
SENDGRID_API_KEY=SG.your-actual-api-key-here
SENDGRID_FROM_EMAIL=noreply@yourdomain.com
```

**Important**: Never commit `.env` file to git! It's already in `.gitignore`

## Step 5: Test Email Sending

After deployment, test the OTP email functionality:

1. Make a POST request to: `POST /api/auth/genarateOTP?email=your-test-email@example.com`
2. Check your email inbox for the OTP
3. Check Render logs to see: `"Sending email to {email} via SendGrid API"`

## Configuration Priority

The email service checks configuration in this order:

1. **SendGrid API Key** (if `SENDGRID_API_KEY` is set) → Uses SendGrid API ✅
2. **SMTP Settings** (if SendGrid not configured) → Falls back to SMTP

## Troubleshooting

### Email Not Sending?

1. **Check Logs**: Look for SendGrid errors in Render logs
2. **Verify API Key**: Make sure `SENDGRID_API_KEY` is set correctly
3. **Verify Sender**: Ensure sender email is verified in SendGrid dashboard
4. **Check SendGrid Dashboard**: Go to Activity Feed to see email status

### Common Errors:

- **"The from address does not match a verified Sender Identity"**
  → Verify your sender email in SendGrid dashboard

- **"Unauthorized"**
  → Check your API key is correct and has proper permissions

- **"Forbidden"**
  → Your API key might not have "Mail Send" permission

### Check SendGrid Activity

1. Go to SendGrid dashboard
2. Click **Activity** → **Activity Feed**
3. You'll see all email attempts with status (delivered, bounced, etc.)

## Free Tier Limits

- **100 emails/day** (forever free)
- **Unlimited contacts**
- **Email API access**
- **Basic analytics**

For production with higher volume, consider upgrading to a paid plan.

## Security Best Practices

1. ✅ **Never commit API keys to git**
2. ✅ **Use environment variables** (already configured)
3. ✅ **Rotate API keys** periodically
4. ✅ **Use restricted API keys** with only "Mail Send" permission
5. ✅ **Monitor SendGrid activity** for suspicious activity

## Support

- SendGrid Documentation: https://docs.sendgrid.com/
- SendGrid Support: https://support.sendgrid.com/

## Next Steps

Once SendGrid is configured:
- ✅ OTP emails will work reliably
- ✅ Welcome emails will be sent
- ✅ Password reset emails will work
- ✅ All email functionality will use SendGrid API

No more timeout errors! 🎉
