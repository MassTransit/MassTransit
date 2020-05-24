namespace MassTransit.Registration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Attachments;
    using Configuration;


    public class MultiBusInstance<TBus> :
        IBusInstance<TBus>
        where TBus : IBus
    {
        readonly List<IBusAttachment> _attachments = new List<IBusAttachment>();
        readonly IBusInstance _instance;

        public MultiBusInstance(TBus bus, IBusInstance instance)
        {
            _instance = instance;
            BusInstance = bus;
        }

        public Type InstanceType => typeof(TBus);
        public IBus Bus => BusInstance;
        public IBusControl BusControl => _instance.BusControl;
        public IHostConfiguration HostConfiguration => _instance.HostConfiguration;
        public TBus BusInstance { get; }

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
