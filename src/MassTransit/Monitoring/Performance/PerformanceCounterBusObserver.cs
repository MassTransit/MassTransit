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
            return TaskUtil.Completed;
        }

        public Task PostStart(IBus bus, Task<BusReady> busReady)
        {
            return TaskUtil.Completed;
        }

        public Task StartFaulted(IBus bus, Exception exception)
        {
            return TaskUtil.Completed;
        }

        public Task PreStop(IBus bus)
        {
            return TaskUtil.Completed;
        }

        public Task PostStop(IBus bus)
        {
            return TaskUtil.Completed;
        }

        public Task StopFaulted(IBus bus, Exception exception)
        {
            return TaskUtil.Completed;
        }
    }
}
