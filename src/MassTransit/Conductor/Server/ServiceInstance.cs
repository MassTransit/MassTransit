namespace MassTransit.Conductor.Server
{
    using System;
    using System.Collections.Generic;
    using Util;


    public class ServiceInstance :
        IServiceInstance
    {
        readonly IServiceInstanceClientCache _clientCache;

        public ServiceInstance(IReadOnlyDictionary<string, string> instanceAttributes)
        {
            var instanceId = NewId.Next();

            InstanceId = instanceId.ToGuid();
            InstanceName = instanceId.ToString(FormatUtil.Formatter);
            InstanceAttributes = instanceAttributes;

            _clientCache = new ServiceInstanceClientCache();
        }

        public Guid InstanceId { get; }
        public string InstanceName { get; }
        public IReadOnlyDictionary<string, string> InstanceAttributes { get; }

        public IServiceEndpoint CreateServiceEndpoint(IReceiveEndpointConfigurator configurator)
        {
            return new ServiceEndpoint(this, configurator, new ServiceEndpointClientCache(_clientCache));
        }
    }
}
