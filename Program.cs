using Level5Backend.Models;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string _connectionString = "server=level5db.ctnfhe6sfb4k.us-east-2.rds.amazonaws.com;user id=admin;pwd=GREENelk93;database=level5;persistsecurityinfo=True; convert zero datetime=True";

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<Level5Context>(options =>
options.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString)));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
