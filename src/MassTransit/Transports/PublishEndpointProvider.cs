namespace MassTransit.Transports
{
    using System;
    using System.Threading.Tasks;
    using Observables;


    public class PublishEndpointProvider :
        IPublishEndpointProvider
    {
        readonly ISendEndpointCache<Type> _cache;
        readonly ReceiveEndpointContext _context;
        readonly Uri _hostAddress;
        readonly PublishObservable _publishObservers;
        readonly IPublishTopology _publishTopology;
        readonly ISendPipe _sendPipe;
        readonly IPublishTransportProvider _transportProvider;

        public PublishEndpointProvider(IPublishTransportProvider transportProvider, Uri hostAddress, PublishObservable publishObservers,
            ReceiveEndpointContext context, IPublishPipe publishPipe, IPublishTopology publishTopology)
        {
            _transportProvider = transportProvider;
            _hostAddress = hostAddress;
            _publishTopology = publishTopology;
            _publishObservers = publishObservers;
            _context = context;

            _sendPipe = new PipeAdapter(publishPipe);

            _cache = new SendEndpointCache<Type>();
        }

        public Task<ISendEndpoint> GetPublishSendEndpoint<T>()
            where T : class
        {
            return _cache.GetSendEndpoint(typeof(T), type => CreateSendEndpoint<T>());
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _publishObservers.Connect(observer);
        }

        Task<ISendEndpoint> CreateSendEndpoint<T>()
            where T : class
        {
            IMessagePublishTopology<T> messageTopology = _publishTopology.GetMessageTopology<T>();

            if (!messageTopology.TryGetPublishAddress(_hostAddress, out var publishAddress))
                throw new PublishException($"An address for publishing message type {TypeCache<T>.ShortName} was not found.");

            Task<ISendTransport> sendTransportTask = _transportProvider.GetPublishTransport<T>(publishAddress);
            if (sendTransportTask.Status == TaskStatus.RanToCompletion)
            {
                var sendTransport = sendTransportTask.Result;

                var sendEndpoint = new SendEndpoint(sendTransport, _context, publishAddress, _sendPipe, sendTransport.ConnectSendObserver(_publishObservers));

                return Task.FromResult<ISendEndpoint>(sendEndpoint);
            }

            async Task<ISendEndpoint> CreateAsync()
            {
                var sendTransport = await sendTransportTask.ConfigureAwait(false);

                return new SendEndpoint(sendTransport, _context, publishAddress, _sendPipe, sendTransport.ConnectSendObserver(_publishObservers));
            }

            return CreateAsync();
        }


        class PipeAdapter :
            ISendPipe
        {
            readonly IPublishPipe _publishPipe;

            public PipeAdapter(IPublishPipe publishPipe)
            {
                _publishPipe = publishPipe;
            }

            public Task Send<T>(SendContext<T> context)
                where T : class
            {
                var publishContext = context.GetPayload<PublishContext<T>>();

                return _publishPipe.Send(publishContext);
            }

            public void Probe(ProbeContext context)
            {
                _publishPipe.Probe(context);
            }
        }
    }
}
