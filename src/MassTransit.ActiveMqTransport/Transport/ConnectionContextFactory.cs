namespace MassTransit.ActiveMqTransport.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Apache.NMS;
    using Configuration;
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using Policies;
    using Topology;
    using Transports;


    public class ConnectionContextFactory :
        IPipeContextFactory<ConnectionContext>
    {
        readonly IActiveMqHostConfiguration _configuration;
        readonly IActiveMqHostTopology _hostTopology;
        readonly IRetryPolicy _connectionRetryPolicy;

        public ConnectionContextFactory(IActiveMqHostConfiguration configuration, IActiveMqHostTopology hostTopology)
        {
            _configuration = configuration;
            _hostTopology = hostTopology;

            _connectionRetryPolicy = Retry.CreatePolicy(x =>
            {
                x.Handle<ActiveMqTransportException>();

                x.Exponential(1000, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(3));
            });
        }

        IPipeContextAgent<ConnectionContext> IPipeContextFactory<ConnectionContext>.CreateContext(ISupervisor supervisor)
        {
            IAsyncPipeContextAgent<ConnectionContext> asyncContext = supervisor.AddAsyncContext<ConnectionContext>();

            Task.Run(() => CreateConnection(asyncContext, supervisor), supervisor.Stopping);

            return asyncContext;
        }

        IActivePipeContextAgent<ConnectionContext> IPipeContextFactory<ConnectionContext>.CreateActiveContext(ISupervisor supervisor,
            PipeContextHandle<ConnectionContext> context, CancellationToken cancellationToken)
        {
            return supervisor.AddActiveContext(context, CreateSharedConnection(context.Context, cancellationToken));
        }

        async Task<ConnectionContext> CreateSharedConnection(Task<ConnectionContext> context, CancellationToken cancellationToken)
        {
            var connectionContext = await context.ConfigureAwait(false);

            var sharedConnection = new SharedConnectionContext(connectionContext, cancellationToken);

            return sharedConnection;
        }

        async Task<ConnectionContext> CreateConnection(IAsyncPipeContextAgent<ConnectionContext> asyncContext, IAgent supervisor)
        {
            return await _connectionRetryPolicy.Retry(async () =>
            {
                if (supervisor.Stopping.IsCancellationRequested)
                    throw new OperationCanceledException($"The connection is stopping and cannot be used: {_configuration.Description}");

                IConnection connection = null;
                try
                {
                    TransportLogMessages.ConnectHost(_configuration.Description);

                    connection = _configuration.Settings.CreateConnection();

                    void HandleConnectionException(Exception exception)
                    {
                        asyncContext.Stop($"Connection Exception: {exception}");
                    }

                    connection.ExceptionListener += HandleConnectionException;

                #pragma warning disable 4014
                    asyncContext.Completed.ContinueWith(_ => connection.ExceptionListener -= HandleConnectionException);
                #pragma warning restore 4014

                    connection.Start();

                    LogContext.Debug?.Log("Connected: {Host} (client-id: {ClientId}, version: {Version})", _configuration.Description, connection.ClientId,
                        connection.MetaData.NMSVersion);

                    var connectionContext = new ActiveMqConnectionContext(connection, _configuration, _hostTopology, supervisor.Stopped);

                    await asyncContext.Created(connectionContext).ConfigureAwait(false);

                    return connectionContext;
                }
                catch (OperationCanceledException)
                {
                    await asyncContext.CreateCanceled().ConfigureAwait(false);
                    throw;
                }
                catch (NMSConnectionException ex)
                {
                    LogContext.Error?.Log(ex, "ActiveMQ connection failed");

                    await asyncContext.CreateFaulted(ex).ConfigureAwait(false);

                    connection?.Dispose();

                    throw new ActiveMqConnectException("Connect failed: " + _configuration.Description, ex);
                }
            });
        }
    }
}
