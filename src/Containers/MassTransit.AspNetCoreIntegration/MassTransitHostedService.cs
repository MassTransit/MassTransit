namespace MassTransit.AspNetCoreIntegration
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Registration;
    using Util;


    public class MassTransitHostedService :
        IHostedService
    {
        readonly IBusDepot _depot;
        Task _startTask;

        public MassTransitHostedService(IBusDepot depot)
        {
            _depot = depot;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _startTask = _depot.Start(cancellationToken);

            return _startTask.IsCompleted
                ? _startTask
                : TaskUtil.Completed;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _depot.Stop(cancellationToken);
        }
    }
}
