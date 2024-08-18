using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

if (builder.Environment.IsProduction())
{
    Console.WriteLine("--> Using MSSQL Database");
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn")));
}
else
{
    Console.WriteLine("--> Using InMemory Database");
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseInMemoryDatabase("InMem"));
}

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();

builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

builder.Services.AddControllers();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// This line logs the CommandService endpoint URL to the console during application startup.
// It helps to verify that the configuration value for "CommandService" has been correctly loaded.
// This is useful for debugging and ensuring the app is communicating with the correct service.
Console.WriteLine($"---> CommandService Endpoint {builder.Configuration["CommandService"]}");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Call the PrepPopulation method to initialize the database with seed data
// This ensures that the database is populated with initial data when the application starts
PrepDb.PrepPopulation(app, app.Environment.IsProduction());

app.Run();
