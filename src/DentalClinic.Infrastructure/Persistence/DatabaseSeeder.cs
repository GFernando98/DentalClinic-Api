using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DentalClinic.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            // Apply pending migrations
            if ((await context.Database.GetPendingMigrationsAsync()).Any())
            {
                await context.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied.");
            }

            // Seed Roles
            string[] roles = { "Admin", "Doctor", "Receptionist", "Assistant" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    logger.LogInformation("Role '{Role}' created.", role);
                }
            }

            // Seed Admin User
            var adminEmail = configuration["DefaultAdmin:Email"] ?? "admin@syscore.app";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "Sistema",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var adminPassword = configuration["DefaultAdmin:Password"] ?? "Admin@123!";
                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    logger.LogInformation("Admin user created: {Email}", adminEmail);
                }
                else
                {
                    logger.LogError("Failed to create admin user: {Errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            // Seed Treatment Catalog
            if (!await context.Treatments.AnyAsync())
            {
                var treatments = new List<Treatment>
                {
                    new() { Code = "PREV-001", Name = "Limpieza dental", Category = TreatmentCategory.Preventive, DefaultPrice = 500, EstimatedDurationMinutes = 45 },
                    new() { Code = "PREV-002", Name = "Aplicación de flúor", Category = TreatmentCategory.Preventive, DefaultPrice = 300, EstimatedDurationMinutes = 20 },
                    new() { Code = "PREV-003", Name = "Sellante de fosetas y fisuras", Category = TreatmentCategory.Preventive, DefaultPrice = 400, EstimatedDurationMinutes = 30 },
                    new() { Code = "REST-001", Name = "Restauración con resina", Category = TreatmentCategory.Restorative, DefaultPrice = 800, EstimatedDurationMinutes = 45 },
                    new() { Code = "REST-002", Name = "Restauración con amalgama", Category = TreatmentCategory.Restorative, DefaultPrice = 600, EstimatedDurationMinutes = 40 },
                    new() { Code = "REST-003", Name = "Incrustación", Category = TreatmentCategory.Restorative, DefaultPrice = 3000, EstimatedDurationMinutes = 60 },
                    new() { Code = "ENDO-001", Name = "Endodoncia unirradicular", Category = TreatmentCategory.Endodontics, DefaultPrice = 3500, EstimatedDurationMinutes = 90 },
                    new() { Code = "ENDO-002", Name = "Endodoncia multirradicular", Category = TreatmentCategory.Endodontics, DefaultPrice = 5000, EstimatedDurationMinutes = 120 },
                    new() { Code = "PERIO-001", Name = "Raspado y alisado radicular", Category = TreatmentCategory.Periodontics, DefaultPrice = 1500, EstimatedDurationMinutes = 60 },
                    new() { Code = "CIRUG-001", Name = "Extracción simple", Category = TreatmentCategory.OralSurgery, DefaultPrice = 800, EstimatedDurationMinutes = 30 },
                    new() { Code = "CIRUG-002", Name = "Extracción quirúrgica", Category = TreatmentCategory.OralSurgery, DefaultPrice = 2500, EstimatedDurationMinutes = 60 },
                    new() { Code = "CIRUG-003", Name = "Extracción de tercer molar", Category = TreatmentCategory.OralSurgery, DefaultPrice = 4000, EstimatedDurationMinutes = 90 },
                    new() { Code = "PROST-001", Name = "Corona de porcelana", Category = TreatmentCategory.Prosthodontics, DefaultPrice = 5000, EstimatedDurationMinutes = 60 },
                    new() { Code = "PROST-002", Name = "Puente fijo", Category = TreatmentCategory.Prosthodontics, DefaultPrice = 12000, EstimatedDurationMinutes = 90 },
                    new() { Code = "PROST-003", Name = "Prótesis removible", Category = TreatmentCategory.Prosthodontics, DefaultPrice = 8000, EstimatedDurationMinutes = 60 },
                    new() { Code = "ORTO-001", Name = "Ortodoncia - brackets metálicos", Category = TreatmentCategory.Orthodontics, DefaultPrice = 35000, EstimatedDurationMinutes = 60 },
                    new() { Code = "ORTO-002", Name = "Ortodoncia - brackets estéticos", Category = TreatmentCategory.Orthodontics, DefaultPrice = 45000, EstimatedDurationMinutes = 60 },
                    new() { Code = "COSM-001", Name = "Blanqueamiento dental", Category = TreatmentCategory.Cosmetic, DefaultPrice = 3000, EstimatedDurationMinutes = 60 },
                    new() { Code = "COSM-002", Name = "Carilla de porcelana", Category = TreatmentCategory.Cosmetic, DefaultPrice = 6000, EstimatedDurationMinutes = 60 },
                    new() { Code = "DIAG-001", Name = "Radiografía periapical", Category = TreatmentCategory.Diagnostic, DefaultPrice = 150, EstimatedDurationMinutes = 10 },
                    new() { Code = "DIAG-002", Name = "Radiografía panorámica", Category = TreatmentCategory.Diagnostic, DefaultPrice = 500, EstimatedDurationMinutes = 15 },
                    new() { Code = "DIAG-003", Name = "Consulta de evaluación", Category = TreatmentCategory.Diagnostic, DefaultPrice = 300, EstimatedDurationMinutes = 30 },
                };

                await context.Treatments.AddRangeAsync(treatments);
                await context.SaveChangesAsync();
                logger.LogInformation("{Count} treatments seeded.", treatments.Count);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during database seeding.");
            throw;
        }
    }
}
