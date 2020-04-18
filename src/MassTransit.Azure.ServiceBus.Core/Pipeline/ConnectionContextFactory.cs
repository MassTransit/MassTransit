namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Management;


    public class ConnectionContextFactory :
        IPipeContextFactory<ConnectionContext>
    {
        readonly ServiceBusHostSettings _settings;
        readonly RetryPolicy _retryPolicy;
        readonly Uri _endpoint;

        public ConnectionContextFactory(IServiceBusHostConfiguration configuration)
        {
            _endpoint = new UriBuilder(configuration.HostAddress) {Path = ""}.Uri;

            _settings = configuration.Settings;
            _retryPolicy = CreateRetryPolicy(_settings);
        }

        IPipeContextAgent<ConnectionContext> IPipeContextFactory<ConnectionContext>.CreateContext(ISupervisor supervisor)
        {
            var context = Task.Run(() => CreateConnection(supervisor), supervisor.Stopped);

            IPipeContextAgent<ConnectionContext> contextHandle = supervisor.AddContext(context);

            return contextHandle;
        }

        IActivePipeContextAgent<ConnectionContext> IPipeContextFactory<ConnectionContext>.CreateActiveContext(ISupervisor supervisor,
            PipeContextHandle<ConnectionContext> context, CancellationToken cancellationToken)
        {
            return supervisor.AddActiveContext(context, CreateSharedConnection(context.Context, cancellationToken));
        }

        static RetryPolicy CreateRetryPolicy(ServiceBusHostSettings settings)
        {
            return new RetryExponential(settings.RetryMinBackoff, settings.RetryMaxBackoff, settings.RetryLimit);
        }

        async Task<ConnectionContext> CreateSharedConnection(Task<ConnectionContext> context, CancellationToken cancellationToken)
        {
            return context.IsCompletedSuccessfully()
                ? new SharedConnectionContext(context.Result, cancellationToken)
                : new SharedConnectionContext(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
        }

        async Task<ConnectionContext> CreateConnection(ISupervisor supervisor)
        {
            if (supervisor.Stopping.IsCancellationRequested)
                throw new OperationCanceledException($"The connection is stopping and cannot be used: {_endpoint}");

            var connection = new ServiceBusConnection(_endpoint.ToString(), _settings.TransportType, _retryPolicy)
            {
                TokenProvider = _settings.TokenProvider,
                OperationTimeout = _settings.OperationTimeout,
            };

            var managementClient = new ManagementClient(_endpoint.ToString(), _settings.TokenProvider);

            return new ServiceBusConnectionContext(connection, managementClient, _retryPolicy, _settings.OperationTimeout, supervisor.Stopped);
        }
    }
}
