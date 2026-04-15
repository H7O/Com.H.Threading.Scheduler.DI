using Microsoft.Extensions.Options;
using Com.H.Threading.Scheduler;
using System;
using System.Threading;
using System.Threading.Tasks;
using Com.H.Events;

namespace Com.H.Threading.Scheduler
{
    public class SchedulerService : ISchedulerService
    {
        private readonly SchedulerServiceOptions serviceOptions;

        public HTaskScheduler? BaseScheduler => this._scheduler;

        private readonly HTaskScheduler _scheduler;
        private bool _disposed;

        public SchedulerService(IOptions<SchedulerServiceOptions> schedulerServiceOptionsAccessor)
        {
            if (schedulerServiceOptionsAccessor == null)
                throw new ArgumentNullException(nameof(schedulerServiceOptionsAccessor));

            this.serviceOptions = schedulerServiceOptionsAccessor.Value
                ?? throw new ArgumentException("Scheduler service options value is null.", nameof(schedulerServiceOptionsAccessor));

            if (string.IsNullOrEmpty(this.serviceOptions.ConfigPath))
                throw new ArgumentException("ConfigPath must be specified in SchedulerServiceOptions.", nameof(schedulerServiceOptionsAccessor));

            this._scheduler = new HTaskScheduler(this.serviceOptions.ConfigPath);
            if (this.serviceOptions.TickInterval > 0) this._scheduler.TickInterval = (int) this.serviceOptions.TickInterval;
            if (this.serviceOptions.ValueProcessors is not null)
            {
                foreach (var vp in this.serviceOptions.ValueProcessors)
                    this._scheduler.Tasks?.ValueProcessors?.TryAdd(vp.Key, vp.Value);
            }
        }

        
        /// <summary>
        /// Gets triggered whenever a service is due for execution
        /// </summary>
        public event AsyncEventHandler<HTaskEventArgs> IsDue
        {
            add => this._scheduler.TaskIsDue += value;
            remove => this._scheduler.TaskIsDue -= value;
        }

        /// <summary>
        /// Gets triggered whenever there is an error that might get supressed when retry-on-error is enabled
        /// </summary>
        public event AsyncEventHandler<HTaskExecutionErrorEventArgs> TaskExceptionError
        {
            add => this._scheduler.TaskExecutionError += value;
            remove => this._scheduler.TaskExecutionError -= value;
        }


        /// <summary>
        /// Gets triggered whenever there is an error loading new tasks from storage (XML, DB, custom..) into memory
        /// </summary>
        public event AsyncEventHandler<HErrorEventArgs> TaskLoadingError
        {
            add => this._scheduler.TaskLoadingError += value;
            remove => this._scheduler.TaskLoadingError -= value;
        }


        /// <summary>
        /// Start monitoring scheduled services in order to trigger IsDue event when services are ready for execution.
        /// </summary>
        /// <param name="cancellationToken">If provided, the monitoring force stops all running services</param>
        /// <returns>a running monitoring task</returns>
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await this._scheduler.StartAsync(cancellationToken);
        }

        /// <summary>
        /// Stops monitoring services schedule, and terminates running services.
        /// </summary>
        public void Stop()
        {
            this._scheduler.Stop();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            this._scheduler.Dispose();
        }
    }
}