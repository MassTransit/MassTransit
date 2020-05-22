namespace MassTransit.Registration
{
    using System;
    using Context;


    public interface IBusConnector
    {
        ReceiveEndpointContext CreateReceiveEndpointContext(string entityName, Action<IReceiveEndpointConfigurator> configure = null);
    }
}
