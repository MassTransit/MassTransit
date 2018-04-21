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
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using Logging;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Exceptions;
    using Topology;


    public class ConnectionContextFactory :
        IPipeContextFactory<ConnectionContext>
    {
        static readonly ILog _log = Logger.Get<ConnectionContextFactory>();
        readonly Lazy<ConnectionFactory> _connectionFactory;
        readonly string _description;
        readonly RabbitMqHostSettings _settings;
        readonly IRabbitMqHostTopology _topology;

        public ConnectionContextFactory(RabbitMqHostSettings settings, IRabbitMqHostTopology topology)
        {
            _settings = settings;
            _topology = topology;

            _description = settings.ToDebugString();

            _connectionFactory = new Lazy<ConnectionFactory>(settings.GetConnectionFactory);
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

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Connecting: {0}", _description);

                if (_settings.ClusterMembers?.Any() ?? false)
                {
                    connection = _connectionFactory.Value.CreateConnection(_settings.ClusterMembers, _settings.ClientProvidedName);
                }
                else
                {
                    List<string> hostNames = Enumerable.Repeat(_settings.Host, 1).ToList();

                    connection = _connectionFactory.Value.CreateConnection(hostNames, _settings.ClientProvidedName);
                }

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Connected: {0} (address: {1}, local: {2})", _description, connection.Endpoint, connection.LocalPort);

                var connectionContext = new RabbitMqConnectionContext(connection, _settings, _topology, _description, supervisor.Stopped);
                connectionContext.GetOrAddPayload(() => _settings);

                return connectionContext;
            }
            catch (ConnectFailureException ex)
            {
                if (_log.IsDebugEnabled)
                    _log.Debug("RabbitMQ Connect failed:", ex);

                connection?.Dispose();

                throw new RabbitMqConnectionException("Connect failed: " + _description, ex);
            }
            catch (BrokerUnreachableException ex)
            {
                if (_log.IsDebugEnabled)
                    _log.Debug("RabbitMQ was unreachable", ex);

                connection?.Dispose();

                throw new RabbitMqConnectionException("Broker unreachable: " + _description, ex);
            }
            catch (OperationInterruptedException ex)
            {
                if (_log.IsDebugEnabled)
                    _log.Debug("RabbitMQ operation interrupted", ex);

                connection?.Dispose();

                throw new RabbitMqConnectionException("Operation interrupted: " + _description, ex);
            }
        }
    }
}