# 🔍 Gmail SMTP Not Sending - Troubleshooting Guide

## ❌ Problem
Password reset emails are not being sent from https://modernhubshop.netlify.app/forgot-password

## ✅ What I Fixed
Added detailed error logging to see what's failing.

---

## 🎯 Possible Causes & Solutions

### 1. **SMTP Credentials Not Set in Render**

**Problem:** Your Render deployment doesn't have SMTP environment variables.

**Solution:** Add these to Render Dashboard:

```
SMTP__HOST=smtp.gmail.com
SMTP__PORT=587
SMTP__USERNAME=jattsam100@gmail.com
SMTP__PASSWORD=vesbkntrbgqrorwj
SMTP__FROMEMAIL=jattsam100@gmail.com
SMTP__FROMNAME=ModernHub
```

**Steps:**
1. Go to https://dashboard.render.com/
2. Select your API service
3. Go to **Environment** tab
4. Click **Add Environment Variable**
5. Add each variable above
6. Click **Save Changes** (Render will auto-redeploy)

---

### 2. **Gmail Blocking "Less Secure Apps"**

**Problem:** Gmail might be blocking SMTP access even with App Password.

**Solution A - Check Gmail Settings:**
1. Go to https://myaccount.google.com/security
2. Ensure **2-Step Verification** is ON
3. Check https://myaccount.google.com/apppasswords
4. If you see "App passwords unavailable", it means:
   - Either 2FA is not enabled
   - Or your account is a Google Workspace account with different security settings

**Solution B - Generate NEW App Password:**
1. Delete old app password
2. Generate a fresh one: https://myaccount.google.com/apppasswords
3. Update Render environment variable: `SMTP__PASSWORD=new-app-password`

---

### 3. **Wrong Frontend URL in Config**

**Problem:** Reset link points to localhost instead of production domain.

**Current:**
```json
"Frontend": {
  "ResetPasswordUrl": "http://localhost:5173/reset-password",
  "AdminResetPasswordUrl": "http://localhost:5174/reset-password"
}
```

**Solution:** Add production URLs to Render environment variables:

```
FRONTEND__RESETPASSWORDURL=https://modernhubshop.netlify.app/reset-password
FRONTEND__ADMINRESETPASSWORDURL=https://modernhubadmin.netlify.app/reset-password
```

Replace with your actual production URLs!

---

### 4. **CORS/API Not Reaching Backend**

**Problem:** Frontend can't reach your API on Render.

**Check:**
1. What's your Render API URL? (e.g., https://your-api.onrender.com)
2. Is your frontend calling the correct API URL?
3. Check browser console for CORS errors

**Solution:**
Update `AllowedOrigins` in Render:
```
ALLOWEDORIGINS__0=https://modernhubshop.netlify.app
ALLOWEDORIGINS__1=https://your-admin-domain.netlify.app
```

---

### 5. **Check Render Logs**

**Most Important Step:**

1. Go to Render Dashboard
2. Select your API service
3. Click **Logs** tab
4. Look for these messages after testing:
   - ✅ `Password reset email sent successfully to...`
   - ❌ `ERROR sending email to...`

**Common Errors:**

| Error Message | Solution |
|---------------|----------|
| `The SMTP server requires a secure connection` | Already fixed in code |
| `Username and Password not accepted` | Wrong App Password - regenerate |
| `Unable to connect to remote server` | Firewall/network issue on Render |
| `SMTP configuration is missing` | Environment variables not set |

---

## 🧪 Testing Steps

### Test Locally First:

1. **Run locally:**
   ```powershell
   cd "c:\Users\Sanjay Kumar Sharma\Downloads\ECOMMERCE E.NET\ECommerceApi(PRODUCTION)"
   dotnet run
   ```

2. **Test from Swagger:**
   - Open: http://localhost:80/swagger
   - Find: POST /api/v1/auth/forgot-password
   - Try it with: `{ "email": "jattsam100@gmail.com" }`
   - Check console output for ✅ or ❌ messages

3. **Check your email inbox**

### If Local Works But Production Doesn't:

**It means:**
- ✅ Your code is correct
- ✅ Gmail SMTP works
- ❌ Render environment variables are missing

**Action:** Add SMTP environment variables to Render (see Solution 1 above)

---

## 📋 Complete Render Environment Variables Checklist

```env
# MUST HAVE - Database
DEFAULT_CONNECTION=Host=ep-late-base-a1b63jtn-pooler.ap-southeast-1.aws.neon.tech;Port=5432;Database=neondb;Username=neondb_owner;Password=npg_TH3OwESbdlm1;SSL Mode=Require;Trust Server Certificate=true

# MUST HAVE - JWT
JWT__KEY=mysuperstrongsecurekeythatshardtoguess1234
JWT__ISSUER=ECommerce
JWT__AUDIENCE=ECommerce

# MUST HAVE - SMTP (for password reset)
SMTP__HOST=smtp.gmail.com
SMTP__PORT=587
SMTP__USERNAME=jattsam100@gmail.com
SMTP__PASSWORD=vesbkntrbgqrorwj
SMTP__FROMEMAIL=jattsam100@gmail.com
SMTP__FROMNAME=ModernHub

# MUST HAVE - Frontend URLs (Production)
FRONTEND__RESETPASSWORDURL=https://modernhubshop.netlify.app/reset-password
FRONTEND__ADMINRESETPASSWORDURL=https://your-admin-url.netlify.app/reset-password

# MUST HAVE - SuperAdmin
SUPERADMIN__NAME=Super Admin
SUPERADMIN__EMAIL=k74839209@gmail.com
SUPERADMIN__PASSWORD=1234

# MUST HAVE - CORS (Production domains)
ALLOWEDORIGINS__0=https://modernhubshop.netlify.app
ALLOWEDORIGINS__1=https://your-admin-url.netlify.app

# Optional - Payment (if using Razorpay)
RAZORPAY__KEY=rzp_test_QaYPGsbG4JfuC7
RAZORPAY__SECRET=ie0ZOSymp4Sv0msx5ibAi1J0

# Optional - Image Upload (if using Cloudinary)
CLOUDINARY__CLOUDNAME=dr4kfpzyo
CLOUDINARY__APIKEY=181366731389861
CLOUDINARY__APISECRET=OkMtGQKs2YUWPSHeAWrYDT8-nAM

# Optional - Server
PORT=5000
DOTNET_RUNNING_IN_CONTAINER=true
```

---

## 🎯 Quick Fix Steps (In Order)

1. **✅ Test Locally First**
   ```powershell
   dotnet run
   # Test forgot password
   # Check console for ✅ or ❌
   ```

2. **✅ If Local Works:**
   - Go to Render Dashboard
   - Add ALL SMTP__ environment variables
   - Save and wait for redeploy
   - Check Render logs
   - Test from production frontend

3. **✅ If Local Fails:**
   - Check Gmail App Password is correct
   - Regenerate App Password
   - Update appsettings.json
   - Test again

4. **✅ Check Email Inbox:**
   - Check Inbox
   - Check Spam folder
   - Check Gmail "All Mail"

---

## 🚨 Emergency Alternative

**If Gmail SMTP keeps failing**, switch to Resend (5 min setup):

1. Sign up: https://resend.com/ (free, 3000 emails/month)
2. Get API key
3. Install package: `dotnet add package Resend`
4. Update EmailService.cs (I can help with this)
5. Much more reliable than Gmail SMTP

---

## ✅ Expected Behavior

**When working correctly:**

1. User enters email on frontend
2. Frontend calls: `POST https://your-api.onrender.com/api/v1/auth/forgot-password`
3. Backend logs: `✅ Password reset email sent successfully to user@email.com`
4. User receives email with reset link
5. User clicks link → frontend opens reset page
6. User enters new password → done!

---

## 📞 Next Steps

1. **Deploy to Render** with environment variables
2. **Check Render logs** after testing
3. **Share the error message** if emails still not sending
4. Consider **switching to Resend** if Gmail continues to have issues

The improved logging will now show exactly what's failing! 🔍
