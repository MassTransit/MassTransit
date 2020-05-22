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
        readonly IServiceBusHostConfiguration _hostConfiguration;

        public ConnectionContextFactory(IServiceBusHostConfiguration hostConfiguration)
        {
            _hostConfiguration = hostConfiguration;
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

        async Task<ConnectionContext> CreateSharedConnection(Task<ConnectionContext> context, CancellationToken cancellationToken)
        {
            return context.IsCompletedSuccessfully()
                ? new SharedConnectionContext(context.Result, cancellationToken)
                : new SharedConnectionContext(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
        }

        async Task<ConnectionContext> CreateConnection(ISupervisor supervisor)
        {
            var endpoint = new UriBuilder(_hostConfiguration.HostAddress) {Path = ""}.Uri.ToString();

            if (supervisor.Stopping.IsCancellationRequested)
                throw new OperationCanceledException($"The connection is stopping and cannot be used: {endpoint}");

            var settings = _hostConfiguration.Settings;
            var retryPolicy = new RetryExponential(settings.RetryMinBackoff, settings.RetryMaxBackoff, settings.RetryLimit);

            var connection = new ServiceBusConnection(endpoint, settings.TransportType, retryPolicy)
            {
                TokenProvider = settings.TokenProvider,
                OperationTimeout = settings.OperationTimeout,
            };

            var managementClient = new ManagementClient(endpoint, settings.TokenProvider);

            return new ServiceBusConnectionContext(connection, managementClient, retryPolicy, settings.OperationTimeout, supervisor.Stopped);
        }
    }
}
