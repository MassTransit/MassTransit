namespace MassTransit.TestFramework
{
    using System;


    public interface ActivityTestContextConfigurator
    {
        void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configure);
    }
}
