using DAL;
using BLL;
using System.Text.Json.Serialization;
using DAL.Persistence;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using PL.Middleware;

Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var connectionString = $"Host={Env.GetString("DB_HOST", "localhost")};" +
                       $"Port={Env.GetString("DB_PORT", "5432")};" +
                       $"Database={Env.GetString("POSTGRES_DB", "NoticeBoardDb")};" +
                       $"Username={Env.GetString("POSTGRES_USER", "postgres")};" +
                       $"Password={Env.GetString("POSTGRES_PASSWORD", "postgres")};";

builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDataAccess(builder.Configuration);
builder.Services.AddBusinessLogic();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandler>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Starting database migration...");
        await context.Database.MigrateAsync();
        logger.LogInformation("Database migrated successfully.");
    }
    catch (Exception ex)
    {
        logger.LogCritical(ex, "An error occurred while migrating the database.");
        throw;
    }
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();