namespace MassTransit.Configuration
{
    using Microsoft.Extensions.Options;
    using Middleware;
    using QuartzIntegration;


    public class QuartzEndpointDefinition :
        IEndpointDefinition<ScheduleMessageConsumer>,
        IEndpointDefinition<CancelScheduledMessageConsumer>
    {
        readonly QuartzEndpointOptions _options;

        public QuartzEndpointDefinition(IOptions<QuartzEndpointOptions> options)
        {
            _options = options.Value;

            Partition = new Partitioner(_options.ConcurrentMessageLimit ?? _options.PrefetchCount ?? 32, new Murmur3UnsafeHashGenerator());
        }

        public IPartitioner Partition { get; }

        public virtual bool ConfigureConsumeTopology => true;

        public virtual bool IsTemporary => false;

        public virtual int? PrefetchCount => _options.PrefetchCount;

        public virtual int? ConcurrentMessageLimit => _options.ConcurrentMessageLimit;

        string IEndpointDefinition.GetEndpointName(IEndpointNameFormatter formatter)
        {
            return _options.QueueName;
        }

        public void Configure<T>(T configurator)
            where T : IReceiveEndpointConfigurator
        {
        }
    }
}
