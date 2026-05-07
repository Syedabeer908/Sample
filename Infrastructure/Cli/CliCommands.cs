using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApplication1.Seeders;
using WebApplication1.Settings;

namespace WebApplication1.Infrastructure.Cli
{
    public class CliCommands
    {
        // ---------------- MIGRATE ----------------
        public static async Task MigrateAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            Console.WriteLine("Running migrations...");

            await db.Database.MigrateAsync();

            Console.WriteLine("Migrations completed.");
        }

        // ---------------- SEED ----------------
        public static async Task SeedAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var roleOptions = scope.ServiceProvider
                .GetRequiredService<IOptions<RoleSettings>>().Value;

            var seeder = new DbSeeder(context, roleOptions);

            Console.WriteLine("Seeding database...");

            await seeder.SeedAsync();

            Console.WriteLine("Seeding completed.");
        }

        public static Task SeedHangfireAsync(IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            var storage = new SqlServerStorage(
                connectionString,
                new SqlServerStorageOptions
                {
                    PrepareSchemaIfNecessary = true
                });

            // 🔥 Force full initialization
            var monitor = storage.GetMonitoringApi();
            var servers = monitor.Servers();

            Console.WriteLine("Hangfire schema ensured.");

            return Task.CompletedTask;
        }
    }
}
