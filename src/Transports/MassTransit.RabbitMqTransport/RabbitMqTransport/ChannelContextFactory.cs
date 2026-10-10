namespace MassTransit.RabbitMqTransport;

using System;
using System.Threading;
using System.Threading.Tasks;
using Agents;
using Internals;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;


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
            Task.Run(() => asyncContext.Stop(args.ReplyText))
                .IgnoreUnobservedExceptions();

            return Task.CompletedTask;
        }

        context.ContinueWith(task =>
        {
            var channelContext = task.Result;

            channelContext.Channel.ChannelShutdownAsync += HandleShutdown;
            channelContext.ConnectionContext.Connection.ConnectionShutdownAsync += HandleShutdown;

            // A channel can close after it is created but before its shutdown handler is subscribed.
            // Check its state after subscribing so that the supervisor does not retain a dead channel.
            if (channelContext.Channel.IsClosed)
                _ = HandleShutdown(channelContext.Channel, channelContext.Channel.CloseReason);

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

    public IActivePipeContextAgent<ChannelContext> CreateActiveContext(ISupervisor supervisor, PipeContextHandle<ChannelContext> context,
        CancellationToken cancellationToken)
    {
        return supervisor.AddActiveContext(context, CreateSharedChannel(context.Context, cancellationToken));
    }

    static async Task<ChannelContext> CreateSharedChannel(Task<ChannelContext> contextTask, CancellationToken cancellationToken)
    {
        var context = contextTask.Status == TaskStatus.RanToCompletion
            ? contextTask.Result
            : await contextTask.OrCanceled(cancellationToken).ConfigureAwait(false);

        if (context.Channel.IsClosed)
            throw new OperationInterruptedException(
                new ShutdownEventArgs(ShutdownInitiator.Peer, 491, $"Channel is already closed: {context.Channel.CloseReason}"));

        return new ScopeChannelContext(context, cancellationToken);
    }

    Task<ChannelContext> CreateChannel(IAsyncPipeContextAgent<ChannelContext> asyncContext, CancellationToken cancellationToken)
    {
        Task<ChannelContext> CreateChannelContext(ConnectionContext connectionContext, CancellationToken createCancellationToken,
            ushort? concurrentMessageLimit)
        {
            return connectionContext.CreateChannelContext(asyncContext, concurrentMessageLimit, createCancellationToken);
        }

        return _supervisor.CreateAgent(asyncContext, (context, token) => CreateChannelContext(context, token, _concurrentMessageLimit), cancellationToken);
    }
}
