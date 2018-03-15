// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport.Pipeline
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using Logging;
    using Microsoft.ServiceBus;


    public class NamespaceContextFactory :
        IPipeContextFactory<NamespaceContext>
    {
        static readonly ILog _log = Logger.Get<NamespaceContextFactory>();
        readonly Uri _serviceUri;
        readonly NamespaceManagerSettings _settings;

        public NamespaceContextFactory(Uri serviceUri, NamespaceManagerSettings settings)
        {
            _serviceUri = new UriBuilder(serviceUri) {Path = ""}.Uri;

            _settings = settings;
        }

        IPipeContextAgent<NamespaceContext> IPipeContextFactory<NamespaceContext>.CreateContext(ISupervisor supervisor)
        {
            var context = Task.Factory.StartNew(() => CreateNamespaceContext(supervisor), supervisor.Stopping, TaskCreationOptions.None, TaskScheduler.Default)
                .Unwrap();

            IPipeContextAgent<NamespaceContext> contextHandle = supervisor.AddContext(context);

            return contextHandle;
        }

        IActivePipeContextAgent<NamespaceContext> IPipeContextFactory<NamespaceContext>.CreateActiveContext(ISupervisor supervisor,
            PipeContextHandle<NamespaceContext> context, CancellationToken cancellationToken)
        {
            return supervisor.AddActiveContext(context, CreateSharedConnection(context.Context, cancellationToken));
        }

        async Task<NamespaceContext> CreateSharedConnection(Task<NamespaceContext> context, CancellationToken cancellationToken)
        {
            var connectionContext = await context.ConfigureAwait(false);

            var sharedConnection = new SharedNamespaceContext(connectionContext, cancellationToken);

            return sharedConnection;
        }

        async Task<NamespaceContext> CreateNamespaceContext(ISupervisor supervisor)
        {
            try
            {
                if (supervisor.Stopping.IsCancellationRequested)
                    throw new OperationCanceledException($"The namespace is stopping and cannot be used: {_serviceUri}");

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Creating namespace manager: {0}", _serviceUri);

                var namespaceManager = new NamespaceManager(_serviceUri, _settings);

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Created namespace manager: {0}", _serviceUri);

                var context = new ServiceBusNamespaceContext(namespaceManager, supervisor.Stopped);

                return context;
            }
            catch (Exception ex)
            {
                if (_log.IsDebugEnabled)
                    _log.Debug($"Namespace Create Failed: {_serviceUri}", ex);

                throw;
            }
        }
    }
}