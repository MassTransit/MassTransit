namespace MassTransit.Conductor.Server
{
    using System;
    using Contracts;


    public interface IServiceEndpoint
    {
        Uri ServiceAddress { get; }

        EndpointInfo EndpointInfo { get; }

        IMessageEndpoint<T> GetMessageEndpoint<T>()
            where T : class;

        void ConnectConfigurationObserver(IConsumePipeConfigurator configurator);
    }
}
