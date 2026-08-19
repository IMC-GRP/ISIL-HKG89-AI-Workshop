using AI_Workshop.Domain.Interfaces;
using AI_Workshop.Infrastructure.Data;
using AI_Workshop.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AI_Workshop.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AIWorkshopDatabase")
            ?? throw new InvalidOperationException("Connection string 'AIWorkshopDatabase' is not configured.");

        services.AddDbContext<AIWorkshopDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IIdeaRepository, IdeaRepository>();
        services.AddScoped<IScheduleRepository, ScheduleRepository>();
        services.AddScoped<IWorkshopTeamRepository, WorkshopTeamRepository>();
        services.AddScoped<IWorkshopSettingsRepository, WorkshopSettingsRepository>();

        return services;
    }
}
