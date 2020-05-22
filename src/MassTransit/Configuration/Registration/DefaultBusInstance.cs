namespace MassTransit.Registration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Attachments;
    using Configuration;


    public class DefaultBusInstance :
        IBusInstance
    {
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
            throw new ConfigurationException("TODO: add a link to a documentation");
        }

        public Task Start(CancellationToken cancellationToken)
        {
            return BusControl.StartAsync(cancellationToken);
        }

        public Task Stop(CancellationToken cancellationToken)
        {
            return BusControl.StopAsync(cancellationToken);
        }
    }
}
