namespace MassTransit.ActiveMqTransport.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Apache.NMS;
    using Configuration;
    using Context;
    using GreenPipes;
    using GreenPipes.Payloads;
    using Topology;
    using Util;


    public class ActiveMqConnectionContext :
        BasePipeContext,
        ConnectionContext,
        IAsyncDisposable
    {
        readonly IConnection _connection;
        readonly IActiveMqHostConfiguration _configuration;
        readonly LimitedConcurrencyLevelTaskScheduler _taskScheduler;

        public ActiveMqConnectionContext(IConnection connection, IActiveMqHostConfiguration configuration, CancellationToken cancellationToken)
            : base(new PayloadCache(), cancellationToken)
        {
            _connection = connection;
            _configuration = configuration;

            _taskScheduler = new LimitedConcurrencyLevelTaskScheduler(1);
        }

        public string Description => _configuration.Description;
        public Uri HostAddress => _configuration.HostAddress;
        public IActiveMqHostTopology Topology => _configuration.Topology;

        public Task<ISession> CreateSession()
        {
            return Task.Factory.StartNew(() => _connection.CreateSession(), CancellationToken, TaskCreationOptions.None, _taskScheduler);
        }

        IConnection ConnectionContext.Connection => _connection;

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
