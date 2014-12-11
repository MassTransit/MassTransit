// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using MassTransit.Pipeline;
    using Pipeline;
    using RabbitMQ.Client.Events;


    public class RabbitMqConnectionCache :
        IConnectionCache
    {
        static readonly ILog _log = Logger.Get<RabbitMqConnectionCache>();
        readonly IRabbitMqConnector _connector;
        volatile ConnectionScope _scope;

        public RabbitMqConnectionCache(IRabbitMqConnector connector)
        {
            _connector = connector;
        }

        public Task Send<T>(T message, IPipe<TupleContext<ConnectionContext, T>> connectionPipe, CancellationToken cancellationToken)
            where T : class
        {
            Interlocked.MemoryBarrier();

            ConnectionScope existingScope = _scope;
            if (existingScope != null)
            {
                if (existingScope.ConnectionClosed.Task.Wait(TimeSpan.Zero))
                    return SendUsingExistingConnection(message, connectionPipe, cancellationToken, existingScope);
            }

            return SendUsingNewConnection(message, connectionPipe, cancellationToken);
        }

        Task SendUsingNewConnection<T>(T message, IPipe<TupleContext<ConnectionContext, T>> connectionPipe,
            CancellationToken cancellationToken)
            where T : class
        {
            IPipe<ConnectionContext> connectPipe = Pipe.New<ConnectionContext>(x =>
            {
                x.ExecuteAsync(async connectionContext =>
                {
                    var scope = new ConnectionScope(connectionContext);

                    ConnectionShutdownEventHandler connectionShutdown = null;
                    connectionShutdown = (connection, reason) =>
                    {
                        scope.ConnectionContext.Connection.ConnectionShutdown -= connectionShutdown;
                        scope.ConnectionClosed.TrySetResult(true);

                        Interlocked.CompareExchange(ref _scope, null, scope);
                    };

                    connectionContext.Connection.ConnectionShutdown += connectionShutdown;

                    Interlocked.CompareExchange(ref _scope, scope, null);

                    try
                    {
                        var context = new TupleContextProxy<ConnectionContext, T>(scope.ConnectionContext, message, cancellationToken);

                        await connectionPipe.Send(context);
                    }
                    catch (Exception ex)
                    {
                        if (_log.IsDebugEnabled)
                            _log.Debug(string.Format("The existing connection usage threw an exception"), ex);

                        throw;
                    }
                });
            });

            return _connector.Connect(connectPipe, new CancellationToken());
        }

        static async Task SendUsingExistingConnection<T>(T message, IPipe<TupleContext<ConnectionContext, T>> connectionPipe,
            CancellationToken cancellationToken, ConnectionScope existingScope)
            where T : class
        {
            try
            {
                var context = new TupleContextProxy<ConnectionContext, T>(existingScope.ConnectionContext, message, cancellationToken);

                await connectionPipe.Send(context);
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
            public readonly ConnectionContext ConnectionContext;

            public ConnectionScope(ConnectionContext connectionContext)
            {
                ConnectionContext = connectionContext;
                ConnectionClosed = new TaskCompletionSource<bool>();
            }
        }
    }
}