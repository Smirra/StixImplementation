using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vulnerabilities.Api.Data;

namespace Vulnerabilities.Api.Tests;
public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddDbContext<VulnDbContext>(options =>
            {
                options.UseSqlite("Data Source=../../src/Vulnerabilities.Api/vulnerabilities.db");
            });

            // // Identity services
            // services.AddIdentity<IdentityUser, IdentityRole>()
            //     .AddEntityFrameworkStores<VulnDbContext>();

            // Build the service provider
            var sp = services.BuildServiceProvider();

            // Create a scope to resolve services
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<VulnDbContext>();
            var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TProgram>>>();

            // Ensure the database is created.
            db.Database.Migrate();

            try
            {
                // Seed the database with test data
                SeedDb.SeedVulnerabilities(db);
                SeedDb.SeedRoles(scopedServices.GetRequiredService<RoleManager<IdentityRole>>());
                SeedDb.SeedUsers(scopedServices.GetRequiredService<UserManager<IdentityUser>>());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the database with test data.");
            }
        });
    }
}