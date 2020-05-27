namespace MassTransit.Registration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Attachments;
    using Configuration;
    using Context;


    public class TransportBusInstance :
        IBusInstance
    {
        readonly List<IBusAttachment> _attachments = new List<IBusAttachment>();

        public TransportBusInstance(IBusControl busControl, IHostConfiguration hostConfiguration)
        {
            BusControl = busControl;
            HostConfiguration = hostConfiguration;
        }

        public Type InstanceType => typeof(IBus);
        public IBus Bus => BusControl;
        public IBusControl BusControl { get; }

        public IHostConfiguration HostConfiguration { get; }

        public void Connect(IBusAttachment attachment)
        {
            _attachments.Add(attachment);
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            LogContext.SetCurrentIfNull(HostConfiguration.LogContext);

            LogContext.Info?.Log("Starting bus: {Type} instance", InstanceType.Name);
            await BusControl.StartAsync(cancellationToken).ConfigureAwait(false);

            if (!_attachments.Any())
                return;

            LogContext.Info?.Log("Connecting {Type} attachments: {Names}", InstanceType.Name, string.Join(", ", _attachments.Select(x => x.Name)));
            await Task.WhenAll(_attachments.Select(attachment => attachment.Connect(cancellationToken))).ConfigureAwait(false);
        }

        public async Task Stop(CancellationToken cancellationToken)
        {
            LogContext.SetCurrentIfNull(HostConfiguration.LogContext);

            if (_attachments.Any())
            {
                LogContext.Info?.Log("Disconnecting {Type} attachments: {Names}", InstanceType.Name, string.Join(", ", _attachments.Select(x => x.Name)));
                await Task.WhenAll(_attachments.Select(attachment => attachment.Disconnect(cancellationToken))).ConfigureAwait(false);
            }

            LogContext.Info?.Log("Stopping bus: {Type} instance", InstanceType.Name);
            await BusControl.StopAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
