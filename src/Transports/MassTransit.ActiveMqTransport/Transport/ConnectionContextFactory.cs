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

        public ConnectionContextFactory(IActiveMqHostConfiguration configuration)
        {
            _configuration = configuration;
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
            var description = _configuration.Settings.ToDescription();

            return await _configuration.ReceiveTransportRetryPolicy.Retry(async () =>
            {
                if (supervisor.Stopping.IsCancellationRequested)
                    throw new OperationCanceledException($"The connection is stopping and cannot be used: {description}");

                IConnection connection = null;
                try
                {
                    TransportLogMessages.ConnectHost(description);

                    connection = _configuration.Settings.CreateConnection();

                    connection.Start();

                    LogContext.Debug?.Log("Connected: {Host} (client-id: {ClientId}, version: {Version})", description,
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
                    throw new ActiveMqConnectionException("Connection exception: " + description, ex);
                }
                catch (Exception ex)
                {
                    connection?.Dispose();
                    throw new ActiveMqConnectionException("Create Connection Faulted: " + description, ex);
                }
            }, supervisor.Stopping).ConfigureAwait(false);
        }
    }
}
