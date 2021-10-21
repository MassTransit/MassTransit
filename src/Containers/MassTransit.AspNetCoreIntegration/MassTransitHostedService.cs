namespace MassTransit.AspNetCoreIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Registration;
    using Util;


    public class MassTransitHostedService :
        IHostedService, IDisposable
    {
        readonly IBusDepot _depot;
        readonly bool _waitUntilStarted;
        Task _startTask;

        public MassTransitHostedService(IBusDepot depot, bool waitUntilStarted)
        {
            _depot = depot;
            _waitUntilStarted = waitUntilStarted;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _startTask = _depot.Start(cancellationToken);

            return _startTask.IsCompleted || _waitUntilStarted
                ? _startTask
                : TaskUtil.Completed;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _depot.Stop(cancellationToken);
        }

        public void Dispose()
        {
            _depot.Stop(CancellationToken.None).GetAwaiter().GetResult();
        }
    }
}
