namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using Microsoft.Azure.ServiceBus;


    public class MessagingFactoryContextFactory :
        IPipeContextFactory<MessagingFactoryContext>
    {
        readonly MessagingFactorySettings _messagingFactorySettings;
        readonly RetryPolicy _retryPolicy;
        readonly Uri _serviceUri;

        public MessagingFactoryContextFactory(IServiceBusHostConfiguration configuration)
        {
            _serviceUri = new UriBuilder(configuration.HostAddress) {Path = ""}.Uri;

            _messagingFactorySettings = CreateMessagingFactorySettings(configuration.Settings);
            _retryPolicy = CreateRetryPolicy(configuration.Settings);
        }

        IPipeContextAgent<MessagingFactoryContext> IPipeContextFactory<MessagingFactoryContext>.CreateContext(ISupervisor supervisor)
        {
            var context = Task.Run(() => CreateConnection(supervisor), supervisor.Stopping);

            IPipeContextAgent<MessagingFactoryContext> contextHandle = supervisor.AddContext(context);

            return contextHandle;
        }

        IActivePipeContextAgent<MessagingFactoryContext> IPipeContextFactory<MessagingFactoryContext>.CreateActiveContext(ISupervisor supervisor,
            PipeContextHandle<MessagingFactoryContext> context, CancellationToken cancellationToken)
        {
            return supervisor.AddActiveContext(context, CreateSharedConnection(context.Context, cancellationToken));
        }

        static MessagingFactorySettings CreateMessagingFactorySettings(ServiceBusHostSettings settings)
        {
            var mfs = new MessagingFactorySettings
            {
                TokenProvider = settings.TokenProvider,
                OperationTimeout = settings.OperationTimeout,
                TransportType = settings.TransportType
            };

            return mfs;
        }

        static RetryPolicy CreateRetryPolicy(ServiceBusHostSettings settings)
        {
            return new RetryExponential(settings.RetryMinBackoff, settings.RetryMaxBackoff, settings.RetryLimit);
        }

        async Task<MessagingFactoryContext> CreateSharedConnection(Task<MessagingFactoryContext> context, CancellationToken cancellationToken)
        {
            var connectionContext = await context.ConfigureAwait(false);

            return new SharedMessagingFactoryContext(connectionContext, cancellationToken);
        }

        async Task<MessagingFactoryContext> CreateConnection(ISupervisor supervisor)
        {
            if (supervisor.Stopping.IsCancellationRequested)
                throw new OperationCanceledException($"The connection is stopping and cannot be used: {_serviceUri}");

            var messagingFactory = MessagingFactory.Create(_serviceUri, _messagingFactorySettings);

            messagingFactory.RetryPolicy = _retryPolicy;

            return new ServiceBusMessagingFactoryContext(messagingFactory, supervisor.Stopped);
        }
    }
}
