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
namespace MassTransit.HttpTransport.Hosting
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration.Builders;
    using Logging;
    using MassTransit.Pipeline;
    using Util;


    public class OwinHostCache :
        IOwinHostCache,
        IProbeSite
    {
        static readonly ILog _log = Logger.Get<OwinHostCache>();
        readonly ITaskScope _cacheTaskScope;
        readonly OwinHostInstanceFactory _owinHostInstanceFactory;
        readonly object _scopeLock = new object();
        readonly HttpHostSettings _settings;
        OwinHostScope _scope;

        public OwinHostCache(HttpHostSettings settings, ITaskSupervisor supervisor)
        {
            _settings = settings;
            _owinHostInstanceFactory = settings.GetHostFactory();

            _cacheTaskScope = supervisor.CreateScope($"{TypeMetadataCache<OwinHostCache>.ShortName} - {settings.ToDebugString()}", CloseScope);
        }

        public Task Send(IPipe<OwinHostContext> connectionPipe, CancellationToken stoppingToken)
        {
            OwinHostScope newScope = null;
            OwinHostScope existingScope;

            lock (_scopeLock)
            {
                existingScope = _scope;
                if (existingScope == null)
                {
                    newScope = new OwinHostScope(_cacheTaskScope, _settings);
                    _scope = newScope;
                }
            }
            if (existingScope != null)
                return SendUsingExistingConnection(connectionPipe, existingScope, stoppingToken);

            return SendUsingNewConnection(connectionPipe, newScope, stoppingToken);
        }

        public void Probe(ProbeContext context)
        {
            var connectionScope = _scope;
            if (connectionScope != null)
            {
                context.Set(new
                {
                    Connected = true
                });
            }
        }

        Task SendUsingNewConnection(IPipe<OwinHostContext> connectionPipe, OwinHostScope scope, CancellationToken stoppingToken)
        {
            try
            {
                if (_cacheTaskScope.StoppingToken.IsCancellationRequested)
                    throw new TaskCanceledException($"The connection is being disconnected: {_settings.ToDebugString()}");

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Connecting: {0}", _owinHostInstanceFactory.ToDebugString());

                var owinHost = _owinHostInstanceFactory.CreateHost();

                if (_log.IsDebugEnabled)
                {
                    _log.DebugFormat("Connected: {0} (address: {1}, local: {2}", _owinHostInstanceFactory.ToDebugString(),
                        owinHost.Host, owinHost.Port);
                }

                EventHandler<ShutdownEventArgs> hostShutdown = null;
                hostShutdown = (obj, reason) =>
                {
                    Interlocked.CompareExchange(ref _scope, null, scope);

                    scope.Shutdown(reason.ReplyText);
                };

                owinHost.HostShutdown += hostShutdown;

                var hostContext = new HttpOwinHostContext(owinHost, _settings, _cacheTaskScope);

                hostContext.GetOrAddPayload(() => _settings);

                scope.Connected(hostContext);
            }
            catch (Exception ex)
            {
                Interlocked.CompareExchange(ref _scope, null, scope);

                scope.ConnectFaulted(ex);

                throw new HttpConnectionException("Connect failed: " + _owinHostInstanceFactory.ToDebugString(), ex);
            }

            return SendUsingExistingConnection(connectionPipe, scope, stoppingToken);
        }

        async Task SendUsingExistingConnection(IPipe<OwinHostContext> connectionPipe, OwinHostScope scope, CancellationToken cancellationToken)
        {
            try
            {
                using (var context = await scope.Attach(cancellationToken).ConfigureAwait(false))
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Using host: {0}", context.HostSettings.ToDebugString());

                    await connectionPipe.Send(context).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                if (_log.IsDebugEnabled)
                    _log.Debug("The host usage threw an exception", ex);

                Interlocked.CompareExchange(ref _scope, null, scope);

                scope.ConnectFaulted(ex);

                throw;
            }
        }

        Task CloseScope()
        {
            return TaskUtil.Completed;
        }


        class OwinHostScope
        {
            readonly TaskCompletionSource<HttpOwinHostContext> _owinHostContext;
            readonly ITaskScope _taskScope;

            public OwinHostScope(ITaskScope scope, HttpHostSettings settings)
            {
                _owinHostContext = new TaskCompletionSource<HttpOwinHostContext>();

                _taskScope = scope.CreateScope($"{TypeMetadataCache<OwinHostScope>.ShortName} - {settings.ToDebugString()}", CloseContext);
            }

            public void Connected(HttpOwinHostContext hostContext)
            {
                _owinHostContext.TrySetResult(hostContext);

                _taskScope.SetReady();
            }

            public void ConnectFaulted(Exception exception)
            {
                _owinHostContext.TrySetException(exception);

                _taskScope.SetNotReady(exception);

                _taskScope.Stop(new StopEventArgs($"Connection faulted: {exception.Message}"));
            }

            public async Task<SharedHttpOwinHostContext> Attach(CancellationToken cancellationToken)
            {
                var owinHostContext = await _owinHostContext.Task.ConfigureAwait(false);

                return new SharedHttpOwinHostContext(owinHostContext, cancellationToken, _taskScope);
            }

            public void Shutdown(string reason)
            {
                _taskScope.Stop(new StopEventArgs(reason));
            }

            async Task CloseContext()
            {
                if (_owinHostContext.Task.Status == TaskStatus.RanToCompletion)
                {
                    var owinHostContext = await _owinHostContext.Task.ConfigureAwait(false);

                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Disposing connection: {0}", owinHostContext.HostSettings.ToDebugString());

                    owinHostContext.Dispose();
                }
            }
        }
    }
}