namespace MassTransit.Conductor.Server
{
    using System;
    using System.Collections.Generic;

    public interface IServiceInstance
    {
        Guid InstanceId { get; }
        string InstanceName { get; }
        IReadOnlyDictionary<string, string> InstanceAttributes { get; }

        IServiceEndpoint CreateServiceEndpoint(IReceiveEndpointConfigurator configurator);
    }
}
