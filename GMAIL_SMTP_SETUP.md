# Gmail SMTP Setup Guide

## ✅ What I Did

I've replaced SendGrid with **Gmail SMTP** for sending password reset emails.

- ✅ Updated `EmailService.cs` to use Gmail SMTP
- ✅ Updated `appsettings.json` with SMTP configuration
- ✅ Build successful - ready to use

---

## 🔑 Setup Steps (5 minutes)

### Step 1: Generate Gmail App Password

**Important:** You CANNOT use your regular Gmail password. You need an "App Password".

1. **Go to Gmail App Passwords:**
   - Visit: https://myaccount.google.com/apppasswords
   - (If you don't see this option, enable 2-Step Verification first)

2. **Create New App Password:**
   - Click "Select app" → Choose "Mail"
   - Click "Select device" → Choose "Other (Custom name)"
   - Type: "ModernHub Backend"
   - Click "Generate"

3. **Copy the 16-character password:**
   - It will look like: `abcd efgh ijkl mnop`
   - Remove spaces: `abcdefghijklmnop`
   - Save this - you'll need it in Step 2

### Step 2: Update appsettings.json

Open `appsettings.json` and update these values:

```json
"Smtp": {
  "Host": "smtp.gmail.com",
  "Port": "587",
  "Username": "jattsam100@gmail.com",           // ← Your Gmail address
  "Password": "abcdefghijklmnop",               // ← App password from Step 1
  "FromEmail": "jattsam100@gmail.com",          // ← Your Gmail address
  "FromName": "ModernHub"
}
```

**OR** use environment variables (more secure):

```powershell
$env:SMTP__USERNAME = "jattsam100@gmail.com"
$env:SMTP__PASSWORD = "abcdefghijklmnop"
$env:SMTP__FROMEMAIL = "jattsam100@gmail.com"
```

### Step 3: Test It!

Run your application:
```powershell
dotnet run
```

Then test password reset:
1. Go to your frontend
2. Click "Forgot Password"
3. Enter an email
4. Check your inbox!

---

## 📧 Email Limits

**Gmail SMTP Free Limits:**
- ✅ 500 emails per day
- ✅ 100% free forever
- ✅ Good for small to medium apps

If you need more, consider:
- Resend: 3,000 emails/month free
- Mailgun: 5,000 emails/month free

---

## 🔒 Security Tips

### Option 1: Environment Variables (Recommended)
Don't put your App Password in `appsettings.json` if you commit to Git.

Use environment variables instead:

**PowerShell:**
```powershell
$env:SMTP__USERNAME = "your-gmail@gmail.com"
$env:SMTP__PASSWORD = "your-app-password"
$env:SMTP__FROMEMAIL = "your-gmail@gmail.com"
$env:SMTP__FROMNAME = "ModernHub"
```

**Bash/Linux:**
```bash
export SMTP__USERNAME="your-gmail@gmail.com"
export SMTP__PASSWORD="your-app-password"
export SMTP__FROMEMAIL="your-gmail@gmail.com"
export SMTP__FROMNAME="ModernHub"
```

### Option 2: User Secrets (Development Only)
```powershell
dotnet user-secrets set "Smtp:Username" "your-gmail@gmail.com"
dotnet user-secrets set "Smtp:Password" "your-app-password"
dotnet user-secrets set "Smtp:FromEmail" "your-gmail@gmail.com"
```

---

## ⚠️ Troubleshooting

### Error: "The SMTP server requires a secure connection"
✅ Already fixed - using SSL on port 587

### Error: "Username and Password not accepted"
❌ You're using your regular Gmail password
✅ Use App Password from Step 1

### Error: "Less secure app access"
❌ This setting is deprecated
✅ Use App Password instead (Step 1)

### Emails go to Spam folder
This can happen with Gmail SMTP. Solutions:
1. **Add SPF record** to your domain DNS
2. **Use a custom domain** (not Gmail)
3. **Switch to Resend/Mailgun** for better deliverability

### "2-Step Verification is not enabled"
1. Go to: https://myaccount.google.com/security
2. Enable "2-Step Verification"
3. Then generate App Password

---

## 🎯 What Happens Next

When a user requests password reset:

1. User enters email on frontend
2. Frontend calls: `POST /api/v1/auth/forgot-password`
3. Backend generates reset token
4. Backend sends email via Gmail SMTP
5. User receives email with reset link
6. User clicks link → enters new password
7. Frontend calls: `POST /api/v1/auth/reset-password`
8. Password updated! ✅

---

## 📊 Comparison: SendGrid vs Gmail SMTP

| Feature | SendGrid | Gmail SMTP |
|---------|----------|------------|
| Free tier | 100/day | 500/day |
| Setup time | 15 min | 5 min |
| Cost | $19.95/month after | Free forever |
| Deliverability | Excellent | Good |
| Requires domain | No | No |
| API Key | Yes | No (uses password) |

---

## ✅ Done!

Your password reset now uses **Gmail SMTP** instead of SendGrid.

**Test it now:**
1. Update the SMTP credentials in `appsettings.json`
2. Run `dotnet run`
3. Try password reset from your frontend
4. Check your email!

**Need help?** If emails aren't sending:
- Check Gmail App Password is correct
- Check no spaces in the password
- Check Gmail account has 2FA enabled
- Check logs for specific error messages
