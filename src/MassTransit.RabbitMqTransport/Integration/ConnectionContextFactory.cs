namespace MassTransit.RabbitMqTransport.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Exceptions;
    using Topology;


    public class ConnectionContextFactory :
        IPipeContextFactory<ConnectionContext>
    {
        readonly IRabbitMqHostConfiguration _configuration;
        readonly IRabbitMqHostTopology _hostTopology;
        readonly Lazy<ConnectionFactory> _connectionFactory;
        readonly string _description;

        public ConnectionContextFactory(IRabbitMqHostConfiguration configuration, IRabbitMqHostTopology hostTopology)
        {
            _configuration = configuration;
            _hostTopology = hostTopology;

            _description = configuration.Settings.ToDescription();

            _connectionFactory = new Lazy<ConnectionFactory>(_configuration.Settings.GetConnectionFactory);
        }

        IPipeContextAgent<ConnectionContext> IPipeContextFactory<ConnectionContext>.CreateContext(ISupervisor supervisor)
        {
            var context = Task.Factory.StartNew(() => CreateConnection(supervisor), supervisor.Stopping, TaskCreationOptions.None, TaskScheduler.Default)
                .Unwrap();

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

        async Task<ConnectionContext> CreateSharedConnection(Task<ConnectionContext> context, CancellationToken cancellationToken)
        {
            var connectionContext = await context.ConfigureAwait(false);

            var sharedConnection = new SharedConnectionContext(connectionContext, cancellationToken);

            return sharedConnection;
        }

        async Task<ConnectionContext> CreateConnection(ISupervisor supervisor)
        {
            IConnection connection = null;
            try
            {
                if (supervisor.Stopping.IsCancellationRequested)
                    throw new OperationCanceledException($"The connection is stopping and cannot be used: {_description}");

                LogContext.Debug?.Log("Connecting: {Host}", _description);

                if (_configuration.Settings.ClusterMembers?.Any() ?? false)
                {
                    connection = _connectionFactory.Value.CreateConnection(_configuration.Settings.ClusterMembers, _configuration.Settings.ClientProvidedName);
                }
                else
                {
                    List<string> hostNames = Enumerable.Repeat(_configuration.Settings.Host, 1).ToList();

                    connection = _connectionFactory.Value.CreateConnection(hostNames, _configuration.Settings.ClientProvidedName);
                }

                LogContext.Debug?.Log("Connected: {Host} (address: {RemoteAddress}, local: {LocalAddress})", _description, connection.Endpoint,
                    connection.LocalPort);

                var connectionContext = new RabbitMqConnectionContext(connection, _configuration, _hostTopology, _description, supervisor.Stopped);

                connectionContext.GetOrAddPayload(() => _configuration.Settings);

                return connectionContext;
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
        }
    }
}
