namespace MassTransit.Mediator.Contexts
{
    using System.Threading.Tasks;
    using Context;
    using Observables;
    using Transports;


    /// <summary>
    /// </summary>
    public class MediatorPublishSendEndpoint :
        SendEndpointProxy,
        IPublishObserverConnector
    {
        readonly PublishObservable _observers;
        readonly IPublishPipe _publishPipe;

        public MediatorPublishSendEndpoint(ISendEndpoint endpoint, IPublishPipe publishPipe)
            : base(endpoint)
        {
            _publishPipe = publishPipe;

            _observers = new PublishObservable();
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _observers.Connect(observer);
        }

        protected override IPipe<SendContext<T>> GetPipeProxy<T>(IPipe<SendContext<T>> pipe = default)
        {
            return new PublishPipeAdapter<T>(_publishPipe, pipe);
        }


        class PublishPipeAdapter<T> :
            IPipe<SendContext<T>>
            where T : class
        {
            readonly IPipe<SendContext<T>> _pipe;
            readonly IPublishPipe _publishPipe;

            public PublishPipeAdapter(IPublishPipe publishPipe, IPipe<SendContext<T>> pipe)
            {
                _publishPipe = publishPipe;
                _pipe = pipe;
            }

            void IProbeSite.Probe(ProbeContext context)
            {
                _pipe.Probe(context);
            }

            public async Task Send(SendContext<T> context)
            {
                var publishContext = context.GetPayload<MessageSendContext<T>>();

                publishContext.IsPublish = true;

                await _publishPipe.Send(publishContext).ConfigureAwait(false);

                if (_pipe.IsNotEmpty())
                    await _pipe.Send(context).ConfigureAwait(false);
            }
        }
    }
}
