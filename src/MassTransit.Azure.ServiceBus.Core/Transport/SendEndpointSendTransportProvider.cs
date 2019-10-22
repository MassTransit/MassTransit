namespace MassTransit.Azure.ServiceBus.Core.Transport
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
            return _host.CreateSendTransport(address);
        }
    }
}
