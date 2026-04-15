using Com.H.Threading.Scheduler.VP;
using System.Collections.Generic;

namespace Com.H.Threading.Scheduler
{
    public class SchedulerServiceOptions
    {
        public string Version { get; set; } = "v1.0";
        public string? ConfigPath { get; set; }
        public int? TickInterval { get; set; }
        public Dictionary<string, ValueProcessor> ValueProcessors { get; set; } = new Dictionary<string, ValueProcessor>();

    }
}
