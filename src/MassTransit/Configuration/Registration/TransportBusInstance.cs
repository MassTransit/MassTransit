namespace MassTransit.Registration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Context;
    using Riders;


    public class TransportBusInstance :
        IBusInstance
    {
        readonly List<IRider> _riders = new List<IRider>();

        public TransportBusInstance(IBusControl busControl, IHostConfiguration hostConfiguration)
        {
            BusControl = busControl;
            HostConfiguration = hostConfiguration;
        }

        public Type InstanceType => typeof(IBus);
        public IBus Bus => BusControl;
        public IBusControl BusControl { get; }

        public IHostConfiguration HostConfiguration { get; }

        public void Add(IRider rider)
        {
            _riders.Add(rider);
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            LogContext.SetCurrentIfNull(HostConfiguration.LogContext);

            LogContext.Info?.Log("Starting bus: {Type} instance", InstanceType.Name);
            await BusControl.StartAsync(cancellationToken).ConfigureAwait(false);

            if (!_riders.Any())
                return;

            LogContext.Info?.Log("Starting {Type} riders: {Names}", InstanceType.Name, string.Join(", ", _riders.Select(x => x.Name)));
            await Task.WhenAll(_riders.Select(rider => rider.Start(cancellationToken))).ConfigureAwait(false);
        }

        public async Task Stop(CancellationToken cancellationToken)
        {
            LogContext.SetCurrentIfNull(HostConfiguration.LogContext);

            if (_riders.Any())
            {
                LogContext.Info?.Log("Stopping {Type} riders: {Names}", InstanceType.Name, string.Join(", ", _riders.Select(x => x.Name)));
                await Task.WhenAll(_riders.Select(rider => rider.Stop(cancellationToken))).ConfigureAwait(false);
            }

            LogContext.Info?.Log("Stopping bus: {Type} instance", InstanceType.Name);
            await BusControl.StopAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
