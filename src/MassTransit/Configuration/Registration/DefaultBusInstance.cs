namespace MassTransit.Registration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Attachments;
    using Configuration;


    public class DefaultBusInstance :
        IBusInstance
    {
        readonly List<IBusAttachment> _attachments = new List<IBusAttachment>();

        public DefaultBusInstance(IBusControl busControl)
        {
            BusControl = busControl;
        }

        public Type InstanceType => typeof(IBus);
        public IBus Bus => BusControl;
        public IBusControl BusControl { get; }

        public IHostConfiguration HostConfiguration => default;

        public void Connect(IBusAttachment attachment)
        {
            _attachments.Add(attachment);
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            await BusControl.StartAsync(cancellationToken).ConfigureAwait(false);

            if (_attachments.Any())
                await Task.WhenAll(_attachments.Select(attachment => attachment.Connect(cancellationToken))).ConfigureAwait(false);
        }

        public async Task Stop(CancellationToken cancellationToken)
        {
            if (_attachments.Any())
                await Task.WhenAll(_attachments.Select(attachment => attachment.Disconnect(cancellationToken))).ConfigureAwait(false);

            await BusControl.StopAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}