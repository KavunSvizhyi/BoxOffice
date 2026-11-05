using BoxOffice.Data.Data;
using BoxOffice.WebApi.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.WebApi.Test
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _dbName = Guid.NewGuid().ToString();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.UseEnvironment("Test");

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IDbContextOptionsConfiguration<TheatreDbContext>>();

                services.AddDbContext<TheatreDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_dbName);
                });
            });
        }

        public void ResetDatabase()
        {
            using var scope = Services.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
            var context = scope.ServiceProvider.GetRequiredService<TheatreDbContext>();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            seeder.Seed(context);
        }
    }
}
