using Microsoft.Extensions.DependencyInjection;

namespace LegalStrategyAdvisor.Aspire.ServiceDefaults
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServiceDefaults(this IServiceCollection services)
        {
            // Add default services here
            return services;
        }
    }
}