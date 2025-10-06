# Deploy SMTP Timeout Fix to Render

Write-Host "🚀 Deploying SMTP Timeout Fix to Render..." -ForegroundColor Cyan

# Check if we're in a git repo
if (-Not (Test-Path .git)) {
    Write-Host "❌ Not a git repository. Initialize git first:" -ForegroundColor Red
    Write-Host "   git init" -ForegroundColor Yellow
    Write-Host "   git remote add origin https://github.com/sanjaysharma11/ModernHub-Backend.git" -ForegroundColor Yellow
    exit 1
}

# Stage all changes
Write-Host "`n📦 Staging changes..." -ForegroundColor Yellow
git add .

# Commit with message
Write-Host "`n💾 Creating commit..." -ForegroundColor Yellow
git commit -m "Fix SMTP timeout with port fallback strategy (587 -> 465)

- Added automatic port fallback (587, 465)
- Increased SMTP timeout to 20 seconds per port
- Increased controller timeout to 30 seconds
- Added detailed logging for debugging
- Better error handling and retry logic"

# Push to GitHub
Write-Host "`n🚢 Pushing to GitHub..." -ForegroundColor Yellow
git push origin master

Write-Host "`n✅ Deployed! Render will auto-deploy in 2-3 minutes." -ForegroundColor Green
Write-Host "`n📝 Next steps:" -ForegroundColor Cyan
Write-Host "   1. Go to Render dashboard: https://dashboard.render.com/" -ForegroundColor White
Write-Host "   2. Check deployment status" -ForegroundColor White
Write-Host "   3. Once deployed, test: POST /auth/forgot-password" -ForegroundColor White
Write-Host "   4. Check logs for: '✅ Email sent successfully via port 465'" -ForegroundColor White
Write-Host "`n💡 Tip: If port 587 still times out, update Render env var:" -ForegroundColor Yellow
Write-Host "   SMTP__PORT=465" -ForegroundColor White
Write-Host ""
