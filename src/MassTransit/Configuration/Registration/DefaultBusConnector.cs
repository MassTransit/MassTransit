namespace MassTransit.Registration
{
    using System;
    using Context;


    public class DefaultBusConnector :
        IBusConnector
    {
        public static readonly Lazy<IBusConnector> Instance = new Lazy<IBusConnector>(() => new DefaultBusConnector());

        DefaultBusConnector()
        {
        }

        public ReceiveEndpointContext CreateReceiveEndpointContext(string entityName, Action<IReceiveEndpointConfigurator> configure)
        {
            throw new NotSupportedException();
        }
    }
}
