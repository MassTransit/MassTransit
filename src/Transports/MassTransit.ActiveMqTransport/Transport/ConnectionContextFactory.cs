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
    using GreenPipes.Internals.Extensions;
    using Policies;
    using Transports;


    public class ConnectionContextFactory :
        IPipeContextFactory<ConnectionContext>
    {
        readonly IActiveMqHostConfiguration _configuration;
        readonly IRetryPolicy _connectionRetryPolicy;

        public ConnectionContextFactory(IActiveMqHostConfiguration configuration)
        {
            _configuration = configuration;

            _connectionRetryPolicy = Retry.CreatePolicy(x =>
            {
                x.Handle<ActiveMqTransportException>();

                x.Exponential(1000, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(3));
            });
        }

        IPipeContextAgent<ConnectionContext> IPipeContextFactory<ConnectionContext>.CreateContext(ISupervisor supervisor)
        {
            Task<ConnectionContext> context = Task.Run(() => CreateConnection(supervisor), supervisor.Stopped);

            IPipeContextAgent<ConnectionContext> contextHandle = supervisor.AddContext(context);

            void HandleConnectionException(Exception exception)
            {
                contextHandle.Stop($"Connection Exception: {exception}");
            }

            context.ContinueWith(task =>
            {
                task.Result.Connection.ExceptionListener += HandleConnectionException;

                contextHandle.Completed.ContinueWith(_ => task.Result.Connection.ExceptionListener -= HandleConnectionException);
            }, TaskContinuationOptions.OnlyOnRanToCompletion);

            return contextHandle;
        }

        IActivePipeContextAgent<ConnectionContext> IPipeContextFactory<ConnectionContext>.CreateActiveContext(ISupervisor supervisor,
            PipeContextHandle<ConnectionContext> context, CancellationToken cancellationToken)
        {
            return supervisor.AddActiveContext(context, CreateSharedConnection(context.Context, cancellationToken));
        }

        static async Task<ConnectionContext> CreateSharedConnection(Task<ConnectionContext> context, CancellationToken cancellationToken)
        {
            return context.IsCompletedSuccessfully()
                ? new SharedConnectionContext(context.Result, cancellationToken)
                : new SharedConnectionContext(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
        }

        async Task<ConnectionContext> CreateConnection(ISupervisor supervisor)
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

                    connection.Start();

                    LogContext.Debug?.Log("Connected: {Host} (client-id: {ClientId}, version: {Version})", _configuration.Description,
                        connection.ClientId, connection.MetaData.NMSVersion);

                    return new ActiveMqConnectionContext(connection, _configuration, supervisor.Stopped);
                }
                catch (OperationCanceledException)
                {
                    connection?.Dispose();
                    throw;
                }
                catch (NMSConnectionException ex)
                {
                    connection?.Dispose();
                    throw new ActiveMqConnectException("Connection exception: " + _configuration.Description, ex);
                }
                catch (Exception ex)
                {
                    connection?.Dispose();
                    throw new ActiveMqConnectException("Create Connection Faulted: " + _configuration.Description, ex);
                }
            }, supervisor.Stopping).ConfigureAwait(false);
        }
    }
}
