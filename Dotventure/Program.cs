using Dotventure.Models;
using Dotventure.Services;
using Dotventure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICustomerService, CustomerService>();

// Add connection string logging for debugging
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Connection String: {connectionString}");

// FIXED: Add DbContext with retry policy for Docker environments
builder.Services.AddDbContext<AdventureWorksLt2022Context>(options =>
    options.UseSqlServer(connectionString, sqlServerOptions =>
    {
        sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 10,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null);
        sqlServerOptions.CommandTimeout(60); // Increase timeout
    }));

// SIMPLIFIED: Basic health checks without Entity Framework specific check
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
    app.UseSwagger();
    app.UseSwaggerUI();

app.MapHealthChecks("/health");

// Add database connection test endpoint
app.MapGet("/test-db", async (AdventureWorksLt2022Context context) =>
{
    try
    {
        var canConnect = await context.Database.CanConnectAsync();
        return Results.Ok(new
        {
            DatabaseConnected = canConnect,
            Message = canConnect ? "Database connection successful" : "Database connection failed"
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Database connection error: {ex.Message}");
    }
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

// Add automatic database migration (optional)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AdventureWorksLt2022Context>();
        // Apply pending migrations
        context.Database.Migrate();
        Console.WriteLine("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while applying migrations: {ex.Message}");
        // Don't crash the app if migrations fail
    }
}

app.Run();