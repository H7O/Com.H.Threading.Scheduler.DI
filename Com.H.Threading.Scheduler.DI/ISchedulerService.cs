using Com.H.Threading.Scheduler;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Com.H.Threading.Scheduler
{
    public interface ISchedulerService
    {
        
        event Com.H.Threading.Scheduler.HTaskScheduler.TaskIsDueEventHandler IsDue;
        event Com.H.Threading.Scheduler.HTaskScheduler.ErrorEventHandler Error;

        Task Start(CancellationToken cancellationToken);
        void Stop();


    }
}
