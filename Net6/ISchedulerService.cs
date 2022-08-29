using Com.H.Threading.Scheduler;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static Com.H.Threading.Scheduler.HTaskScheduler;

namespace Com.H.Threading.Scheduler
{
    public interface ISchedulerService
    {
        
        event Com.H.Threading.Scheduler.HTaskScheduler.TaskIsDueEventHandler? IsDue;
        event Com.H.Threading.Scheduler.HTaskScheduler.TaskExecutionErrorEventHandler? TaskExceptionError;
        event Com.H.Events.HErrorEventHandler? TaskLoadingError;

        Task Start(CancellationToken cancellationToken);
        void Stop();


    }
}
