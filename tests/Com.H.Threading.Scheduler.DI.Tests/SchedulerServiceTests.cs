using Microsoft.Extensions.DependencyInjection;

namespace Com.H.Threading.Scheduler.DI.Tests;

/// <summary>The scheduler as an application gets it: registered with AddSchedulerService and resolved from the container.</summary>
public class SchedulerServiceTests : IDisposable
{
    private readonly string _folder = Directory.CreateTempSubdirectory("hscheduler-di-tests-").FullName;

    private string ConfigPath => Path.Combine(_folder, "tasks.xml");

    public void Dispose()
    {
        try
        {
            Directory.Delete(_folder, recursive: true);
        }
        catch (IOException)
        {
            // Left for the system's temp clean-up.
        }
    }

    private ServiceProvider Build(Action<SchedulerServiceOptions>? more = null)
        => new ServiceCollection()
            .AddSchedulerService(options =>
            {
                options.ConfigPath = ConfigPath;
                options.TickInterval = 25;
                more?.Invoke(options);
            })
            .BuildServiceProvider();

    /// <summary>Runs the service for a while and returns what each call of the handler saw in the named tag.</summary>
    private static async Task<List<string?>> RunAsync(ISchedulerService scheduler, string tag, int milliseconds, bool fail = false)
    {
        var seen = new List<string?>();
        scheduler.IsDue += (_, e, _) =>
        {
            lock (seen) seen.Add(e[tag]);
            return fail ? throw new InvalidOperationException("failed") : Task.CompletedTask;
        };
        using var stop = new CancellationTokenSource();
        var running = scheduler.StartAsync(stop.Token);
        await Task.Delay(milliseconds);
        stop.Cancel();
        await running;
        await Task.Delay(100);
        lock (seen) return [.. seen];
    }

    [Fact]
    public async Task A_due_task_reaches_the_handler()
    {
        File.WriteAllText(ConfigPath, "<tasks><task><sys><interval>100</interval></sys><greeting>hello</greeting></task></tasks>");
        using var services = Build();

        var seen = await RunAsync(services.GetRequiredService<ISchedulerService>(), "greeting", 450);

        Assert.InRange(seen.Count, 2, 6);
        Assert.All(seen, value => Assert.Equal("hello", value));
    }

    [Fact]
    public async Task A_value_processor_given_in_the_options_is_used()
    {
        File.WriteAllText(ConfigPath, """<tasks><task><sys><interval>60000</interval></sys><greeting content_type="shout">hello</greeting></task></tasks>""");
        using var services = Build(options => options.ValueProcessors["shout"] = (item, _) =>
        {
            item!.Value = item.Value?.ToUpperInvariant();
            return item;
        });

        var seen = await RunAsync(services.GetRequiredService<ISchedulerService>(), "greeting", 300);

        Assert.Equal("HELLO", Assert.Single(seen));
    }

    [Fact]
    public async Task The_pause_for_a_failed_task_can_be_set_in_the_options()
    {
        // No interval and no sleep_on_error: the pause is the default one.
        File.WriteAllText(ConfigPath, "<tasks><task><sys><enabled>true</enabled></sys><name>t</name></task></tasks>");
        using var services = Build(options => options.DefaultSleepOnError = 300);

        var seen = await RunAsync(services.GetRequiredService<ISchedulerService>(), "name", 800, fail: true);

        // At 0, 300 and 600. A tick is 25 ms, so a retry on every tick would be about thirty calls.
        Assert.InRange(seen.Count, 2, 4);
    }
}
