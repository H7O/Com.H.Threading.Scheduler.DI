# Com.H.Threading.Scheduler.DI
Dependency injection extension for [Com.H.Threading.Scheduler](https://github.com/H7O/Com.H.Threading.Scheduler) API to facilitate easier usage for applications that utilize IHostBuilder services DI pipeline

## Installation
The easiest way to install this package is via Nuget [https://www.nuget.org/packages/Com.H.Threading.Scheduler.DI/](https://www.nuget.org/packages/Com.H.Threading.Scheduler.DI/).
Alternatively, you can compile this git repository and reference the output dll directly in your application.

## Usage
### Windows service / Linux Daemon
#### Example 1 - How to use with IHostBuilder running a BackroundService

> Program.cs
```c#
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.H.Threading.Scheduler;
using System.IO;

namespace MyMiddleware
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddSchedulerService((options) => // add the scheduler to services DI pipeline
                    {
                        // pass the service configuration file (details on this can be found under https://github.com/H7O/Com.H.Threading.Scheduler project
                        options.ConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "tasks.xml");
                    });
                });
    }
}

```
> Worker.cs
```c#
using Com.H.Threading.Scheduler;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyMiddleware
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly SchedulerService _scheduler;

        public Worker(ILogger<Worker> logger,
                SchedulerService scheduler
            )
        {
            _logger = logger;
            this._scheduler = scheduler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this._scheduler.IsDue += Scheduler_TaskIsDue;
            await this._scheduler.Start(stoppingToken);
        }

        private void Scheduler_TaskIsDue(object sender, HTaskSchedulerEventArgs e)
        {
            _logger.LogInformation("Service triggered at: {time}", DateTimeOffset.Now);
        }
    }
}

```
