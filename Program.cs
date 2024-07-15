using Level5Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


var MyAllowSpecificOrigins = "ApiCors";
// Add services to the container.
string _connectionString = "server=level5db.ctnfhe6sfb4k.us-east-2.rds.amazonaws.com;user id=admin;pwd=GREENelk93;database=level5;persistsecurityinfo=True; convert zero datetime=True";

var builder = WebApplication.CreateBuilder(args);

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
                                              AllowAnyMethod().
                                              AllowAnyOrigin() ;
                      });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<Level5Context>(options =>
options.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString)));

builder.Services.AddCors(options => options.AddPolicy("ApiCorsPolicy", builder =>
{
    builder.WithOrigins("http://localhost:5173").AllowAnyMethod().AllowAnyHeader();
}));


var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(MyAllowSpecificOrigins);
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
