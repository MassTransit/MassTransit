namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Agents;
    using Internals;
    using RabbitMQ.Client.Events;


    public class ChannelContextFactory :
        IPipeContextFactory<ChannelContext>
    {
        readonly ushort? _concurrentMessageLimit;
        readonly IConnectionContextSupervisor _supervisor;

        public ChannelContextFactory(IConnectionContextSupervisor supervisor, ushort? concurrentMessageLimit)
        {
            _supervisor = supervisor;
            _concurrentMessageLimit = concurrentMessageLimit;
        }

        public IPipeContextAgent<ChannelContext> CreateContext(ISupervisor supervisor)
        {
            IAsyncPipeContextAgent<ChannelContext> asyncContext = supervisor.AddAsyncContext<ChannelContext>();

            Task<ChannelContext> context = CreateChannel(asyncContext, supervisor.Stopped);

            Task HandleShutdown(object sender, ShutdownEventArgs args)
            {
                _ = Task.Run(() => asyncContext.Stop(args.ReplyText));

                return Task.CompletedTask;
            }

            context.ContinueWith(task =>
            {
                var channelContext = task.Result;

                channelContext.Channel.ChannelShutdownAsync += HandleShutdown;
                channelContext.ConnectionContext.Connection.ConnectionShutdownAsync += HandleShutdown;

                void RemoveHandlers()
                {
                    try
                    {
                        channelContext.ConnectionContext.Connection.ConnectionShutdownAsync -= HandleShutdown;
                    }
                    catch (ObjectDisposedException)
                    {
                    }

                    try
                    {
                        channelContext.Channel.ChannelShutdownAsync -= HandleShutdown;
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                }

                asyncContext.Completed.ContinueWith(_ => RemoveHandlers());
            }, TaskContinuationOptions.OnlyOnRanToCompletion);

            return asyncContext;
        }

        public IActivePipeContextAgent<ChannelContext> CreateActiveContext(ISupervisor supervisor,
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
            static Task<ChannelContext> CreateChannelContext(ConnectionContext connectionContext, CancellationToken createCancellationToken,
                ushort? concurrentMessageLimit)
            {
                return connectionContext.CreateChannelContext(createCancellationToken, concurrentMessageLimit);
            }

            return _supervisor.CreateAgent(asyncContext, (context, token) => CreateChannelContext(context, token, _concurrentMessageLimit), cancellationToken);
        }
    }
}
