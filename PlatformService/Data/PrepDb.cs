using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        // This method is called to prepare the database with seed data
        public static void PrepPopulation(IApplicationBuilder app)
        {
            // Create a temporary scope to access services
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                // Call the SeedData method to populate the database
                // Using the service scope to get an instance of AppDbContext
                // serviceScope: Provides a scoped lifetime for services, meaning the services created 
                // within this scope will be disposed of when the scope is disposed. 
                // ServiceProvider: Allows access to services within the created scope. 
                // GetService<AppDbContext>(): Retrieves the AppDbContext instance to interact with the database.
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>());
            }
        }
        // This method checks if the database is empty and, if so, adds initial data
        private static void SeedData(AppDbContext context)
        {
            // Check if there are any records in the Platforms table
            if (!context.Platforms.Any())
            {
                Console.WriteLine("--> Seeding data...");

                // Add a list of initial data (seed data) to the Platforms table
                context.Platforms.AddRange(
                    new Platform() { Name="Dot Net", Publisher ="Microsoft", Cost="Free" },
                    new Platform() { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free" },
                    new Platform() { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
                );

                // Save the changes to the database
                context.SaveChanges();
            }
            else
            {
                // If the database already has data, log a message
                Console.WriteLine("--> We already hava data");
            }
        }
    }
}
