using JumboTravel.Api.src.Application.Data;
using JumboTravel.Api.src.Application.Services;
using JumboTravel.Api.src.Domain.Interfaces.Services;

string MyAllowSpecificOrigins = "JumboTravel";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

builder.Services.AddCors(o =>
{
    o.AddPolicy(name: MyAllowSpecificOrigins, builder =>
    {
        builder.WithOrigins("http://localhost:5500;https://jumbo-travel-web.vercel.app")
           .AllowAnyHeader()
           .AllowAnyMethod()
           .SetIsOriginAllowed((_) => true)
           .AllowCredentials();
    });
});

// Dependency Injection
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPlaneService, PlaneService>();

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

app.UseAuthorization();

app.MapControllers();

app.Run();