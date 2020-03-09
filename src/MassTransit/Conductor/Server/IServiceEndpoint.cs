namespace MassTransit.Conductor.Server
{
    using System.Threading.Tasks;
    using Contracts;


    public interface IServiceEndpoint
    {
        void ConfigureServiceEndpoint<T>(IConsumePipeConfigurator configurator)
            where T : class;

        void ConfigureControlEndpoint<T>(IReceiveEndpointConfigurator configurator)
            where T : class;

        Task<ServiceInfo> ServiceInfo { get; }

        Task<InstanceInfo> InstanceInfo { get; }
    }
}
