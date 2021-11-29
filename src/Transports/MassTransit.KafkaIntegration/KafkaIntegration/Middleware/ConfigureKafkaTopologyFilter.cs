namespace MassTransit.KafkaIntegration.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Confluent.Kafka.Admin;
    using Logging;


    public class ConfigureKafkaTopologyFilter<TKey, TValue> :
        IFilter<ConsumerContext<TKey, TValue>>
        where TValue : class
    {
        readonly AdminClientConfig _config;
        readonly KafkaTopicOptions _options;
        readonly TopicSpecification _specification;

        public ConfigureKafkaTopologyFilter(IReadOnlyDictionary<string, string> clientConfig, KafkaTopicOptions options)
        {
            _options = options;
            _specification = _options.ToSpecification();
            _config = new AdminClientConfig(clientConfig.ToDictionary(x => x.Key, x => x.Value));
        }

        public async Task Send(ConsumerContext<TKey, TValue> context, IPipe<ConsumerContext<TKey, TValue>> next)
        {
            await context.OneTimeSetup<ConfigureTopologyContext<TKey, TValue>>(_ => CreateTopic(), () => new Context()).ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("configureTopology");
            scope.Add("specifications", _options);
        }

        async Task CreateTopic()
        {
            var client = new AdminClientBuilder(_config).Build();
            try
            {
                var options = new CreateTopicsOptions { RequestTimeout = _options.RequestTimeout };
                LogContext.Debug?.Log("Creating topic: {Topic}", _specification.Name);
                await client.CreateTopicsAsync(new[] { _specification }, options).ConfigureAwait(false);
            }
            catch (CreateTopicsException e)
            {
                if (!e.Results.All(x => x.Error.Reason.EndsWith("already exists.", StringComparison.OrdinalIgnoreCase)))
                {
                    EnabledLogger? logger = e.Error.IsFatal ? LogContext.Error : LogContext.Debug;
                    logger?.Log("An error occured creating topics. {Errors}", string.Join(", ", e.Results.Select(x => $"{x.Topic}:{x.Error.Reason}")));
                }
            }
            finally
            {
                client.Dispose();
            }
        }


        class Context :
            ConfigureTopologyContext<TKey, TValue>
        {
        }
    }
}
