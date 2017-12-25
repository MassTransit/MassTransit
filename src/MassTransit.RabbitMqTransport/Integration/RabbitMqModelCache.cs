// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using GreenPipes;
    using Logging;
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
        readonly ITaskScope _cacheTaskScope;

        readonly IRabbitMqHost _host;
        readonly object _scopeLock = new object();
        ModelScope _scope;

        public RabbitMqModelCache(IRabbitMqHost host)
        {
            _host = host;

            _cacheTaskScope = host.Supervisor.CreateScope($"{TypeMetadataCache<RabbitMqModelCache>.ShortName}", CloseScope);
        }

        public Task Send(IPipe<ModelContext> connectionPipe, CancellationToken cancellationToken)
        {
            ModelScope newScope = null;
            ModelScope existingScope;

            lock (_scopeLock)
            {
                existingScope = _scope;
                if (existingScope == null || existingScope.IsShuttingDown)
                {
                    newScope = new ModelScope(_cacheTaskScope);
                    _scope = newScope;
                }
            }
            if (existingScope != null)
                return SendUsingExistingModel(connectionPipe, existingScope, cancellationToken);

            return SendUsingNewModel(connectionPipe, newScope, cancellationToken);
        }

        public async Task Close()
        {
            Interlocked.Exchange(ref _scope, null);

            _cacheTaskScope.Stop(new StopEventArgs("Closed by owner"));
        }

        Task CloseScope()
        {
            return TaskUtil.Completed;
        }

        async Task SendUsingNewModel(IPipe<ModelContext> modelPipe, ModelScope scope, CancellationToken cancellationToken)
        {
            IPipe<ConnectionContext> connectionPipe = Pipe.ExecuteAsync<ConnectionContext>(async connectionContext =>
            {
                IModel model = null;
                try
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Creating model: {0}", connectionContext.HostSettings.ToDebugString());

                    model = await connectionContext.CreateModel().ConfigureAwait(false);

                    EventHandler<ShutdownEventArgs> modelShutdown = null;
                    modelShutdown = (obj, reason) =>
                    {
                        model.ModelShutdown -= modelShutdown;

                        Interlocked.CompareExchange(ref _scope, null, scope);

                        scope.Shutdown(reason.ReplyText);
                    };

                    model.ModelShutdown += modelShutdown;

                    var modelContext = new RabbitMqModelContext(connectionContext, model, _cacheTaskScope, _host);

                    scope.Created(modelContext);
                }
                catch (Exception ex)
                {
                    Interlocked.CompareExchange(ref _scope, null, scope);

                    scope.CreateFaulted(ex);

                    model?.Dispose();

                    throw;
                }

                await SendUsingExistingModel(modelPipe, scope, cancellationToken).ConfigureAwait(false);
            });

            try
            {
                await _host.ConnectionCache.Send(connectionPipe, _cacheTaskScope.StoppedToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                if (_log.IsDebugEnabled)
                    _log.Debug("The connection threw an exception", exception);

                Interlocked.CompareExchange(ref _scope, null, scope);

                scope.CreateFaulted(exception);

                throw;
            }
        }

        async Task SendUsingExistingModel(IPipe<ModelContext> modelPipe, ModelScope scope, CancellationToken cancellationToken)
        {
            try
            {
                using (var context = await scope.Attach(cancellationToken).ConfigureAwait(false))
                {
//                    if (_log.IsDebugEnabled)
//                        _log.DebugFormat("Using model: {0}", ((ModelContext)context).ConnectionContext.HostSettings.ToDebugString());

                    await modelPipe.Send(context).ConfigureAwait(false);
                }
            }
            catch (Exception exception)
            {
                if (_log.IsDebugEnabled)
                    _log.Debug("The model usage threw an exception", exception);

                Interlocked.CompareExchange(ref _scope, null, scope);

                scope.CreateFaulted(exception);

                throw;
            }
        }


        class ModelScope
        {
            readonly TaskCompletionSource<RabbitMqModelContext> _modelContext;
            readonly ITaskScope _taskScope;

            public ModelScope(ITaskScope supervisor)
            {
                _modelContext = new TaskCompletionSource<RabbitMqModelContext>();

                _taskScope = supervisor.CreateScope("ModelScope", CloseContext);
            }

            public bool IsShuttingDown => _taskScope.StoppingToken.IsCancellationRequested;

            public void Created(RabbitMqModelContext connectionContext)
            {
                _modelContext.TrySetResult(connectionContext);

                _taskScope.SetReady();
            }

            public void CreateFaulted(Exception exception)
            {
                _modelContext.TrySetException(exception);

                _taskScope.SetNotReady(exception);

                _taskScope.Stop(new StopEventArgs($"Model faulted: {exception.Message}"));
            }

            public async Task<SharedModelContext> Attach(CancellationToken cancellationToken)
            {
                var modelContext = await _modelContext.Task.ConfigureAwait(false);

                return new SharedModelContext(modelContext, cancellationToken, _taskScope);
            }

            public void Shutdown(string reason)
            {
                _taskScope.Stop(new StopEventArgs(reason));
            }

            async Task CloseContext()
            {
                if (_modelContext.Task.Status == TaskStatus.RanToCompletion)
                {
                    try
                    {
                        var modelContext = await _modelContext.Task.ConfigureAwait(false);

                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("Disposing model: {0}", ((ModelContext)modelContext).ConnectionContext.HostSettings.ToDebugString());

                        modelContext.Dispose();
                    }
                    catch (Exception exception)
                    {
                        if (_log.IsWarnEnabled)
                            _log.Warn("The model failed to be disposed", exception);
                    }
                }
            }
        }
    }
}