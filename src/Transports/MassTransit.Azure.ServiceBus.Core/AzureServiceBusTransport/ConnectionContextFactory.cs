namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Agents;
    using Azure.Identity;
    using Azure.Messaging.ServiceBus;
    using Azure.Messaging.ServiceBus.Administration;
    using Configuration;
    using Internals;


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
            Task<ConnectionContext> context = Task.Run(() => CreateConnection(supervisor), supervisor.Stopped);

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
            var endpoint = new UriBuilder(_hostConfiguration.HostAddress) { Path = "" }.Uri.Host;

            if (supervisor.Stopping.IsCancellationRequested)
                throw new ServiceBusConnectionException($"The connection is stopping and cannot be used: {endpoint}");

            var settings = _hostConfiguration.Settings;

            var client = settings.ServiceBusClient;
            var managementClient = settings.ServiceBusAdministrationClient;

            var clientOptions = new ServiceBusClientOptions
            {
                TransportType = settings.TransportType,
                RetryOptions = new ServiceBusRetryOptions
                {
                    MaxRetries = settings.RetryLimit,
                    Mode = ServiceBusRetryMode.Exponential,
                    MaxDelay = settings.RetryMaxBackoff,
                },
                EnableCrossEntityTransactions = false,
            };

            if (settings.TokenCredential != null)
            {
                client ??= new ServiceBusClient(endpoint, settings.TokenCredential, clientOptions);
                managementClient ??= new ServiceBusAdministrationClient(endpoint, settings.TokenCredential);
            }
            else if (settings.NamedKeyCredential != null)
            {
                client ??= new ServiceBusClient(endpoint, settings.NamedKeyCredential, clientOptions);
                managementClient ??= new ServiceBusAdministrationClient(endpoint, settings.NamedKeyCredential);
            }
            else if (settings.SasCredential != null)
            {
                client ??= new ServiceBusClient(endpoint, settings.SasCredential, clientOptions);
                managementClient ??= new ServiceBusAdministrationClient(endpoint, settings.SasCredential);
            }
            else
            {
                if (settings.ConnectionString != null && HasSharedAccess(settings.ConnectionString))
                {
                    client ??= new ServiceBusClient(settings.ConnectionString, clientOptions);
                    managementClient ??= new ServiceBusAdministrationClient(settings.ConnectionString);
                }
                else
                {
                    var defaultAzureCredential = new DefaultAzureCredential();

                    client ??= new ServiceBusClient(endpoint, defaultAzureCredential, clientOptions);
                    managementClient ??= new ServiceBusAdministrationClient(endpoint, defaultAzureCredential);
                }
            }

            return new ServiceBusConnectionContext(client, managementClient, supervisor.Stopped);
        }

        static bool HasSharedAccess(string connectionString)
        {
            var connectionStringProperties = ServiceBusConnectionStringProperties.Parse(connectionString);

            return !string.IsNullOrEmpty(connectionStringProperties.SharedAccessKeyName)
                && !string.IsNullOrEmpty(connectionStringProperties.SharedAccessKey) || !string.IsNullOrEmpty(connectionStringProperties.SharedAccessSignature);
        }
    }
}
