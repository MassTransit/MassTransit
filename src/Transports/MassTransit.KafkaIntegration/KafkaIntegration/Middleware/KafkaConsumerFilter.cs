namespace MassTransit.KafkaIntegration.Middleware
{
    using System.Linq;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using MassTransit.Middleware;
    using Transports;


    public class KafkaConsumerFilter<TKey, TValue> :
        IFilter<ConsumerContext>
        where TValue : class
    {
        readonly KafkaReceiveEndpointContext<TKey, TValue> _context;

        public KafkaConsumerFilter(KafkaReceiveEndpointContext<TKey, TValue> context)
        {
            _context = context;
        }

        public async Task Send(ConsumerContext context, IPipe<ConsumerContext> next)
        {
            var receiveSettings = _context.GetPayload<ReceiveSettings>();
            var consumers = new IKafkaMessageConsumer<TKey, TValue>[receiveSettings.ConcurrentConsumerLimit];
            for (var i = 0; i < consumers.Length; i++)
                consumers[i] = new KafkaMessageConsumer<TKey, TValue>(receiveSettings, _context, context);

            var supervisor = CreateConsumerSupervisor(context, consumers);

            await _context.TransportObservers.NotifyReady(_context.InputAddress).ConfigureAwait(false);

            try
            {
                await supervisor.Completed.ConfigureAwait(false);
            }
            finally
            {
                DeliveryMetrics metrics = new CombinedDeliveryMetrics(consumers);

                await _context.TransportObservers.NotifyCompleted(_context.InputAddress, metrics).ConfigureAwait(false);

                _context.LogConsumerCompleted(metrics.DeliveryCount, metrics.ConcurrentDeliveryCount);
            }

            await next.Send(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
        }

        Supervisor CreateConsumerSupervisor(ConsumerContext context, IKafkaMessageConsumer<TKey, TValue>[] actualConsumers)
        {
            var supervisor = new Supervisor();

            foreach (IKafkaMessageConsumer<TKey, TValue> consumer in actualConsumers)
                supervisor.Add(consumer);

            _context.AddConsumeAgent(supervisor);

            void HandleError(Error exception)
            {
                supervisor.Stop(exception.Reason);
            }

            context.ErrorHandler += HandleError;

            supervisor.SetReady();

            supervisor.Completed.ContinueWith(task => context.ErrorHandler -= HandleError,
                TaskContinuationOptions.ExecuteSynchronously);

            return supervisor;
        }


        class CombinedDeliveryMetrics :
            DeliveryMetrics
        {
            public CombinedDeliveryMetrics(IKafkaMessageConsumer<TKey, TValue>[] receivers)
            {
                DeliveryCount = receivers.Sum(x => x.DeliveryCount);
                ConcurrentDeliveryCount = receivers.Sum(x => x.ConcurrentDeliveryCount);
            }

            public long DeliveryCount { get; }
            public int ConcurrentDeliveryCount { get; }
        }
    }
}
