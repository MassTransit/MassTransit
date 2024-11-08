namespace MassTransit.RabbitMqTransport
{
    using System.Threading;
    using System.Threading.Tasks;
    using Agents;
    using Internals;
    using RabbitMQ.Client.Events;


    public class ChannelContextFactory :
        IPipeContextFactory<ChannelContext>
    {
        readonly IConnectionContextSupervisor _supervisor;
        readonly ushort? _concurrentMessageLimit;

        public ChannelContextFactory(IConnectionContextSupervisor supervisor, ushort? concurrentMessageLimit)
        {
            _supervisor = supervisor;
            _concurrentMessageLimit = concurrentMessageLimit;
        }

        IPipeContextAgent<ChannelContext> IPipeContextFactory<ChannelContext>.CreateContext(ISupervisor supervisor)
        {
            IAsyncPipeContextAgent<ChannelContext> asyncContext = supervisor.AddAsyncContext<ChannelContext>();

            Task<ChannelContext> context = CreateChannel(asyncContext, supervisor.Stopped);

            async Task HandleShutdown(object sender, ShutdownEventArgs args)
            {
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
                ? new ScopeChannelContext(context.Result, cancellationToken)
                : new ScopeChannelContext(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
        }

        Task<ChannelContext> CreateChannel(IAsyncPipeContextAgent<ChannelContext> asyncContext, CancellationToken cancellationToken)
        {
            static Task<ChannelContext> CreateChannelContext(ConnectionContext connectionContext, CancellationToken createCancellationToken, ushort? concurrentMessageLimit)
            {
                return connectionContext.CreateChannelContext(createCancellationToken, concurrentMessageLimit);
            }

            return _supervisor.CreateAgent(asyncContext, (context,token) => CreateChannelContext(context,token,_concurrentMessageLimit), cancellationToken);
        }
    }
}
