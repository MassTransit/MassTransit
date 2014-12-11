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
    using Contexts;
    using Logging;
    using MassTransit.Pipeline;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;


    /// <summary>
    /// Caches the models for sending that have already been created, so that the model 
    /// is retained and configured using an existing connection
    /// </summary>
    public class RabbitMqModelCache :
        IModelCache
    {
        static readonly ILog _log = Logger.Get<RabbitMqConnectionCache>();
        readonly IConnectionCache _connectionCache;
        volatile ModelScope _scope;

        public RabbitMqModelCache(IConnectionCache connectionCache)
        {
            _connectionCache = connectionCache;
        }

        public Task Send<T>(T message, IPipe<TupleContext<ModelContext, T>> connectionPipe, CancellationToken cancellationToken)
            where T : class
        {
            Interlocked.MemoryBarrier();

            ModelScope existingScope = _scope;
            if (existingScope != null)
            {
                if (existingScope.ModelClosed.Task.Wait(TimeSpan.Zero))
                    return SendUsingExistingConnection(message, connectionPipe, cancellationToken, existingScope);
            }

            return SendUsingNewConnection(message, connectionPipe, cancellationToken);
        }

        Task SendUsingNewConnection<T>(T message, IPipe<TupleContext<ModelContext, T>> modelPipe,
            CancellationToken cancellationToken)
            where T : class
        {
            IPipe<TupleContext<ConnectionContext, T>> connectionPipe = Pipe.New<TupleContext<ConnectionContext, T>>(x =>
            {
                x.ExecuteAsync(async connectionContext =>
                {
                    IModel model = connectionContext.Context.Connection.CreateModel();
                    var modelContext = new RabbitMqModelContext(connectionContext.Context, model, connectionContext.CancellationToken);

                    var scope = new ModelScope(modelContext);

                    ModelShutdownEventHandler modelShutdown = null;
                    modelShutdown = (connection, reason) =>
                    {
                        scope.ModelContext.Model.ModelShutdown -= modelShutdown;
                        scope.ModelClosed.TrySetResult(true);

                        Interlocked.CompareExchange(ref _scope, null, scope);
                    };

                    scope.ModelContext.Model.ModelShutdown += modelShutdown;

                    Interlocked.CompareExchange(ref _scope, scope, null);

                    try
                    {
                        var context = new TupleContextProxy<ModelContext, T>(scope.ModelContext, message, cancellationToken);

                        await modelPipe.Send(context);
                    }
                    catch (Exception ex)
                    {
                        if (_log.IsDebugEnabled)
                            _log.Debug(string.Format("The existing connection usage threw an exception"), ex);

                        throw;
                    }
                });
            });

            return _connectionCache.Send(message, connectionPipe, new CancellationToken());
        }

        static async Task SendUsingExistingConnection<T>(T message, IPipe<TupleContext<ModelContext, T>> connectionPipe,
            CancellationToken cancellationToken, ModelScope existingScope)
            where T : class
        {
            try
            {
                var context = new TupleContextProxy<ModelContext, T>(existingScope.ModelContext, message, cancellationToken);

                await connectionPipe.Send(context);
            }
            catch (Exception ex)
            {
                if (_log.IsDebugEnabled)
                    _log.Debug(string.Format("The existing model usage threw an exception"), ex);

                throw;
            }
        }


        class ModelScope
        {
            public readonly TaskCompletionSource<bool> ModelClosed;
            public readonly ModelContext ModelContext;

            public ModelScope(ModelContext modelContext)
            {
                ModelContext = modelContext;
                ModelClosed = new TaskCompletionSource<bool>();
            }
        }
    }
}