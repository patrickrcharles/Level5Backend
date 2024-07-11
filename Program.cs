using Level5Backend.Models;
using Microsoft.AspNetCore.Mvc;
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

builder.Services.AddCors(options => options.AddPolicy("ApiCorsPolicy", builder =>
{
    builder.WithOrigins("http://localhost:5173").AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddMvc();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    //app.UseSwaggerUI();
//    app.UseSwaggerUI(c =>
//                {
//                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Level 5");
//                    //c.RoutePrefix = "/swagger/v1/ui";
//                });
//}

app.UseCors(builder => builder
     .AllowAnyOrigin()
     .AllowAnyMethod()
     .AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
