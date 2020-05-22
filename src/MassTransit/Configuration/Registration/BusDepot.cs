namespace MassTransit.Registration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;


    public class BusDepot :
        IBusDepot
    {
        readonly ILogger<BusDepot> _logger;
        readonly IDictionary<Type, IBusInstance> _instances;

        public BusDepot(IEnumerable<IBusInstance> instances, ILogger<BusDepot> logger)
        {
            _logger = logger;
            _instances = instances.ToDictionary(x => x.InstanceType);
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting bus instances: {Instances}", string.Join(", ", _instances.Keys.Select(x => x.Name)));

            await Task.WhenAll(_instances.Values.Select(x => x.BusControl.StartAsync(cancellationToken))).ConfigureAwait(false);
        }

        public async Task Stop(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Stopping bus instances: {Instances}", string.Join(", ", _instances.Keys.Select(x => x.Name)));

            await Task.WhenAll(_instances.Values.Select(x => x.BusControl.StopAsync(cancellationToken))).ConfigureAwait(false);
        }
    }
}
