namespace LibraryManagement.Infrastructure.Migrations
{
    using LibraryManagement.Infrastructure.Persistence; // ✅ ADD THIS if DbInitializer is in this namespace
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<LibraryManagement.Infrastructure.Persistence.AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            // Optional: You can set a ContextKey if you have multiple DbContexts targeting the same database,
            // but it's often set automatically to the fully qualified name of your DbContext.
            ContextKey = "LibraryManagement.Infrastructure.Persistence.AppDbContext";
        }

        protected override void Seed(LibraryManagement.Infrastructure.Persistence.AppDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data. For example:
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            // ✅ Call your custom DbInitializer's Seed method here
            DbInitializer.Seed(context);
        }
    }
}