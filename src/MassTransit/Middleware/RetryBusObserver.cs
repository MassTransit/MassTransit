namespace MassTransit.Middleware
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


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
            _stopping.Cancel();

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
