namespace MassTransit.KafkaIntegration.Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Confluent.Kafka.Admin;
    using Context;
    using Contexts;
    using GreenPipes;
    using Logging;
    using Transport;


    public class ConfigureTopologyFilter<TKey, TValue> :
        IFilter<ConsumerContext<TKey, TValue>>
        where TValue : class
    {
        readonly AdminClientConfig _config;
        readonly KafkaTopicOptions _options;
        readonly TopicSpecification _specification;

        public ConfigureTopologyFilter(IReadOnlyDictionary<string, string> clientConfig, KafkaTopicOptions options)
        {
            _options = options;
            _specification = _options.ToSpecification();
            _config = new AdminClientConfig(clientConfig.ToDictionary(x => x.Key, x => x.Value));
        }

        public async Task Send(ConsumerContext<TKey, TValue> context, IPipe<ConsumerContext<TKey, TValue>> next)
        {
            await context.OneTimeSetup<ConfigureTopologyContext<TKey, TValue>>(_ => CreateTopic(), () => new Context()).ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);

            if (_options.IsTemporary)
                await DeleteTopic().ConfigureAwait(false);
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
                var options = new CreateTopicsOptions {RequestTimeout = TimeSpan.FromSeconds(30)};
                LogContext.Debug?.Log("Creating topic: {Topic}", _specification.Name);
                await client.CreateTopicsAsync(new[] {_specification}, options).ConfigureAwait(false);
            }
            catch (CreateTopicsException e)
            {
                EnabledLogger? logger = e.Error.IsFatal ? LogContext.Critical : LogContext.Error;
                logger?.Log("An error occured creating topics. {Errors}", string.Join(", ", e.Results.Select(x => $"{x.Topic}:{x.Error.Reason}")));
            }
            finally
            {
                client.Dispose();
            }
        }

        async Task DeleteTopic()
        {
            var client = new AdminClientBuilder(_config).Build();
            try
            {
                var options = new DeleteTopicsOptions {RequestTimeout = TimeSpan.FromSeconds(30)};
                LogContext.Debug?.Log("Deleting topic: {Topic}", _specification.Name);
                await client.DeleteTopicsAsync(new[] {_specification.Name}, options).ConfigureAwait(false);
            }
            catch (DeleteTopicsException e)
            {
                EnabledLogger? logger = e.Error.IsFatal ? LogContext.Critical : LogContext.Error;
                logger?.Log("An error occured deleting topics. {Errors}", string.Join(", ", e.Results.Select(x => $"{x.Topic}:{x.Error.Reason}")));
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
