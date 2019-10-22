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
    using Configuration;
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using Microsoft.ServiceBus;


    public class NamespaceContextFactory :
        IPipeContextFactory<NamespaceContext>
    {
        readonly Uri _serviceUri;
        readonly NamespaceManagerSettings _settings;

        public NamespaceContextFactory(IServiceBusHostConfiguration configuration)
        {
            _serviceUri = new UriBuilder(configuration.HostAddress) {Path = ""}.Uri;

            _settings = CreateNamespaceManagerSettings(configuration.Settings, CreateRetryPolicy(configuration.Settings));
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

        static NamespaceManagerSettings CreateNamespaceManagerSettings(ServiceBusHostSettings settings, RetryPolicy retryPolicy)
        {
            var nms = new NamespaceManagerSettings
            {
                TokenProvider = settings.TokenProvider,
                OperationTimeout = settings.OperationTimeout,
                RetryPolicy = retryPolicy
            };

            return nms;
        }

        static RetryPolicy CreateRetryPolicy(ServiceBusHostSettings settings)
        {
            return new RetryExponential(settings.RetryMinBackoff, settings.RetryMaxBackoff, settings.RetryLimit);
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

                LogContext.Debug?.Log("Created NamespaceManager: {ServiceUri}", _serviceUri);

                var namespaceManager = new NamespaceManager(_serviceUri, _settings);

                var context = new ServiceBusNamespaceContext(namespaceManager, supervisor.Stopped);

                return context;
            }
            catch (Exception ex)
            {
                LogContext.Error?.Log(ex, "Create NamespaceManager faulted: {ServiceUri}", _serviceUri);

                throw;
            }
        }
    }
}
