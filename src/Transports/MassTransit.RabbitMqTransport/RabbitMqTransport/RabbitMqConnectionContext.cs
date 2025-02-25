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

        public async Task<IChannel> CreateChannel(CancellationToken cancellationToken, ushort? concurrentMessageLimit)
        {
            using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

            var options = new CreateChannelOptions(PublisherConfirmation, PublisherConfirmation, consumerDispatchConcurrency: concurrentMessageLimit);

            return await Connection.CreateChannelAsync(options, cancellationToken).ConfigureAwait(false);
        }

        public async Task<ChannelContext> CreateChannelContext(CancellationToken cancellationToken, ushort? concurrentMessageLimit)
        {
            var channel = await CreateChannel(cancellationToken, concurrentMessageLimit).ConfigureAwait(false);

            return new RabbitMqChannelContext(this, channel, cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            TransportLogMessages.DisconnectHost(Description);

            await Connection.Cleanup(200, "Connection Disposed").ConfigureAwait(false);

            TransportLogMessages.DisconnectedHost(Description);
        }
    }
}
