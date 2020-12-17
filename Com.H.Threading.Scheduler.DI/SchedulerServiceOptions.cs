namespace Com.H.Threading.Scheduler
{
    public class SchedulerServiceOptions
    {
        public string Version { get; set; } = "v1.0";
        public string ConfigPath { get; set; }
        public int? TickInterval { get; set; }

    }
}
