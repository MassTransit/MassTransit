namespace MassTransit.KafkaIntegration.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Context;
    using Contexts;
    using GreenPipes;
    using Pipeline;
    using Transports;


    public class KafkaReceiver<TKey, TValue> :
        IKafkaReceiver<TKey, TValue>
        where TValue : class
    {
        readonly ReceiveEndpointContext _context;
        readonly IReceivePipeDispatcher _dispatcher;

        public KafkaReceiver(ReceiveEndpointContext context)
        {
            _context = context;

            _dispatcher = context.CreateReceivePipeDispatcher();
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("receiver");
            scope.Add("type", "eventData");
        }

        ConnectHandle IReceiveObserverConnector.ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _context.ConnectReceiveObserver(observer);
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _context.ConnectPublishObserver(observer);
        }

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            return _context.ConnectSendObserver(observer);
        }

        public async Task Handle(ConsumeResult<TKey, TValue> message, CancellationToken cancellationToken, Action<ReceiveContext> contextCallback)
        {
            var context = new KafkaReadReceiveContext<TKey, TValue>(message, _context);
            contextCallback?.Invoke(context);

            CancellationTokenRegistration registration;
            if (cancellationToken.CanBeCanceled)
                registration = cancellationToken.Register(context.Cancel);

            try
            {
                await _dispatcher.Dispatch(context).ConfigureAwait(false);
            }
            finally
            {
                registration.Dispose();
                context.Dispose();
            }
        }

        ConnectHandle IConsumeMessageObserverConnector.ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
        {
            return _context.ReceivePipe.ConnectConsumeMessageObserver(observer);
        }

        ConnectHandle IConsumeObserverConnector.ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _context.ReceivePipe.ConnectConsumeObserver(observer);
        }
    }
}
