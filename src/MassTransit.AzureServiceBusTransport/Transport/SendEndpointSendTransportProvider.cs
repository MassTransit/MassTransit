namespace MassTransit.AzureServiceBusTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class SendEndpointSendTransportProvider :
        ISendTransportProvider
    {
        readonly IServiceBusHostControl _host;

        public SendEndpointSendTransportProvider(IServiceBusHostControl host)
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
