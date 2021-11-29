namespace MassTransit.RabbitMqTransport
{
    using System.Threading;
    using System.Threading.Tasks;
    using Agents;
    using Internals;
    using RabbitMQ.Client;


    public class ModelContextFactory :
        IPipeContextFactory<ModelContext>
    {
        readonly IConnectionContextSupervisor _supervisor;

        public ModelContextFactory(IConnectionContextSupervisor supervisor)
        {
            _supervisor = supervisor;
        }

        IPipeContextAgent<ModelContext> IPipeContextFactory<ModelContext>.CreateContext(ISupervisor supervisor)
        {
            IAsyncPipeContextAgent<ModelContext> asyncContext = supervisor.AddAsyncContext<ModelContext>();

            Task<ModelContext> context = CreateModel(asyncContext, supervisor.Stopped);

            void HandleShutdown(object sender, ShutdownEventArgs args)
            {
                if (args.Initiator != ShutdownInitiator.Application)
                    asyncContext.Stop(args.ReplyText);
            }

            context.ContinueWith(task =>
            {
                task.Result.Model.ModelShutdown += HandleShutdown;

                asyncContext.Completed.ContinueWith(_ => task.Result.Model.ModelShutdown -= HandleShutdown);
            }, TaskContinuationOptions.OnlyOnRanToCompletion);

            return asyncContext;
        }

        IActivePipeContextAgent<ModelContext> IPipeContextFactory<ModelContext>.CreateActiveContext(ISupervisor supervisor,
            PipeContextHandle<ModelContext> context, CancellationToken cancellationToken)
        {
            return supervisor.AddActiveContext(context, CreateSharedModel(context.Context, cancellationToken));
        }

        static async Task<ModelContext> CreateSharedModel(Task<ModelContext> context, CancellationToken cancellationToken)
        {
            return context.IsCompletedSuccessfully()
                ? new ScopeModelContext(context.Result, cancellationToken)
                : new ScopeModelContext(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
        }

        Task<ModelContext> CreateModel(IAsyncPipeContextAgent<ModelContext> asyncContext, CancellationToken cancellationToken)
        {
            static Task<ModelContext> CreateModelContext(ConnectionContext connectionContext, CancellationToken createCancellationToken)
            {
                return connectionContext.CreateModelContext(createCancellationToken);
            }

            return _supervisor.CreateAgent(asyncContext, CreateModelContext, cancellationToken);
        }
    }
}
