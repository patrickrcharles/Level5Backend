using Level5Backend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;


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

builder.Services.AddDbContext<Level5Context>(options =>
options.UseNpgsql(_connectionString));

builder.Services.AddCors(options => options.AddPolicy("ApiCorsPolicy", builder =>
{
    builder.WithOrigins("http://localhost:5173").AllowAnyMethod().AllowAnyHeader();
}));

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

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(MyAllowSpecificOrigins);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
