using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CreditGuardAPI.Data
{
    public static class DatabaseMigrationHelper
    {
        public static void MigrateDatabase<T>(IServiceProvider serviceProvider, ILogger logger) where T : DbContext
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<T>();
                    context.Database.Migrate();
                    logger.LogInformation($"Successfully migrated database for context {typeof(T).Name}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred while migrating database for context {typeof(T).Name}");
                throw;
            }
        }
    }
}