using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Com.H.Threading.Scheduler
{
    public static class SchedulerServiceCollectionExtensions
    {
        public static IServiceCollection AddSchedulerService(this IServiceCollection services)
        {
            services.TryAddSingleton<ISchedulerService, SchedulerService>();
            return services;
        }

        public static IServiceCollection AddSchedulerService(this IServiceCollection services, 
            Action<SchedulerServiceOptions> setupAction)
        {
            services.AddSchedulerService();
            services.Configure(setupAction);
            return services;
        }
    }
}