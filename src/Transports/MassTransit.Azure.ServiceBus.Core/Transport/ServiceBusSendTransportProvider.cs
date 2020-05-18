namespace MassTransit.Azure.ServiceBus.Core.Transport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class ServiceBusSendTransportProvider :
        ISendTransportProvider
    {
        readonly IServiceBusHostControl _host;

        public ServiceBusSendTransportProvider(IServiceBusHostControl host)
        {
            _host = host;
        }

        Task<ISendTransport> ISendTransportProvider.GetSendTransport(Uri address)
        {
            var endpointAddress = new ServiceBusEndpointAddress(_host.Address, address);

            return _host.CreateSendTransport(endpointAddress);
        }

        public Uri NormalizeAddress(Uri address)
        {
            return new ServiceBusEndpointAddress(_host.Address, address);
        }
    }
}
