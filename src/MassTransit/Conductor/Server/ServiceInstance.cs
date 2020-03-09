namespace MassTransit.Conductor.Server
{
    using System;


    public class ServiceInstance :
        IServiceInstance
    {
        readonly IServiceInstanceClientCache _clientCache;

        public ServiceInstance()
        {
            var instanceId = NewId.Next();

            InstanceId = instanceId.ToGuid();
            InstanceName = instanceId.ToString(Util.FormatUtil.Formatter);

            _clientCache = new ServiceInstanceClientCache();
        }

        public Guid InstanceId { get; }
        public string InstanceName { get; }

        public IServiceEndpoint CreateServiceEndpoint(IReceiveEndpointConfigurator configurator)
        {
            return new ServiceEndpoint(this, configurator, new ServiceEndpointClientCache(_clientCache));
        }
    }
}
