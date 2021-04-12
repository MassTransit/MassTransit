using System.Threading;
using System.Threading.Tasks;
using GreenPipes;
using GreenPipes.Agents;
using GreenPipes.Internals.Extensions;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public class ConnectionContextFactory :
        IPipeContextFactory<ConnectionContext>
    {
        readonly IHostSettings _hostSettings;

        public ConnectionContextFactory(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
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
            return supervisor.AddActiveContext(context, CreateSharedConnectionContext(context.Context, cancellationToken));
        }

        static async Task<ConnectionContext> CreateSharedConnectionContext(Task<ConnectionContext> context, CancellationToken cancellationToken)
        {
            return context.IsCompletedSuccessfully()
                ? new SharedConnectionContext(context.Result, cancellationToken)
                : new SharedConnectionContext(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
        }

        ConnectionContext CreateConnectionContext(ISupervisor supervisor)
        {
            return new EventStoreDbConnectionContext(_hostSettings, supervisor.Stopped);
        }
    }
}
