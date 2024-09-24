using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Com.H.Threading.Scheduler
{
    public static class SchedulerServiceCollectionExtensions
    {
        public static void AddSchedulerService(this IServiceCollection services)
        {
            // services.TryAddSingleton<ISchedulerService, SchedulerService>();
            services.AddSingleton<SchedulerService>();
        }

        public static void AddSchedulerService(this IServiceCollection services, 
            Action<SchedulerServiceOptions> setupAction)
        {
            services.AddSchedulerService();
            services.Configure(setupAction);
        }
    }
}