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
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using Logging;
    using MassTransit.Pipeline;
    using RabbitMQ.Client;
    using Util;


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
        readonly ITaskScope _taskScope;
        ModelScope _scope;

        public RabbitMqModelCache(IConnectionCache connectionCache, ITaskSupervisor supervisor)
        {
            _connectionCache = connectionCache;

            _taskScope = supervisor.CreateScope($"{TypeMetadataCache<RabbitMqSendTransportProvider>.ShortName} - Model Cache", CloseScope);
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
                    newScope = new ModelScope(_taskScope);
                    _scope = newScope;
                }
            }
            if (existingScope != null)
                return SendUsingExistingModel(connectionPipe, existingScope, cancellationToken);

            return SendUsingNewModel(connectionPipe, newScope, cancellationToken);
        }

        public Task Close()
        {
            return _taskScope.Stop("Closed by owner");
        }

        async Task CloseScope()
        {
            ModelScope existingScope;
            lock (_scopeLock)
            {
                existingScope = _scope;
            }
            if (existingScope != null)
                await existingScope.Close().ConfigureAwait(false);
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

                var modelContext = new RabbitMqModelContext(connectionContext, model, _taskScope);

                scope.Created(modelContext);

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
                await _connectionCache.Send(connectionPipe, _taskScope.StoppedToken).ConfigureAwait(false);
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
            readonly ITaskScope _taskScope;
            bool _closed;

            public ModelScope(ITaskScope supervisor)
            {
                _modelContext = new TaskCompletionSource<RabbitMqModelContext>();

                _taskScope = supervisor.CreateScope("ModelScope", Close);
            }

            /// <summary>
            /// Connect a connectable type
            /// </summary>
            /// <param name="cancellationToken"></param>
            /// <returns>The connection handle</returns>
            public async Task<SharedModelContext> Attach(CancellationToken cancellationToken)
            {
                var modelContext = await _modelContext.Task.ConfigureAwait(false);

                var participant =
                    _taskScope.CreateParticipant(
                        $"{TypeMetadataCache<ModelScope>.ShortName} - {((ModelContext)modelContext).ConnectionContext.HostSettings.ToDebugString()}");

                return new SharedModelContext(modelContext, cancellationToken, participant);
            }

            public void Created(RabbitMqModelContext connectionContext)
            {
                _modelContext.TrySetResult(connectionContext);

                _taskScope.SetReady();
            }

            public async Task Close()
            {
                if (_closed)
                    return;

                var modelContext = await _modelContext.Task.ConfigureAwait(false);

                try
                {
                    using (var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60)))
                    {
                        await _taskScope.Stop("Closing model", tokenSource.Token);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("Close Faulted waiting for shared contexts", ex);
                }

                _closed = true;

                try
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Disposing model: {0}", ((ModelContext)modelContext).ConnectionContext.HostSettings.ToDebugString());

                    modelContext.Dispose();
                }
                catch (Exception exception)
                {
                    _log.Error("Failed to dispose of the model context", exception);
                }

                _taskScope.SetComplete();
            }
        }
    }
}