namespace MassTransit.Transports
{
    using System;
    using System.Threading.Tasks;
    using Observables;


    public class SendEndpointProvider :
        ISendEndpointProvider
    {
        readonly ISendEndpointCache<Uri> _cache;
        readonly ReceiveEndpointContext _context;
        readonly SendObservable _observers;
        readonly ISendTransportProvider _provider;
        readonly ISendPipe _sendPipe;

        public SendEndpointProvider(ISendTransportProvider provider, SendObservable observers, ReceiveEndpointContext context, ISendPipe sendPipe)
        {
            _provider = provider;
            _sendPipe = sendPipe;

            _observers = observers;
            _context = context;

            _cache = new SendEndpointCache<Uri>();
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

            return new SendEndpoint(sendTransport, _context, address, _sendPipe, handle);
        }
    }
}
