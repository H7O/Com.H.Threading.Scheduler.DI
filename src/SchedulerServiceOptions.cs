using Com.H.Threading.Scheduler.VP;
using System.Collections.Generic;

namespace Com.H.Threading.Scheduler
{
    public class SchedulerServiceOptions
    {
        public string? ConfigPath { get; set; }
        public int? TickInterval { get; set; }

        /// <summary>
        /// How long, in milliseconds, a task that threw waits before it is tried again, when the task
        /// sets neither &lt;sleep_on_error&gt; nor &lt;interval&gt;. The scheduler's own default is 60000.
        /// </summary>
        public int? DefaultSleepOnError { get; set; }

        /// <summary>
        /// Custom value processors, by the content_type name that selects them. One with the name of
        /// a built-in processor (uri, json, xml, csv, psv) replaces it.
        /// </summary>
        public Dictionary<string, ValueProcessor> ValueProcessors { get; set; } = new Dictionary<string, ValueProcessor>();
    }
}
