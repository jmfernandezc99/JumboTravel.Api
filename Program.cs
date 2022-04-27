using JumboTravel.Api.src.Application.Data;
using JumboTravel.Api.src.Application.Services;
using JumboTravel.Api.src.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

string MyAllowSpecificOrigins = "JumboTravel";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

// CORS Configuration
builder.Services.AddCors(o =>
{
    o.AddPolicy(name: MyAllowSpecificOrigins, builder =>
    {
        builder.WithOrigins("http://localhost:5500", "https://jumbo-travel-web.vercel.app")
           .AllowAnyMethod()
           .AllowAnyHeader()
           .SetIsOriginAllowed((_) => true)
           .AllowCredentials();
    });
});

// Dependency Injection
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPlaneService, PlaneService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

var configuration = new ConfigurationBuilder()
     .AddJsonFile($"appsettings.development.json");

var _configuration = configuration.Build();

// JWT Configuration
builder.Services.AddAuthentication(
        options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }
    ).AddJwtBearer(o =>
    {
        var key = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
        o.SaveToken = true;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["JWT:Issuer"],
            ValidAudience = _configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)            
        };
    });

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();