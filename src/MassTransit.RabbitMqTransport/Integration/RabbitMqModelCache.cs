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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using Logging;
    using MassTransit.Pipeline;
    using RabbitMQ.Client;


    /// <summary>
    /// Caches the models for sending that have already been created, so that the model 
    /// is retained and configured using an existing connection
    /// </summary>
    public class RabbitMqModelCache :
        IModelCache
    {
        static readonly ILog _log = Logger.Get<RabbitMqModelCache>();

        readonly IConnectionCache _connectionCache;
        readonly object _scopeLock = new object();
        ModelScope _scope;

        public RabbitMqModelCache(IConnectionCache connectionCache)
        {
            _connectionCache = connectionCache;
        }

        public Task Send(IPipe<ModelContext> connectionPipe, CancellationToken cancellationToken)
        {
            ModelScope newScope = null;
            ModelScope existingScope;

            lock (_scopeLock)
            {
                existingScope = _scope;
                if (existingScope == null)
                {
                    newScope = new ModelScope();
                    _scope = newScope;
                }
            }
            if (existingScope != null)
                return SendUsingExistingModel(connectionPipe, existingScope, cancellationToken);

            return SendUsingNewModel(connectionPipe, newScope, cancellationToken);
        }

        async Task SendUsingNewModel(IPipe<ModelContext> modelPipe, ModelScope scope, CancellationToken cancellationToken)
        {
            IPipe<ConnectionContext> connectionPipe = Pipe.ExecuteAsync<ConnectionContext>(async connectionContext =>
            {
                IModel model = await connectionContext.CreateModel();

                EventHandler<ShutdownEventArgs> modelShutdown = null;
                modelShutdown = (obj, reason) =>
                {
                    model.ModelShutdown -= modelShutdown;

                    Interlocked.CompareExchange(ref _scope, null, scope);

                    scope.Close();
                };

                model.ModelShutdown += modelShutdown;

                var modelContext = new RabbitMqModelContext(connectionContext, model, connectionContext.CancellationToken);

                scope.Connected(modelContext);

                try
                {
                    using (SharedModelContext context = await scope.Attach(cancellationToken))
                    {
                        await modelPipe.Send(context);
                    }
                }
                catch (Exception ex)
                {
                    if (_log.IsDebugEnabled)
                        _log.Debug("The existing model usage threw an exception", ex);

                    throw;
                }
            });

            await _connectionCache.Send(connectionPipe, new CancellationToken());
        }

        static async Task SendUsingExistingModel(IPipe<ModelContext> modelPipe, ModelScope existingScope, CancellationToken cancellationToken)
        {
            try
            {
                using (SharedModelContext context = await existingScope.Attach(cancellationToken))
                {
                    await modelPipe.Send(context);
                }
            }
            catch (Exception ex)
            {
                if (_log.IsDebugEnabled)
                    _log.Debug("The existing model usage threw an exception", ex);

                throw;
            }
        }


        class ModelScope
        {
            readonly TaskCompletionSource<RabbitMqModelContext> _modelContext;
            readonly ConcurrentDictionary<long, SharedModelContext> _models;
            long _nextId;

            public ModelScope()
            {
                _models = new ConcurrentDictionary<long, SharedModelContext>();
                _modelContext = new TaskCompletionSource<RabbitMqModelContext>();
            }

            /// <summary>
            /// Connect a connectable type
            /// </summary>
            /// <param name="cancellationToken"></param>
            /// <returns>The connection handle</returns>
            public async Task<SharedModelContext> Attach(CancellationToken cancellationToken)
            {
                long id = Interlocked.Increment(ref _nextId);

                var context = new SharedModelContext(await _modelContext.Task, id, Disconnect, cancellationToken);

                bool added = _models.TryAdd(id, context);
                if (!added)
                    throw new InvalidOperationException("The connection could not be added");

                return context;
            }

            /// <summary>
            /// Enumerate the connections invoking the callback for each connection
            /// </summary>
            /// <param name="callback">The callback</param>
            /// <returns>An awaitable Task for the operation</returns>
            public async Task ForEach(Func<SharedModelContext, Task> callback)
            {
                if (callback == null)
                    throw new ArgumentNullException(nameof(callback));

                if (_models.Count == 0)
                    return;

                var exceptions = new List<Exception>();

                foreach (SharedModelContext connection in _models.Values)
                {
                    try
                    {
                        await callback(connection);
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }

                if (exceptions.Count > 0)
                    throw new AggregateException(exceptions);
            }

            public bool All(Func<SharedModelContext, bool> callback)
            {
                return _models.Values.All(callback);
            }

            void Disconnect(long id)
            {
                SharedModelContext ignored;
                _models.TryRemove(id, out ignored);
            }

            public void Connected(RabbitMqModelContext connectionContext)
            {
                _modelContext.TrySetResult(connectionContext);
            }

            public async void Close()
            {
                try
                {
                    await Task.WhenAll(_models.Values.Select(x => x.Completed));
                }
                catch (Exception ex)
                {
                    _log.Error("Close faulted waiting for attached models", ex);
                }

                (await _modelContext.Task).Dispose();
            }
        }
    }
}