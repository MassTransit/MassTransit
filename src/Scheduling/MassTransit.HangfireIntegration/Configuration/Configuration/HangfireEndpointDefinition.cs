namespace MassTransit.Configuration
{
    using HangfireIntegration;
    using Microsoft.Extensions.Options;
    using Middleware;


    public class HangfireEndpointDefinition :
        IEndpointDefinition<ScheduleMessageConsumer>,
        IEndpointDefinition<ScheduleRecurringMessageConsumer>
    {
        readonly HangfireEndpointOptions _options;

        public HangfireEndpointDefinition(IOptions<HangfireEndpointOptions> options)
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
