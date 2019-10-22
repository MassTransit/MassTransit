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
    using Topology;


    public class ConnectionContextFactory :
        IPipeContextFactory<ConnectionContext>
    {
        readonly IActiveMqHostConfiguration _configuration;
        readonly IActiveMqHostTopology _hostTopology;

        public ConnectionContextFactory(IActiveMqHostConfiguration configuration, IActiveMqHostTopology hostTopology)
        {
            _configuration = configuration;
            _hostTopology = hostTopology;
        }

        IPipeContextAgent<ConnectionContext> IPipeContextFactory<ConnectionContext>.CreateContext(ISupervisor supervisor)
        {
            IAsyncPipeContextAgent<ConnectionContext> asyncContext = supervisor.AddAsyncContext<ConnectionContext>();

            var context = Task.Factory.StartNew(() => CreateConnection(asyncContext, supervisor), supervisor.Stopping, TaskCreationOptions.None,
                TaskScheduler.Default).Unwrap();

            void HandleConnectionException(Exception exception)
            {
                asyncContext.Stop($"Connection Exception: {exception}");
            }

            context.ContinueWith(task =>
            {
                task.Result.Connection.ExceptionListener += HandleConnectionException;

                asyncContext.Completed.ContinueWith(_ => task.Result.Connection.ExceptionListener -= HandleConnectionException);
            }, TaskContinuationOptions.OnlyOnRanToCompletion);

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
            IConnection connection = null;
            try
            {
                if (supervisor.Stopping.IsCancellationRequested)
                    throw new OperationCanceledException($"The connection is stopping and cannot be used: {_configuration.Description}");

                LogContext.Debug?.Log("Connecting: {Host}", _configuration.Description);

                connection = _configuration.Settings.CreateConnection();

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
        }
    }
}
