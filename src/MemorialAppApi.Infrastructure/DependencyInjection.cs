using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MemorialAppApi.Infrastructure.Persistence;
using MemorialAppApi.Infrastructure.Services;
using MemorialAppApi.Infrastructure.Persistence.Repositories;

namespace MemorialAppApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);
                    sqlOptions.CommandTimeout(30);
                }));

        // Repositories
        services.AddScoped<IMemorialRepository, MemorialRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICemeteryRepository, CemeteryRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IOtpRepository, OtpRepository>();

        // Services
        services.AddScoped<IBlobStorageService, BlobStorageService>();
        services.AddScoped<IEmailService, SendGridEmailService>();

        return services;
    }
}
