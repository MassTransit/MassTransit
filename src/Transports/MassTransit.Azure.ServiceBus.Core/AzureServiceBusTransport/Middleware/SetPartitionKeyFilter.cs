namespace MassTransit.AzureServiceBusTransport.Middleware
{
    using System.Threading.Tasks;


    public class SetPartitionKeyFilter<T> :
        IFilter<ServiceBusSendContext<T>>
        where T : class
    {
        readonly IMessagePartitionKeyFormatter<T> _partitionKeyFormatter;

        public SetPartitionKeyFilter(IMessagePartitionKeyFormatter<T> partitionKeyFormatter)
        {
            _partitionKeyFormatter = partitionKeyFormatter;
        }

        public Task Send(ServiceBusSendContext<T> context, IPipe<ServiceBusSendContext<T>> next)
        {
            var partitionKey = _partitionKeyFormatter.FormatPartitionKey(context);

            if (!string.IsNullOrWhiteSpace(partitionKey))
                context.PartitionKey = partitionKey;

            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("setPartitionKey");
        }
    }
}
