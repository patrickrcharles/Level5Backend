using Level5Backend.Models;
using Level5Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;


var MyAllowSpecificOrigins = "ApiCors";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Connection string comes from configuration (appsettings.json "ConnectionStrings:DefaultConnection"),
// which is overridden in production via the ConnectionStrings__DefaultConnection environment variable -
// it must never be a literal in source, since source is committed to git.
string _connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is not configured. Set it via user-secrets (local dev) or the ConnectionStrings__DefaultConnection environment variable (production).");

// TokenController signs JWTs with this at request time; failing fast here surfaces a missing key at
// startup instead of as a cryptic 500 on the first login attempt.
if (string.IsNullOrEmpty(builder.Configuration["Jwt:Key"]))
{
    throw new InvalidOperationException("Jwt:Key is not configured. Set it via user-secrets (local dev) or the Jwt__Key environment variable (production).");
}

// add CORS 
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://sweatthis.com",
                                              "http://www.sweatthis.com",
                                              "http://api.sweatthis.com").
                                              AllowAnyHeader().
                                              AllowAnyMethod();
                      });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Without this, an unhandled exception anywhere returns the bare framework default (a raw stack
// trace in dev, an empty 500 in prod) instead of a consistent ProblemDetails body - and nothing
// about it gets logged unless a controller happens to catch and log it itself.
builder.Services.AddProblemDetails();

builder.Services.AddDbContext<Level5Context>(options =>
options.UseNpgsql(_connectionString));

// Recomputes ServerStats periodically off the request path (see Services/ServerStatsService.cs) -
// this used to run synchronously inline on every highscore POST.
builder.Services.AddScoped<IServerStatsService, ServerStatsService>();
builder.Services.AddHostedService<ServerStatsBackgroundService>();

// For container/orchestrator liveness-readiness probes - there was previously no way to tell from
// outside the process whether the app could actually reach Postgres.
builder.Services.AddHealthChecks().AddCheck<DatabaseHealthCheck>("database");

// TokenController issues JWTs signed with Jwt:Key/Issuer/Audience; without registering a matching
// authentication scheme here, every [Authorize]-protected endpoint 500s instead of 401ing, since
// there's no DefaultChallengeScheme for the authorization middleware to fall back on.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

// Used by the remaining admin-only endpoints (UserReportApiController.GetAllReports,
// ApplicationController.PostApplicationVersion). There's no roles system in this app, but User.Isdev
// already exists for exactly this purpose - TokenController puts it on the JWT as a claim.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireDev", policy => policy.RequireClaim("IsDev", "true"));
});

// TokenController.Post checks a submitted password against a stored hash with no other guard
// against brute-forcing - this is the only throttle on login attempts. Partitioned per client IP
// so one attacker can't exhaust the limit for everyone else.
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("LoginPolicy", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

var app = builder.Build();
// First so it can catch exceptions thrown by everything downstream, including other middleware.
app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(MyAllowSpecificOrigins);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();
