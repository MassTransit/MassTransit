namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using MassTransit.Middleware;
    using RabbitMQ.Client;
    using Transports;
    using Util;


    public class RabbitMqConnectionContext :
        BasePipeContext,
        ConnectionContext,
        IAsyncDisposable
    {
        readonly ChannelExecutor _executor;

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

            _executor = new ChannelExecutor(1);

            connection.ConnectionShutdown += OnConnectionShutdown;
        }

        public IConnection Connection { get; }

        public string Description { get; }
        public Uri HostAddress { get; }
        public bool PublisherConfirmation { get; }

        public BatchSettings BatchSettings { get; }
        public TimeSpan ContinuationTimeout { get; }

        public TimeSpan StopTimeout { get; }

        public IRabbitMqBusTopology Topology { get; }

        public async Task<IModel> CreateModel(CancellationToken cancellationToken)
        {
            using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

            return await _executor.Run(() => Connection.CreateModel(), tokenSource.Token).ConfigureAwait(false);
        }

        public async Task<ModelContext> CreateModelContext(CancellationToken cancellationToken)
        {
            var model = await CreateModel(cancellationToken).ConfigureAwait(false);

            return new RabbitMqModelContext(this, model, cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            Connection.ConnectionShutdown -= OnConnectionShutdown;

            TransportLogMessages.DisconnectHost(Description);

            Connection.Cleanup(200, "Connection Disposed");

            TransportLogMessages.DisconnectedHost(Description);

            await _executor.DisposeAsync().ConfigureAwait(false);
        }

        void OnConnectionShutdown(object connection, ShutdownEventArgs reason)
        {
            Connection.Cleanup(reason.ReplyCode, reason.ReplyText);
        }
    }
}
