using ECommerceApi.Data;
using ECommerceApi.Models;
using ECommerceApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --------------------- Load configuration ---------------------
var configuration = builder.Configuration;

// Override with Environment Variables (for Docker/Render)
var defaultConnection = Environment.GetEnvironmentVariable("DEFAULT_CONNECTION");
if (!string.IsNullOrWhiteSpace(defaultConnection))
    configuration["ConnectionStrings:DefaultConnection"] = defaultConnection;

var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

if (!string.IsNullOrWhiteSpace(jwtKey)) configuration["Jwt:Key"] = jwtKey;
if (!string.IsNullOrWhiteSpace(jwtIssuer)) configuration["Jwt:Issuer"] = jwtIssuer;
if (!string.IsNullOrWhiteSpace(jwtAudience)) configuration["Jwt:Audience"] = jwtAudience;

// --------------------- JWT Settings ---------------------
var jwtSettings = configuration.GetSection("Jwt");
var secretKey = jwtSettings["Key"]!;

// --------------------- CORS ---------------------
var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>();

// --------------------- DB Context ---------------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

// --------------------- Identity ---------------------
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// --------------------- Dependency Injection ---------------------
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<RazorpayService>();
builder.Services.AddScoped<CloudinaryService>();

builder.Services.AddControllers();

// --------------------- CORS Policy ---------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins(allowedOrigins!)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// --------------------- JWT Authentication ---------------------
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero,
        NameClaimType = ClaimTypes.Name
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// --------------------- Kestrel HTTP/HTTPS Config ---------------------
var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
var port = Environment.GetEnvironmentVariable("PORT") ?? "80";

if (isDocker)
{
    // Only HTTP inside Docker
    app.Urls.Add($"http://0.0.0.0:{port}");
}
else
{
    // Local dev - HTTP + HTTPS
    var httpsPort = Environment.GetEnvironmentVariable("HTTPS_PORT") ?? "443";
    app.Urls.Add($"http://0.0.0.0:{port}");
    app.Urls.Add($"https://0.0.0.0:{httpsPort}");
}

// --------------------- Middleware ---------------------
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// --------------------- Root route (GET + HEAD for monitoring) ---------------------
app.MapMethods("/", new[] { "GET", "HEAD" }, () => Results.Ok("🎉 Server is live! Welcome to ECommerceApi!"));

// --------------------- DB Migrations & Seed ---------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
    SeedData.SeedSuperAdmin(db, configuration);
}

// --------------------- Server Live Message ---------------------
var logger = app.Services.GetRequiredService<ILogger<Program>>();
var environment = app.Environment.EnvironmentName;

logger.LogInformation("🎉 Server is live!");
logger.LogInformation("🌍 Environment: {Environment}", environment);
logger.LogInformation("🚀 Listening on port: {Port}", port);
Console.WriteLine($"🎉 Server is live! Environment: {environment}, listening on port: {port}");

// --------------------- Run App ---------------------
app.Run();
