// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using Logging;
    using MassTransit.Pipeline;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Exceptions;


    public class RabbitMqConnectionCache :
        IConnectionCache,
        IProbeSite
    {
        static readonly ILog _log = Logger.Get<RabbitMqConnectionCache>();
        readonly ConnectionFactory _connectionFactory;
        readonly object _scopeLock = new object();

        readonly RabbitMqHostSettings _settings;
        ConnectionScope _scope;

        public RabbitMqConnectionCache(RabbitMqHostSettings settings)
        {
            _settings = settings;

            _connectionFactory = settings.GetConnectionFactory();
        }

        public Task Send(IPipe<ConnectionContext> connectionPipe, CancellationToken cancellationToken)
        {
            ConnectionScope newScope = null;
            ConnectionScope existingScope;

            lock (_scopeLock)
            {
                existingScope = _scope;
                if (existingScope == null)
                {
                    newScope = new ConnectionScope();
                    _scope = newScope;
                }
            }
            if (existingScope != null)
                return SendUsingExistingConnection(connectionPipe, cancellationToken, existingScope);

            return SendUsingNewConnection(connectionPipe, newScope, cancellationToken);
        }

        public void Probe(ProbeContext context)
        {
            ConnectionScope connectionScope = _scope;
            if (connectionScope != null)
            {
                context.Set(new
                {
                    Connected = true,
                });
            }
        }

        async Task SendUsingNewConnection(IPipe<ConnectionContext> connectionPipe, ConnectionScope scope, CancellationToken cancellationToken)
        {
            try
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Connecting: {0}", _connectionFactory.ToDebugString());
                
                IConnection connection = _connectionFactory.CreateConnection();

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Connected: {0}", _connectionFactory.ToDebugString());

                EventHandler<ShutdownEventArgs> connectionShutdown = null;
                connectionShutdown = (obj, reason) =>
                {
                    connection.ConnectionShutdown -= connectionShutdown;

                    Interlocked.CompareExchange(ref _scope, null, scope);

                    scope.Close();
                };

                connection.ConnectionShutdown += connectionShutdown;

                var connectionContext = new RabbitMqConnectionContext(connection, _settings, cancellationToken);

                connectionContext.GetOrAddPayload(() => _settings);

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Connected to {0} using local address {1}", connection.RemoteEndPoint, connection.LocalEndPoint);

                scope.Connected(connectionContext);
            }
            catch (BrokerUnreachableException ex)
            {
                Interlocked.CompareExchange(ref _scope, null, scope);

                scope.ConnectFaulted(ex);

                throw new RabbitMqConnectionException("Connect failed: " + _connectionFactory.ToDebugString(), ex);
            }

            try
            {
                using (SharedConnectionContext context = await scope.Attach(cancellationToken))
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Using new connection: {0}", ((ConnectionContext)context).HostSettings.ToDebugString());

                    await connectionPipe.Send(context);
                }
            }
            catch (Exception ex)
            {
                if (_log.IsDebugEnabled)
                    _log.Debug(string.Format("The connection usage threw an exception"), ex);

                throw;
            }
        }

        static async Task SendUsingExistingConnection(IPipe<ConnectionContext> connectionPipe, CancellationToken cancellationToken, ConnectionScope scope)
        {
            try
            {
                using (SharedConnectionContext context = await scope.Attach(cancellationToken))
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Using existing connection: {0}", ((ConnectionContext)context).HostSettings.ToDebugString());

                    await connectionPipe.Send(context);
                }
            }
            catch (Exception ex)
            {
                if (_log.IsDebugEnabled)
                    _log.Debug(string.Format("The existing connection usage threw an exception"), ex);

                throw;
            }
        }


        class ConnectionScope
        {
            readonly ConcurrentBag<SharedConnectionContext> _attached;
            readonly TaskCompletionSource<RabbitMqConnectionContext> _connectionContext;

            public ConnectionScope()
            {
                _attached = new ConcurrentBag<SharedConnectionContext>();
                _connectionContext = new TaskCompletionSource<RabbitMqConnectionContext>();
            }

            public void Connected(RabbitMqConnectionContext connectionContext)
            {
                _connectionContext.TrySetResult(connectionContext);
            }

            public async Task<SharedConnectionContext> Attach(CancellationToken cancellationToken)
            {
                var context = new SharedConnectionContext(await _connectionContext.Task, cancellationToken);

                _attached.Add(context);
                return context;
            }

            public async void Close()
            {
                try
                {
                    await Task.WhenAll(_attached.Select(x => x.Completed));
                }
                catch (Exception ex)
                {
                    _log.Error("Close faulted waiting for attached connections", ex);
                }

                (await _connectionContext.Task).Dispose();
            }

            public void ConnectFaulted(Exception exception)
            {
                _connectionContext.TrySetException(exception);
            }
        }
    }
}