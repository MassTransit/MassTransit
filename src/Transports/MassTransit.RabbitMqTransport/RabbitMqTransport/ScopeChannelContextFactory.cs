namespace MassTransit.RabbitMqTransport
{
    using System.Threading;
    using System.Threading.Tasks;
    using Agents;
    using Internals;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;


    public class ScopeChannelContextFactory :
        IPipeContextFactory<ChannelContext>
    {
        readonly IChannelContextSupervisor _supervisor;

        public ScopeChannelContextFactory(IChannelContextSupervisor supervisor)
        {
            _supervisor = supervisor;
        }

        IPipeContextAgent<ChannelContext> IPipeContextFactory<ChannelContext>.CreateContext(ISupervisor supervisor)
        {
            IAsyncPipeContextAgent<ChannelContext> asyncContext = supervisor.AddAsyncContext<ChannelContext>();

            Task<ChannelContext> context = CreateChannel(asyncContext, supervisor.Stopped);

            async Task HandleShutdown(object sender, ShutdownEventArgs args)
            {
                if (args.Initiator != ShutdownInitiator.Application)
                    await asyncContext.Stop(args.ReplyText).ConfigureAwait(false);
            }

            context.ContinueWith(task =>
            {
                task.Result.Channel.ChannelShutdownAsync += HandleShutdown;

                asyncContext.Completed.ContinueWith(_ => task.Result.Channel.ChannelShutdownAsync -= HandleShutdown);
            }, TaskContinuationOptions.OnlyOnRanToCompletion);

            return asyncContext;
        }

        IActivePipeContextAgent<ChannelContext> IPipeContextFactory<ChannelContext>.CreateActiveContext(ISupervisor supervisor,
            PipeContextHandle<ChannelContext> context, CancellationToken cancellationToken)
        {
            return supervisor.AddActiveContext(context, CreateSharedChannel(context.Context, cancellationToken));
        }

        static async Task<ChannelContext> CreateSharedChannel(Task<ChannelContext> context, CancellationToken cancellationToken)
        {
            return context.IsCompletedSuccessfully()
                ? new SharedChannelContext(context.Result, cancellationToken)
                : new SharedChannelContext(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
        }

        Task<ChannelContext> CreateChannel(IAsyncPipeContextAgent<ChannelContext> asyncContext, CancellationToken cancellationToken)
        {
            static Task<ChannelContext> CreateChannelContext(ChannelContext context, CancellationToken createCancellationToken)
            {
                return Task.FromResult<ChannelContext>(new SharedChannelContext(context, createCancellationToken));
            }

            return _supervisor.CreateAgent(asyncContext, CreateChannelContext, cancellationToken);
        }
    }
}
