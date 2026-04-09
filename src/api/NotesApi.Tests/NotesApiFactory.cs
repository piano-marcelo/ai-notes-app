using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotesApi.Infrastructure;
using Testcontainers.PostgreSql;

namespace NotesApi.Tests;

public class NotesApiFactory : WebApplicationFactory<Program>, IAsyncDisposable
{
    protected readonly PostgreSqlContainer PostgresContainer = new PostgreSqlBuilder("postgres:16-alpine").Build();

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Start the container before the host builds so GetConnectionString() is valid
        // when ConfigureWebHost service registrations run during builder.Build().
        PostgresContainer.StartAsync().GetAwaiter().GetResult();
        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(PostgresContainer.GetConnectionString()));
        });
    }

    public new async ValueTask DisposeAsync()
    {
        await PostgresContainer.StopAsync();
        await base.DisposeAsync();
    }
}
