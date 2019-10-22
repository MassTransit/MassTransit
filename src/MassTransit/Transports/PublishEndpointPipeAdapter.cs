namespace MassTransit.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Pipeline;
    using Pipeline.Observables;


    public class PublishEndpointPipeAdapter<T> :
        IPipe<SendContext<T>>
        where T : class
    {
        readonly PublishObservable _observer;
        readonly T _message;
        readonly IPipe<PublishContext<T>> _pipe;
        readonly IPublishPipe _publishPipe;
        readonly Uri _sourceAddress;
        readonly ConsumeContext _consumeContext;
        PublishContext<T> _context;

        public PublishEndpointPipeAdapter(T message, IPipe<PublishContext<T>> pipe, IPublishPipe publishPipe, PublishObservable observer, Uri sourceAddress,
            ConsumeContext consumeContext)
        {
            _message = message;
            _pipe = pipe;
            _publishPipe = publishPipe;
            _observer = observer;
            _sourceAddress = sourceAddress;
            _consumeContext = consumeContext;
        }

        public PublishEndpointPipeAdapter(T message, IPipe<PublishContext> pipe, IPublishPipe publishPipe, PublishObservable observer, Uri sourceAddress,
            ConsumeContext consumeContext)
        {
            _message = message;
            _pipe = pipe;
            _publishPipe = publishPipe;
            _observer = observer;
            _sourceAddress = sourceAddress;
            _consumeContext = consumeContext;
        }

        public PublishEndpointPipeAdapter(T message, IPublishPipe publishPipe, PublishObservable observer, Uri sourceAddress, ConsumeContext consumeContext)
        {
            _message = message;
            _pipe = Pipe.Empty<PublishContext<T>>();
            _publishPipe = publishPipe;
            _observer = observer;
            _sourceAddress = sourceAddress;
            _consumeContext = consumeContext;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _pipe.Probe(context);
        }

        public async Task Send(SendContext<T> context)
        {
            context.SourceAddress = _sourceAddress;

            if (_consumeContext != null)
                context.TransferConsumeContextHeaders(_consumeContext);

            var publishContext = context.GetPayload<PublishContext<T>>();

            var firstTime = Interlocked.CompareExchange(ref _context, publishContext, null) == null;

            await _publishPipe.Send(publishContext).ConfigureAwait(false);

            if (_pipe.IsNotEmpty())
                await _pipe.Send(publishContext).ConfigureAwait(false);

            if (firstTime && _observer.Count > 0)
                await _observer.PrePublish(publishContext).ConfigureAwait(false);
        }

        public Task PostPublish()
        {
            return _observer.PostPublish(_context ?? GetDefaultPublishContext());
        }

        public Task PublishFaulted(Exception exception)
        {
            return _observer.PublishFault(_context ?? GetDefaultPublishContext(), exception);
        }

        public int ObserverCount => _observer.Count;

        PublishContext<T> GetDefaultPublishContext()
        {
            return new FaultedPublishContext<T>(_message, CancellationToken.None)
            {
                SourceAddress = _consumeContext?.ReceiveContext?.InputAddress ?? _sourceAddress,
                ConversationId = _consumeContext?.ConversationId,
                InitiatorId = _consumeContext?.CorrelationId ?? _consumeContext?.RequestId,
                Mandatory = _context?.Mandatory ?? false
            };
        }
    }
}
