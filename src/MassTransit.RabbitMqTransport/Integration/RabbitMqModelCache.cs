// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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

        public async Task Close()
        {
            var scope = Interlocked.Exchange(ref _scope, null);

            scope?.Close();
        }

        async Task SendUsingNewModel(IPipe<ModelContext> modelPipe, ModelScope scope, CancellationToken cancellationToken)
        {
            IPipe<ConnectionContext> connectionPipe = Pipe.ExecuteAsync<ConnectionContext>(async connectionContext =>
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Creating model: {0}", connectionContext.HostSettings.ToDebugString());

                var model = await connectionContext.CreateModel().ConfigureAwait(false);

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
                    using (var context = await scope.Attach(cancellationToken).ConfigureAwait(false))
                    {
                        await modelPipe.Send(context).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    if (_log.IsDebugEnabled)
                        _log.Debug("The existing model usage threw an exception", ex);

                    Interlocked.CompareExchange(ref _scope, null, scope);

                    scope.Close();

                    throw;
                }
            });

            try
            {
                await _connectionCache.Send(connectionPipe, new CancellationToken()).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                if (_log.IsDebugEnabled)
                    _log.Debug("The connection threw an exception", exception);

                Interlocked.CompareExchange(ref _scope, null, scope);

                throw;
            }
        }

        async Task SendUsingExistingModel(IPipe<ModelContext> modelPipe, ModelScope existingScope, CancellationToken cancellationToken)
        {
            try
            {
                using (var context = await existingScope.Attach(cancellationToken).ConfigureAwait(false))
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Existing model: {0}", ((ModelContext)context).ConnectionContext.HostSettings.ToDebugString());

                    await modelPipe.Send(context).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                if (_log.IsDebugEnabled)
                    _log.Debug("The existing model usage threw an exception", ex);

                Interlocked.CompareExchange(ref _scope, null, existingScope);

                existingScope.Close();

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
                var id = Interlocked.Increment(ref _nextId);

                var context = new SharedModelContext(await _modelContext.Task.ConfigureAwait(false), id, Disconnect, cancellationToken);

                var added = _models.TryAdd(id, context);
                if (!added)
                    throw new InvalidOperationException("The connection could not be added");

                return context;
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
                    await Task.WhenAll(_models.Values.Select(x => x.Completed)).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _log.Error("Close faulted waiting for attached models", ex);
                }

                var modelContext = (await _modelContext.Task.ConfigureAwait(false));

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Disposing model: {0}", ((ModelContext)modelContext).ConnectionContext.HostSettings.ToDebugString());

                modelContext.Dispose();
            }
        }
    }
}