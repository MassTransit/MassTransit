namespace MassTransit.AmazonSqsTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class SendTransportProvider :
        ISendTransportProvider
    {
        readonly IAmazonSqsHostControl _host;

        public SendTransportProvider(IAmazonSqsHostControl host)
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
