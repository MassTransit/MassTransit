namespace MassTransit.Monitoring.Performance
{
    using System;
    using System.Threading.Tasks;
    using Util;


    public class PerformanceCounterBusObserver<TFactory> :
        IBusObserver
        where TFactory : ICounterFactory, new()
    {


        public Task PostCreate(IBus bus)
        {
            bus.ConnectPublishObserver(new PerformanceCounterPublishObserver<TFactory>());
            bus.ConnectSendObserver(new PerformanceCounterSendObserver<TFactory>());
            bus.ConnectReceiveObserver(new PerformanceCounterReceiveObserver<TFactory>());

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