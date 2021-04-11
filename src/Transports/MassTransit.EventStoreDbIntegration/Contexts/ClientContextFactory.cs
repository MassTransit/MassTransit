using System.Threading;
using System.Threading.Tasks;
using GreenPipes;
using GreenPipes.Agents;
using GreenPipes.Internals.Extensions;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public class ClientContextFactory :
        IPipeContextFactory<ClientContext>
    {
        readonly IHostSettings _hostSettings;

        public ClientContextFactory(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
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
            return new EventStoreDbClientContext(_hostSettings, supervisor.Stopped);
        }
    }
}
