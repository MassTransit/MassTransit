namespace MassTransit.EventHubIntegration.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs.Producer;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;


    public class ConnectionContextFactory :
        IPipeContextFactory<ConnectionContext>
    {
        readonly Action<EventHubProducerClientOptions> _configureOptions;
        readonly IHostSettings _hostSettings;
        readonly IStorageSettings _storageSettings;

        public ConnectionContextFactory(IHostSettings hostSettings, IStorageSettings storageSettings, Action<EventHubProducerClientOptions> configureOptions)
        {
            _hostSettings = hostSettings;
            _storageSettings = storageSettings;
            _configureOptions = configureOptions;
        }

        IPipeContextAgent<ConnectionContext> IPipeContextFactory<ConnectionContext>.CreateContext(ISupervisor supervisor)
        {
            Task<ConnectionContext> context = Task.FromResult(CreateConnectionContext(supervisor));

            IPipeContextAgent<ConnectionContext> contextHandle = supervisor.AddContext(context);

            return contextHandle;
        }

        IActivePipeContextAgent<ConnectionContext> IPipeContextFactory<ConnectionContext>.CreateActiveContext(ISupervisor supervisor,
            PipeContextHandle<ConnectionContext> context, CancellationToken cancellationToken)
        {
            return supervisor.AddActiveContext(context, CreateSharedIKafkaConnectionContext(context.Context, cancellationToken));
        }

        static async Task<ConnectionContext> CreateSharedIKafkaConnectionContext(Task<ConnectionContext> context, CancellationToken cancellationToken)
        {
            return context.IsCompletedSuccessfully()
                ? new SharedConnectionContext(context.Result, cancellationToken)
                : new SharedConnectionContext(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
        }

        ConnectionContext CreateConnectionContext(ISupervisor supervisor)
        {
            return new EventHubConnectionContext(_hostSettings, _storageSettings, _configureOptions, supervisor.Stopped);
        }
    }
}
