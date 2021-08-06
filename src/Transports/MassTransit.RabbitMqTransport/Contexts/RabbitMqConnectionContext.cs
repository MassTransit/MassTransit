namespace MassTransit.RabbitMqTransport.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using GreenPipes;
    using RabbitMQ.Client;
    using Topology;
    using Transports;
    using Util;


    public class RabbitMqConnectionContext :
        BasePipeContext,
        ConnectionContext,
        IAsyncDisposable
    {
        readonly IConnection _connection;
        readonly ChannelExecutor _executor;

        public RabbitMqConnectionContext(IConnection connection, IRabbitMqHostConfiguration hostConfiguration, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _connection = connection;

            Description = hostConfiguration.Settings.ToDescription();
            HostAddress = hostConfiguration.HostAddress;

            PublisherConfirmation = hostConfiguration.PublisherConfirmation;
            BatchSettings = hostConfiguration.BatchSettings;
            ContinuationTimeout = hostConfiguration.Settings.ContinuationTimeout;

            Topology = hostConfiguration.HostTopology;

            StopTimeout = TimeSpan.FromSeconds(30);

            _executor = new ChannelExecutor(1);

            connection.ConnectionShutdown += OnConnectionShutdown;
        }

        IConnection ConnectionContext.Connection => _connection;
        public string Description { get; }
        public Uri HostAddress { get; }
        public bool PublisherConfirmation { get; }

        public BatchSettings BatchSettings { get; }
        public TimeSpan ContinuationTimeout { get; }

        public TimeSpan StopTimeout { get; }

        public IRabbitMqHostTopology Topology { get; }

        public async Task<IModel> CreateModel(CancellationToken cancellationToken)
        {
            using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

            return await _executor.Run(() => _connection.CreateModel(), tokenSource.Token).ConfigureAwait(false);
        }

        async Task<ModelContext> ConnectionContext.CreateModelContext(CancellationToken cancellationToken)
        {
            var model = await CreateModel(cancellationToken).ConfigureAwait(false);

            return new RabbitMqModelContext(this, model, cancellationToken);
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            _connection.ConnectionShutdown -= OnConnectionShutdown;

            TransportLogMessages.DisconnectHost(Description);

            _connection.Cleanup(200, "Connection Disposed");

            TransportLogMessages.DisconnectedHost(Description);

            await _executor.DisposeAsync().ConfigureAwait(false);
        }

        void OnConnectionShutdown(object connection, ShutdownEventArgs reason)
        {
            _connection.Cleanup(reason.ReplyCode, reason.ReplyText);
        }
    }
}
