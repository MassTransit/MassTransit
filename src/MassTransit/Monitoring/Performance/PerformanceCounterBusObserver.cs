namespace MassTransit.Monitoring.Performance
{
    using System;
    using System.Threading.Tasks;
    using Util;


    public class PerformanceCounterBusObserver :
        IBusObserver
    {
        public Task PostCreate(IBus bus)
        {
            bus.ConnectPublishObserver(new PerformanceCounterPublishObserver());
            bus.ConnectSendObserver(new PerformanceCounterSendObserver());
            bus.ConnectReceiveObserver(new PerformanceCounterReceiveObserver());

            return TaskUtil.Completed;
        }

        public Task CreateFaulted(Exception exception)
        {
            return TaskUtil.Completed;
        }

        public Task PreStart(IBus bus)
        {
            return TaskUtil.Completed;
        }

        public Task PostStart(IBus bus, Task busReady)
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