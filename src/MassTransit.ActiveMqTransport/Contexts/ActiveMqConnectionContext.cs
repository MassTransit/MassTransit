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
            using (var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken))
            {
                var model = await Task.Factory.StartNew(() => _connection.CreateSession(), tokenSource.Token, TaskCreationOptions.None, _taskScheduler)
                    .ConfigureAwait(false);

                return model;
            }
        }

        Task IAsyncDisposable.DisposeAsync(CancellationToken cancellationToken)
        {
            LogContext.Debug?.Log("Disconnecting: {Host}", Description);

            try
            {
                _connection.Close();

                _connection.Dispose();
            }
            catch (Exception exception)
            {
                LogContext.Warning?.Log(exception, "Close Connection Faulted: {Host}", Description);
            }

            LogContext.Debug?.Log("Disconnected: {Host}", Description);

            return TaskUtil.Completed;
        }
    }
}
