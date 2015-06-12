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
                    using (var context = await scope.Attach(cancellationToken))
                    {
                        await modelPipe.Send(context);
                    }

                }
                catch (Exception ex)
                {
                    if (_log.IsDebugEnabled)
                        _log.Debug(string.Format("The existing model usage threw an exception"), ex);

                    throw;
                }
            });

            await _connectionCache.Send(connectionPipe, new CancellationToken());
        }

        static async Task SendUsingExistingModel(IPipe<ModelContext> connectionPipe, ModelScope existingScope, CancellationToken cancellationToken)
        {
            try
            {
                using(var context = await existingScope.Attach(cancellationToken))

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
            readonly ConcurrentBag<SharedModelContext> _attached;
            readonly TaskCompletionSource<RabbitMqModelContext> _connectionContext;

            public ModelScope()
            {
                _attached = new ConcurrentBag<SharedModelContext>();
                _connectionContext = new TaskCompletionSource<RabbitMqModelContext>();
            }

            public void Connected(RabbitMqModelContext connectionContext)
            {
                _connectionContext.TrySetResult(connectionContext);
            }

            public async Task<SharedModelContext> Attach(CancellationToken cancellationToken)
            {
                var context = new SharedModelContext(await _connectionContext.Task, cancellationToken);

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
                }

                (await _connectionContext.Task).Dispose();
            }
        }
    }
}