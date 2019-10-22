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

        Task<ISendTransport> ISendTransportProvider.GetSendTransport(Uri address)
        {
            return _host.CreateSendTransport(address);
        }
    }
}
