using Microsoft.Extensions.DependencyInjection;

namespace Copasogo.Infra.Jobs;

public static class DependencyInjection
{
    public static IServiceCollection AddInfraJobs(this IServiceCollection services)
    {
        return services;
    }
}
