namespace MassTransit.Pipeline.Filters
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes.Util;


    public class RetryBusObserver :
        IBusObserver
    {
        readonly CancellationTokenSource _stopping;

        public RetryBusObserver()
        {
            _stopping = new CancellationTokenSource();
        }

        public CancellationToken Stopping => _stopping.Token;

        public void PostCreate(IBus bus)
        {
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
            _stopping.Cancel();

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
