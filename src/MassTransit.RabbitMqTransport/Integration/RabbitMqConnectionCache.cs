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
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using Logging;
    using MassTransit.Pipeline;
    using Pipeline;
    using RabbitMQ.Client;
    using Util;


    public class RabbitMqConnectionCache :
        IConnectionCache
    {
        static readonly ILog _log = Logger.Get<RabbitMqConnectionCache>();

        readonly IRabbitMqConnector _connector;
        readonly CancellationTokenSource _stop;
        ConnectionScope _scope;
        QueuedTaskScheduler _taskScheduler;

        public RabbitMqConnectionCache(IRabbitMqConnector connector)
        {
            _connector = connector;
            _stop = new CancellationTokenSource();
            _taskScheduler = new QueuedTaskScheduler(TaskScheduler.Default, 1);

        }

        public Task Send(IPipe<ConnectionContext> connectionPipe, CancellationToken cancellationToken)
        {
            ConnectionScope existingScope = _scope;
            if (existingScope != null)
            {
                if (existingScope.ConnectionClosed.Task.Wait(TimeSpan.Zero) == false)
                    return SendUsingExistingConnection(connectionPipe, cancellationToken, existingScope);
            }

            return SendUsingNewConnection(connectionPipe, cancellationToken);
        }

        public async Task Stop()
        {
            _stop.Cancel();

            Interlocked.MemoryBarrier();

            ConnectionScope existingScope = _scope;
            if (existingScope != null)
                await existingScope.ConnectionClosed.Task.ConfigureAwait(false);
        }

        async Task SendUsingNewConnection(IPipe<ConnectionContext> connectionPipe,
            CancellationToken cancellationToken)
        {
            var scope = new ConnectionScope();

            IPipe<ConnectionContext> connectPipe = Pipe.ExecuteAsync<ConnectionContext>(async connectionContext =>
            {
                IConnection connection = connectionContext.Connection;

                EventHandler<ShutdownEventArgs> connectionShutdown = null;
                connectionShutdown = (obj, reason) =>
                {
                    connection.ConnectionShutdown -= connectionShutdown;

                    Interlocked.CompareExchange(ref _scope, null, scope);

                    scope.ConnectionClosed.TrySetResult(true);
                };

                CancellationTokenRegistration registration = _stop.Token.Register(() =>
                {
                    scope.ConnectionClosed.TrySetResult(false);
                });

                connectionContext.Connection.ConnectionShutdown += connectionShutdown;

                Interlocked.CompareExchange(ref _scope, scope, null);

                await scope.Connected(connectionContext).ConfigureAwait(false);
            });

            _connector.Connect(connectPipe, _stop.Token).ConfigureAwait(false);

            try
            {
                ConnectionContext connectionContext = await scope.ConnectionContext.ConfigureAwait(false);

                var context = new SharedConnectionContext(connectionContext, cancellationToken);

                await connectionPipe.Send(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (_log.IsDebugEnabled)
                    _log.Debug(string.Format("The existing connection usage threw an exception"), ex);

                throw;
            }
        }

        static async Task SendUsingExistingConnection(IPipe<ConnectionContext> connectionPipe,
            CancellationToken cancellationToken, ConnectionScope existingScope)
        {
            try
            {
                ConnectionContext connectionContext = await existingScope.ConnectionContext.ConfigureAwait(false);

                var context = new SharedConnectionContext(connectionContext, cancellationToken);

                await connectionPipe.Send(context).ConfigureAwait(false);
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
            public readonly TaskCompletionSource<bool> ConnectionClosed;
            public readonly Task<ConnectionContext> ConnectionContext;

            readonly TaskCompletionSource<ConnectionContext> _contextSource;

            public ConnectionScope()
            {
                _contextSource = new TaskCompletionSource<ConnectionContext>();
                ConnectionContext = _contextSource.Task;

                ConnectionClosed = new TaskCompletionSource<bool>();
            }

            public async Task Connected(ConnectionContext context)
            {
                _contextSource.TrySetResult(context);

                await ConnectionClosed.Task.ConfigureAwait(false);
            }
        }
    }
}