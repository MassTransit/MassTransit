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
            var endpointAddress = new AmazonSqsEndpointAddress(_host.Address, address);

            return _host.CreateSendTransport(endpointAddress);
        }

        public Uri NormalizeAddress(Uri address)
        {
            return new AmazonSqsEndpointAddress(_host.Address, address);
        }
    }
}
