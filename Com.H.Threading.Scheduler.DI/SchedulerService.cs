using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Com.H.Threading.Scheduler;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Com.H.Threading.Scheduler
{
    public class SchedulerService : ISchedulerService
    {
        private readonly SchedulerServiceOptions serviceOptions;

        private readonly HTaskScheduler _scheduler;
        public SchedulerService(IOptions<SchedulerServiceOptions> schedulerServiceOptionsAccessor)
        {
            if (schedulerServiceOptionsAccessor == null) 
                throw new System.ArgumentNullException(nameof(schedulerServiceOptionsAccessor));
            this.serviceOptions = schedulerServiceOptionsAccessor.Value;
            if (string.IsNullOrEmpty(this.serviceOptions.ConfigPath)) throw new System.NullReferenceException("missing serviceOptions.ConfigPath");
            this._scheduler = new HTaskScheduler(this.serviceOptions.ConfigPath);
            if (this.serviceOptions.TickInterval > 0) this._scheduler.TickInterval = (int) this.serviceOptions.TickInterval;
        }

        
        /// <summary>
        /// Gets triggered whenever a service is due for execution
        /// </summary>
        public event Com.H.Threading.Scheduler.HTaskScheduler.TaskIsDueEventHandler IsDue
        {
            add
            {
                this._scheduler.TaskIsDue += value;
            }
            remove
            {
                this._scheduler.TaskIsDue -= value;
            }
        }

        /// <summary>
        /// Gets triggered whenever there is an error that might get supressed if retry on error is enabled
        /// </summary>
        public event Com.H.Threading.Scheduler.HTaskScheduler.ErrorEventHandler Error
        {
            add
            {
                this._scheduler.Error += value;
            }
            remove
            {
                
                this._scheduler.Error -= value;
            }
        }

        /// <summary>
        /// Start monitoring scheduled services in order to trigger IsDue event when services are ready for execution.
        /// </summary>
        /// <param name="cancellationToken">If provided, the monitoring force stops all running services</param>
        /// <returns>a running monitoring task</returns>
        public Task Start(CancellationToken cancellationToken)
            => this._scheduler.Start(cancellationToken);

        /// <summary>
        /// Stops monitoring services schedule, and terminates running services.
        /// </summary>
        public void Stop() => this._scheduler.Stop();
    }
}