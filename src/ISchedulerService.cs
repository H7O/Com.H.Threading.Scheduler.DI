using Com.H.Events;
using Com.H.Threading.Scheduler;
using System;
using System.Threading;
using System.Threading.Tasks;
using static Com.H.Threading.Scheduler.HTaskScheduler;

namespace Com.H.Threading.Scheduler
{
    public interface ISchedulerService : IDisposable
    {
        event AsyncEventHandler<HTaskEventArgs> IsDue;
        event AsyncEventHandler<HTaskExecutionErrorEventArgs> TaskExceptionError;
        event AsyncEventHandler<HErrorEventArgs> TaskLoadingError;

        Task StartAsync(CancellationToken cancellationToken = default);
        void Stop();
    }
}
