namespace MassTransit.AspNetCoreIntegration
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Util;


    public class BusHostedService :
        IHostedService
    {
        readonly IBusControl _bus;
        Task<BusHandle> _startTask;

        public BusHostedService(IBusControl bus)
        {
            _bus = bus;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _startTask = _bus.StartAsync(cancellationToken);

            return _startTask.IsCompleted
                ? _startTask
                : TaskUtil.Completed;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _bus.StopAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
