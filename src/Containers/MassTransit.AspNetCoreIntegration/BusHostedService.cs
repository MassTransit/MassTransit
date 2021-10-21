namespace MassTransit.AspNetCoreIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Util;


    public class BusHostedService :
        IHostedService, IDisposable
    {
        readonly IBusControl _bus;
        readonly bool _waitUntilStarted;
        Task<BusHandle> _startTask;

        public BusHostedService(IBusControl bus, bool waitUntilStarted)
        {
            _bus = bus;
            _waitUntilStarted = waitUntilStarted;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _startTask = _bus.StartAsync(cancellationToken);

            return _startTask.IsCompleted || _waitUntilStarted
                ? _startTask
                : TaskUtil.Completed;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _bus.StopAsync(cancellationToken);
        }

        public void Dispose()
        {
            _bus.Stop();
        }
    }
}
