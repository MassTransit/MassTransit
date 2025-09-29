namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using MassTransit.Middleware;
    using RabbitMQ.Client;
    using Transports;


    public class RabbitMqConnectionContext :
        BasePipeContext,
        ConnectionContext,
        IAsyncDisposable
    {
        public RabbitMqConnectionContext(IConnection connection, IRabbitMqHostConfiguration hostConfiguration, string description,
            CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            Connection = connection;

            Description = description;
            HostAddress = hostConfiguration.HostAddress;

            PublisherConfirmation = hostConfiguration.PublisherConfirmation;
            BatchSettings = hostConfiguration.BatchSettings;
            ContinuationTimeout = hostConfiguration.Settings.ContinuationTimeout;

            Topology = hostConfiguration.Topology;

            StopTimeout = TimeSpan.FromSeconds(30);
        }

        public IConnection Connection { get; }

        public string Description { get; }
        public Uri HostAddress { get; }
        public bool PublisherConfirmation { get; }

        public BatchSettings BatchSettings { get; }
        public TimeSpan ContinuationTimeout { get; }

        public TimeSpan StopTimeout { get; }

        public IRabbitMqBusTopology Topology { get; }

        public async Task<IChannel> CreateChannel(ushort? concurrentMessageLimit, CancellationToken cancellationToken)
        {
            var options = new CreateChannelOptions(PublisherConfirmation, PublisherConfirmation, consumerDispatchConcurrency: concurrentMessageLimit);

            var channel = await Connection.CreateChannelAsync(options, cancellationToken).ConfigureAwait(false);

            channel.ContinuationTimeout = ContinuationTimeout;

            return channel;
        }

        public async Task<ChannelContext> CreateChannelContext(IAgent agent, ushort? concurrentMessageLimit, CancellationToken cancellationToken)
        {
            var channel = await CreateChannel(concurrentMessageLimit, cancellationToken).ConfigureAwait(false);

            return new RabbitMqChannelContext(this, channel, agent, cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            TransportLogMessages.DisconnectHost(Description);

            await Connection.Cleanup(200, "Connection Disposed").ConfigureAwait(false);

            TransportLogMessages.DisconnectedHost(Description);
        }
    }
}
