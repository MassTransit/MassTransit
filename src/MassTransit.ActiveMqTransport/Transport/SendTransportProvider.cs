namespace MassTransit.ActiveMqTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class SendTransportProvider :
        ISendTransportProvider
    {
        readonly IActiveMqHostControl _host;

        public SendTransportProvider(IActiveMqHostControl host)
        {
            _host = host;
        }

        Task<ISendTransport> ISendTransportProvider.GetSendTransport(Uri address)
        {
            return _host.CreateSendTransport(address);
        }

        public Uri NormalizeAddress(Uri address)
        {
            return address;
        }
    }
}
