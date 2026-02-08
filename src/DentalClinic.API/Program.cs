using DentalClinic.API.Middleware;
using DentalClinic.Application;
using DentalClinic.Infrastructure.DependencyInjection;
using DentalClinic.Infrastructure.Persistence;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ─── Serilog ──────────────────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/dental-clinic-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// ─── Services ─────────────────────────────────────────────────────────
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// ─── CORS ─────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? new[] { "http://localhost:3000", "http://localhost:5173" };

        policy.WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ─── Swagger ──────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Dental Clinic API",
        Version = "v1",
        Description = "Sistema de gestión para clínicas odontológicas"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese 'Bearer' seguido de su token JWT.\nEjemplo: Bearer eyJhbGciOiJIUzI1NiIs..."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ─── Middleware Pipeline ──────────────────────────────────────────────
app.UseMiddleware<ExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dental Clinic API v1");
        c.RoutePrefix = "swagger"; // Ahora Swagger será /swagger
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ActivityTrackingMiddleware>();

app.MapControllers();

// ─── TEST CONNECTION ──────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var connString = config.GetConnectionString("DefaultConnection");
    
    Console.WriteLine("===========================================");
    Console.WriteLine("CONNECTION STRING TEST:");
    Console.WriteLine(connString?.Replace("Password=syWa2CCtUt@2025!", "Password=***"));
    Console.WriteLine("===========================================");
    
    // Test 1: Direct SQL Connection
    try
    {
        using var conn = new Microsoft.Data.SqlClient.SqlConnection(connString);
        await conn.OpenAsync();
        Console.WriteLine("✓ Direct SqlConnection: SUCCESS");
        Console.WriteLine($"  Server: {conn.DataSource}");
        Console.WriteLine($"  Database: {conn.Database}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"✗ Direct SqlConnection: FAILED");
        Console.WriteLine($"  Error: {ex.Message}");
    }
    
    // Test 2: DbContext
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var canConnect = await context.Database.CanConnectAsync();
        Console.WriteLine($"✓ DbContext CanConnect: {canConnect}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"✗ DbContext: FAILED");
        Console.WriteLine($"  Error: {ex.Message}");
    }
    Console.WriteLine("===========================================");
}

// ─── Seed Database ────────────────────────────────────────────────────
await DatabaseSeeder.SeedAsync(app.Services);

Log.Information("Dental Clinic API started on {Environment}", app.Environment.EnvironmentName);

app.Run();
