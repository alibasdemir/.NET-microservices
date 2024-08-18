var builder = WebApplication.CreateBuilder(args);

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

// Commented out app.UseHttpsRedirection() to avoid HTTPS redirection warning in Docker container.
// The warning occurred because the HTTPS port was not determined, which is common in containerized environments.
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
