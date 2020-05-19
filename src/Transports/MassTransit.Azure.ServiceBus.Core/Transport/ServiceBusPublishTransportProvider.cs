namespace MassTransit.Azure.ServiceBus.Core.Transport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class ServiceBusPublishTransportProvider :
        IPublishTransportProvider
    {
        readonly IServiceBusHostControl _host;

        public ServiceBusPublishTransportProvider(IServiceBusHostControl host)
        {
            _host = host;
        }

        Task<ISendTransport> IPublishTransportProvider.GetPublishTransport<T>(Uri publishAddress)
        {
            return _host.CreatePublishTransport<T>();
        }
    }
}
