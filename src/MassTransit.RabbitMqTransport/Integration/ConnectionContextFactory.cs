// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.
namespace MassTransit.RabbitMqTransport.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using Logging;
    using Microsoft.Extensions.Logging;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Exceptions;


    public class ConnectionContextFactory :
        IPipeContextFactory<ConnectionContext>
    {
        static readonly ILogger _logger = Logger.Get<ConnectionContextFactory>();
        readonly IRabbitMqHostConfiguration _configuration;
        readonly Lazy<ConnectionFactory> _connectionFactory;
        readonly string _description;

        public ConnectionContextFactory(IRabbitMqHostConfiguration configuration)
        {
            _configuration = configuration;

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

                _logger.LogDebug("Connecting: {0}", _description);

                if (_configuration.Settings.ClusterMembers?.Any() ?? false)
                {
                    connection = _connectionFactory.Value.CreateConnection(_configuration.Settings.ClusterMembers, _configuration.Settings.ClientProvidedName);
                }
                else
                {
                    List<string> hostNames = Enumerable.Repeat(_configuration.Settings.Host, 1).ToList();

                    connection = _connectionFactory.Value.CreateConnection(hostNames, _configuration.Settings.ClientProvidedName);
                }

                _logger.LogDebug("Connected: {0} (address: {1}, local: {2})", _description, connection.Endpoint, connection.LocalPort);

                var connectionContext = new RabbitMqConnectionContext(connection, _configuration, _description, supervisor.Stopped);

                connectionContext.GetOrAddPayload(() => _configuration.Settings);

                return connectionContext;
            }
            catch (ConnectFailureException ex)
            {
                _logger.LogDebug("RabbitMQ Connect failed:", ex);

                connection?.Dispose();

                throw new RabbitMqConnectionException("Connect failed: " + _description, ex);
            }
            catch (BrokerUnreachableException ex)
            {
                _logger.LogDebug("RabbitMQ was unreachable", ex);

                connection?.Dispose();

                throw new RabbitMqConnectionException("Broker unreachable: " + _description, ex);
            }
            catch (OperationInterruptedException ex)
            {
                _logger.LogDebug("RabbitMQ operation interrupted", ex);

                connection?.Dispose();

                throw new RabbitMqConnectionException("Operation interrupted: " + _description, ex);
            }
        }
    }
}
