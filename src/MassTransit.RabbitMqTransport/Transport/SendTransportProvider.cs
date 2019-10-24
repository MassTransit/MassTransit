namespace MassTransit.RabbitMqTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class SendTransportProvider :
        ISendTransportProvider
    {
        readonly IRabbitMqHostControl _host;

        public SendTransportProvider(IRabbitMqHostControl host)
        {
            _host = host;
        }

        public Uri NormalizeAddress(Uri address)
        {
            return new RabbitMqEndpointAddress(_host.Address, address);
        }

        Task<ISendTransport> ISendTransportProvider.GetSendTransport(Uri address)
        {
            var endpointAddress = new RabbitMqEndpointAddress(_host.Address, address);

            return _host.CreateSendTransport(endpointAddress);
        }
    }
}
