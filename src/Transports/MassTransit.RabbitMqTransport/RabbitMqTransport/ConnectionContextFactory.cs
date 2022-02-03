namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Agents;
    using Configuration;
    using Internals;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Exceptions;
    using Transports;


    public class ConnectionContextFactory :
        IPipeContextFactory<ConnectionContext>
    {
        readonly Lazy<ConnectionFactory> _connectionFactory;
        readonly IRabbitMqHostConfiguration _hostConfiguration;

        public ConnectionContextFactory(IRabbitMqHostConfiguration hostConfiguration)
        {
            _hostConfiguration = hostConfiguration;

            _connectionFactory = new Lazy<ConnectionFactory>(() => _hostConfiguration.Settings.GetConnectionFactory());
        }

        IPipeContextAgent<ConnectionContext> IPipeContextFactory<ConnectionContext>.CreateContext(ISupervisor supervisor)
        {
            Task<ConnectionContext> context = Task.Run(() => CreateConnection(supervisor), supervisor.Stopped);

            IPipeContextAgent<ConnectionContext> contextHandle = supervisor.AddContext(context);

            void HandleShutdown(object sender, ShutdownEventArgs args)
            {
                if (args.Initiator != ShutdownInitiator.Application)
                    contextHandle.Stop(args.ReplyText);
            }

            context.ContinueWith(task =>
            {
                task.Result.Connection.ConnectionShutdown += HandleShutdown;

                contextHandle.Completed.ContinueWith(_ => task.Result.Connection.ConnectionShutdown -= HandleShutdown);
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
            await _hostConfiguration.Settings.Refresh(_connectionFactory.Value).ConfigureAwait(false);

            var description = _hostConfiguration.Settings.ToDescription(_connectionFactory.Value);

            if (supervisor.Stopping.IsCancellationRequested)
                throw new RabbitMqConnectionException($"The connection is stopping and cannot be used: {description}");

            IConnection connection = null;
            try
            {
                TransportLogMessages.ConnectHost(description);

                if (_hostConfiguration.Settings.EndpointResolver != null)
                {
                    connection = _connectionFactory.Value.CreateConnection(_hostConfiguration.Settings.EndpointResolver,
                        _hostConfiguration.Settings.ClientProvidedName);
                }
                else
                {
                    var hostNames = new List<string>(1) { _hostConfiguration.Settings.Host };

                    connection = _connectionFactory.Value.CreateConnection(hostNames, _hostConfiguration.Settings.ClientProvidedName);
                }

                LogContext.Debug?.Log("Connected: {Host} (address: {RemoteAddress}, local: {LocalAddress})", description, connection.Endpoint,
                    connection.LocalPort);

                var connectionContext = new RabbitMqConnectionContext(connection, _hostConfiguration, description, supervisor.Stopped);

                connectionContext.GetOrAddPayload(() => _hostConfiguration.Settings);

                return connectionContext;
            }
            catch (ConnectFailureException ex)
            {
                connection?.Dispose();

                LogContext.Warning?.Log(ex, "Connection Failed: {InputAddress}", _hostConfiguration.HostAddress);

                throw new RabbitMqConnectionException("Connect failed: " + description, ex);
            }
            catch (BrokerUnreachableException ex)
            {
                connection?.Dispose();

                LogContext.Warning?.Log(ex, "Connection Failed: {InputAddress}", _hostConfiguration.HostAddress);

                throw new RabbitMqConnectionException("Broker unreachable: " + description, ex);
            }
            catch (OperationInterruptedException ex)
            {
                connection?.Dispose();

                LogContext.Warning?.Log(ex, "Connection Failed: {InputAddress}", _hostConfiguration.HostAddress);

                throw new RabbitMqConnectionException("Operation interrupted: " + description, ex);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                connection?.Dispose();

                LogContext.Warning?.Log(ex, "Connection Failed: {InputAddress}", _hostConfiguration.HostAddress);

                throw new RabbitMqConnectionException("Create Connection Faulted: " + description, ex);
            }
        }
    }
}
