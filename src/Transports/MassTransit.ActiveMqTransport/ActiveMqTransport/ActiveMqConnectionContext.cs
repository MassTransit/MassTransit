namespace MassTransit.ActiveMqTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Apache.NMS;
    using Configuration;
    using MassTransit.Middleware;
    using Transports;
    using Util;


    public class ActiveMqConnectionContext :
        BasePipeContext,
        ConnectionContext,
        IAsyncDisposable
    {
        readonly IConnection _connection;
        readonly ChannelExecutor _executor;

        public ActiveMqConnectionContext(IConnection connection, IActiveMqHostConfiguration hostConfiguration, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _connection = connection;

            Description = hostConfiguration.Settings.ToDescription();
            HostAddress = hostConfiguration.HostAddress;

            Topology = hostConfiguration.Topology;

            _executor = new ChannelExecutor(1);
        }

        IConnection ConnectionContext.Connection => _connection;
        public string Description { get; }
        public Uri HostAddress { get; }
        public IActiveMqBusTopology Topology { get; }

        public async Task<ISession> CreateSession(CancellationToken cancellationToken)
        {
            using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

            return await _executor.Run(() => _connection.CreateSession(AcknowledgementMode.IndividualAcknowledge), tokenSource.Token).ConfigureAwait(false);
        }

        public async ValueTask DisposeAsync()
        {
            TransportLogMessages.DisconnectHost(Description);

            try
            {
                _connection.Close();

                TransportLogMessages.DisconnectedHost(Description);

                _connection.Dispose();

                await _executor.DisposeAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                LogContext.Warning?.Log(exception, "Close Connection Faulted: {Host}", Description);
            }
        }
    }
}
