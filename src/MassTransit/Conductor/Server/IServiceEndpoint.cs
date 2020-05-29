namespace MassTransit.Conductor.Server
{
    using System.Threading.Tasks;
    using Contracts.Conductor;


    public interface IServiceEndpoint
    {
        Task<ServiceInfo> ServiceInfo { get; }

        Task<InstanceInfo> InstanceInfo { get; }

        void ConfigureServiceEndpoint<T>(IConsumePipeConfigurator configurator)
            where T : class;

        void ConfigureControlEndpoint<T>(IReceiveEndpointConfigurator configurator)
            where T : class;
    }
}
