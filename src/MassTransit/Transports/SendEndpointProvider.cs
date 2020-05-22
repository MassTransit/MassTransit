namespace MassTransit.Transports
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using Pipeline;
    using Pipeline.Observables;


    public class SendEndpointProvider :
        ISendEndpointProvider
    {
        readonly ISendEndpointCache<Uri> _cache;
        readonly SendObservable _observers;
        readonly ISendPipe _sendPipe;
        readonly IMessageSerializer _serializer;
        readonly Uri _sourceAddress;
        readonly ISendTransportProvider _provider;

        public SendEndpointProvider(ISendTransportProvider provider, SendObservable observers, IMessageSerializer serializer, Uri sourceAddress,
            ISendPipe sendPipe)
        {
            _provider = provider;
            _serializer = serializer;
            _sourceAddress = sourceAddress;
            _sendPipe = sendPipe;

            _cache = new SendEndpointCache<Uri>();
            _observers = observers;
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            address = _provider.NormalizeAddress(address);

            return _cache.GetSendEndpoint(address, CreateSendEndpoint);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _observers.Connect(observer);
        }

        async Task<ISendEndpoint> CreateSendEndpoint(Uri address)
        {
            var sendTransport = await _provider.GetSendTransport(address).ConfigureAwait(false);

            var handle = sendTransport.ConnectSendObserver(_observers);

            return new SendEndpoint(sendTransport, _serializer, address, _sourceAddress, _sendPipe, handle);
        }
    }
}
