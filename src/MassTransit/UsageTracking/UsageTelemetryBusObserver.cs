namespace MassTransit.UsageTracking;

using System;
using System.Threading.Tasks;
using UsageTelemetry;


public class UsageTelemetryBusObserver :
    IBusObserver
{
    readonly BusUsageTelemetry _busTelemetry;
    readonly UsageTracker _usageTracker;

    public UsageTelemetryBusObserver(UsageTracker usageTracker, BusUsageTelemetry busTelemetry)
    {
        _usageTracker = usageTracker;
        _busTelemetry = busTelemetry;
    }

    public void PostCreate(IBus bus)
    {
        _usageTracker.PostCreateBus(bus, _busTelemetry);
    }

    public void CreateFaulted(Exception exception)
    {
    }

    public Task PreStart(IBus bus)
    {
        return Task.CompletedTask;
    }

    public Task PostStart(IBus bus, Task<BusReady> busReady)
    {
        _usageTracker.PostStartBus(bus, _busTelemetry);

        return Task.CompletedTask;
    }

    public Task StartFaulted(IBus bus, Exception exception)
    {
        return Task.CompletedTask;
    }

    public Task PreStop(IBus bus)
    {
        _busTelemetry.Stopped = DateTimeOffset.Now.ToString("O");

        return Task.CompletedTask;
    }

    public Task PostStop(IBus bus)
    {
        return Task.CompletedTask;
    }

    public Task StopFaulted(IBus bus, Exception exception)
    {
        return Task.CompletedTask;
    }
}
