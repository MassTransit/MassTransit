namespace MassTransit.Monitoring.Performance
{
    using System;
    using System.Threading.Tasks;
    using Util;


    public class PerformanceCounterBusObserver :
        IBusObserver
    {
        readonly ICounterFactory _factory;

        public PerformanceCounterBusObserver(ICounterFactory factory)
        {
            _factory = factory;
        }

        public void PostCreate(IBus bus)
        {
            bus.ConnectPublishObserver(new PerformanceCounterPublishObserver(_factory));
            bus.ConnectSendObserver(new PerformanceCounterSendObserver(_factory));
            bus.ConnectReceiveObserver(new PerformanceCounterReceiveObserver(_factory));
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
            return Task.CompletedTask;
        }

        public Task StartFaulted(IBus bus, Exception exception)
        {
            return Task.CompletedTask;
        }

        public Task PreStop(IBus bus)
        {
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
}
