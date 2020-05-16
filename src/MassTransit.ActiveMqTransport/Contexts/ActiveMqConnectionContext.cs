namespace MassTransit.ActiveMqTransport.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Apache.NMS;
    using Configuration;
    using Context;
    using GreenPipes;
    using Topology;
    using Transports;
    using Util;


    public class ActiveMqConnectionContext :
        BasePipeContext,
        ConnectionContext,
        IAsyncDisposable
    {
        readonly IConnection _connection;
        readonly LimitedConcurrencyLevelTaskScheduler _taskScheduler;

        public ActiveMqConnectionContext(IConnection connection, IActiveMqHostConfiguration configuration, IActiveMqHostTopology hostTopology,
            CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _connection = connection;

            Description = configuration.Description;
            HostAddress = configuration.HostAddress;

            Topology = hostTopology;

            _taskScheduler = new LimitedConcurrencyLevelTaskScheduler(1);
        }

        IConnection ConnectionContext.Connection => _connection;
        public string Description { get; }
        public Uri HostAddress { get; }
        public IActiveMqHostTopology Topology { get; }

        public async Task<ISession> CreateSession(CancellationToken cancellationToken)
        {
            using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

            var session = await Task.Factory.StartNew(() => _connection.CreateSession(AcknowledgementMode.ClientAcknowledge),
                    tokenSource.Token, TaskCreationOptions.None, _taskScheduler).ConfigureAwait(false);

            return session;
        }

        Task IAsyncDisposable.DisposeAsync(CancellationToken cancellationToken)
        {
            TransportLogMessages.DisconnectHost(Description);

            try
            {
                _connection.Close();

                _connection.Dispose();
            }
            catch (Exception exception)
            {
                LogContext.Warning?.Log(exception, "Close Connection Faulted: {Host}", Description);
            }

            TransportLogMessages.DisconnectedHost(Description);

            return TaskUtil.Completed;
        }
    }
}
