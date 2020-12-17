using Com.H.Threading.Scheduler;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Com.H.Threading.Scheduler
{
    public interface ISchedulerService
    {
        
        event Com.H.Threading.Scheduler.ServiceScheduler.ServiceIsDueEventHandler IsDue;
        event Com.H.Threading.Scheduler.ServiceScheduler.ErrorEventHandler Error;

        Task Start(CancellationToken cancellationToken);
        void Stop();


    }
}
