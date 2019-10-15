namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using Microsoft.Azure.ServiceBus;


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

            return new SharedNamespaceContext(connectionContext, cancellationToken);
        }

        async Task<NamespaceContext> CreateNamespaceContext(ISupervisor supervisor)
        {
            try
            {
                if (supervisor.Stopping.IsCancellationRequested)
                    throw new OperationCanceledException($"The namespace is stopping and cannot be used: {_serviceUri}");

                var namespaceManager = new NamespaceManager(_serviceUri, _settings);

                LogContext.Debug?.Log("Created NamespaceManager: {ServiceUri}", _serviceUri);

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
