namespace MassTransit.SqlTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Agents;
    using Internals;
    using Transports;


    public abstract class ConnectionContextFactory :
        IPipeContextFactory<ConnectionContext>
    {
        public IPipeContextAgent<ConnectionContext> CreateContext(ISupervisor supervisor)
        {
            ITransportSupervisor<ConnectionContext> transportSupervisor =
                supervisor as ITransportSupervisor<ConnectionContext> ?? throw new ArgumentException(nameof(supervisor));

            return supervisor.AddContext(CreateConnection(transportSupervisor));
        }

        public IActivePipeContextAgent<ConnectionContext> CreateActiveContext(ISupervisor supervisor,
            PipeContextHandle<ConnectionContext> context, CancellationToken cancellationToken)
        {
            return supervisor.AddActiveContext(context, CreateSharedConnection(context.Context, cancellationToken));
        }

        static async Task<ConnectionContext> CreateSharedConnection(Task<ConnectionContext> context, CancellationToken cancellationToken)
        {
            return context.Status == TaskStatus.RanToCompletion
                ? new SharedConnectionContext(context.Result, cancellationToken)
                : new SharedConnectionContext(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
        }

        protected abstract ConnectionContext CreateConnection(ITransportSupervisor<ConnectionContext> supervisor);
    }
}
