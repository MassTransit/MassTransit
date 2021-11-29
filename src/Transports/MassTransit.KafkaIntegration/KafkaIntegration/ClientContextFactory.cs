namespace MassTransit.KafkaIntegration
{
    using System.Threading;
    using System.Threading.Tasks;
    using Agents;
    using Confluent.Kafka;
    using Internals;


    public class ClientContextFactory :
        IPipeContextFactory<ClientContext>
    {
        readonly ClientConfig _clientConfig;

        public ClientContextFactory(ClientConfig clientConfig)
        {
            _clientConfig = clientConfig;
        }

        IPipeContextAgent<ClientContext> IPipeContextFactory<ClientContext>.CreateContext(ISupervisor supervisor)
        {
            Task<ClientContext> context = Task.FromResult(CreateClientContext(supervisor));

            IPipeContextAgent<ClientContext> contextHandle = supervisor.AddContext(context);

            return contextHandle;
        }

        IActivePipeContextAgent<ClientContext> IPipeContextFactory<ClientContext>.CreateActiveContext(ISupervisor supervisor,
            PipeContextHandle<ClientContext> context, CancellationToken cancellationToken)
        {
            return supervisor.AddActiveContext(context, CreateSharedClientContext(context.Context, cancellationToken));
        }

        static async Task<ClientContext> CreateSharedClientContext(Task<ClientContext> context, CancellationToken cancellationToken)
        {
            return context.IsCompletedSuccessfully()
                ? new SharedClientContext(context.Result, cancellationToken)
                : new SharedClientContext(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
        }

        ClientContext CreateClientContext(ISupervisor supervisor)
        {
            return new KafkaClientContext(_clientConfig, supervisor.Stopped);
        }
    }
}
