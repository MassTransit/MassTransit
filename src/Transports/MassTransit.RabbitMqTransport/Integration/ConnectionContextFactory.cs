namespace MassTransit.RabbitMqTransport.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using Policies;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Exceptions;
    using Topology;
    using Transports;


    public class ConnectionContextFactory :
        IPipeContextFactory<ConnectionContext>
    {
        readonly IRabbitMqHostConfiguration _configuration;
        readonly IRabbitMqHostTopology _hostTopology;
        readonly Lazy<ConnectionFactory> _connectionFactory;
        readonly string _description;
        readonly IRetryPolicy _connectionRetryPolicy;

        public ConnectionContextFactory(IRabbitMqHostConfiguration configuration, IRabbitMqHostTopology hostTopology)
        {
            _configuration = configuration;
            _hostTopology = hostTopology;

            _description = configuration.Settings.ToDescription();

            _connectionRetryPolicy = Retry.CreatePolicy(x =>
            {
                x.Handle<RabbitMqConnectionException>();
                x.Ignore<AuthenticationFailureException>();

                x.Exponential(1000, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(3));
            });

            _connectionFactory = new Lazy<ConnectionFactory>(_configuration.Settings.GetConnectionFactory);
        }

        IPipeContextAgent<ConnectionContext> IPipeContextFactory<ConnectionContext>.CreateContext(ISupervisor supervisor)
        {
            var context = Task.Run(() => CreateConnection(supervisor), supervisor.Stopped);

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
            return await _connectionRetryPolicy.Retry(async () =>
            {
                if (supervisor.Stopping.IsCancellationRequested)
                    throw new OperationCanceledException($"The connection is stopping and cannot be used: {_description}");

                IConnection connection = null;
                try
                {
                    TransportLogMessages.ConnectHost(_description);

                    if (_configuration.Settings.EndpointResolver != null)
                    {
                        connection = _connectionFactory.Value.CreateConnection(_configuration.Settings.EndpointResolver,
                            _configuration.Settings.ClientProvidedName);
                    }
                    else
                    {
                        var hostNames = new List<string>(1) {_configuration.Settings.Host};

                        connection = _connectionFactory.Value.CreateConnection(hostNames, _configuration.Settings.ClientProvidedName);
                    }

                    LogContext.Debug?.Log("Connected: {Host} (address: {RemoteAddress}, local: {LocalAddress})", _description, connection.Endpoint,
                        connection.LocalPort);

                    var connectionContext = new RabbitMqConnectionContext(connection, _configuration, _hostTopology, _description, supervisor.Stopped);

                    connectionContext.GetOrAddPayload(() => _configuration.Settings);

                    return (ConnectionContext)connectionContext;
                }
                catch (ConnectFailureException ex)
                {
                    connection?.Dispose();

                    throw new RabbitMqConnectionException("Connect failed: " + _description, ex);
                }
                catch (BrokerUnreachableException ex)
                {
                    connection?.Dispose();

                    throw new RabbitMqConnectionException("Broker unreachable: " + _description, ex);
                }
                catch (OperationInterruptedException ex)
                {
                    connection?.Dispose();

                    throw new RabbitMqConnectionException("Operation interrupted: " + _description, ex);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    connection?.Dispose();

                    throw new RabbitMqConnectionException("Create Connection Faulted: " + _description, ex);
                }
            }, supervisor.Stopping).ConfigureAwait(false);
        }
    }
}
