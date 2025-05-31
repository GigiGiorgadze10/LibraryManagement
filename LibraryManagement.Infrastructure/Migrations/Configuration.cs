namespace LibraryManagement.Infrastructure.Migrations
{
    using LibraryManagement.Infrastructure.Persistence;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<LibraryManagement.Infrastructure.Persistence.AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;

            ContextKey = "LibraryManagement.Infrastructure.Persistence.AppDbContext";
        }

        protected override void Seed(LibraryManagement.Infrastructure.Persistence.AppDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
            DbInitializer.Seed(context);
        }
    }
}
