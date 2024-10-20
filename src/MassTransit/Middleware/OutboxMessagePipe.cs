#nullable enable
namespace MassTransit.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using DependencyInjection;
    using Logging;
    using Serialization;


    public class OutboxMessagePipe<TMessage> :
        IPipe<OutboxConsumeContext<TMessage>>
        where TMessage : class
    {
        readonly IPipe<ConsumeContext<TMessage>> _next;
        readonly OutboxConsumeOptions _options;
        readonly IConsumeScopeContext<TMessage> _scopeContext;

        public OutboxMessagePipe(OutboxConsumeOptions options, IConsumeScopeContext<TMessage> scopeContext, IPipe<ConsumeContext<TMessage>> next)
        {
            _options = options;
            _scopeContext = scopeContext;
            _next = next;
        }

        public async Task Send(OutboxConsumeContext<TMessage> context)
        {
            using var pop = _scopeContext.PushConsumeContext(context);

            var timer = Stopwatch.StartNew();

            if (!context.IsMessageConsumed)
            {
                await _next.Send(context).ConfigureAwait(false);

                await context.ConsumeCompleted.ConfigureAwait(false);

                try
                {
                    await context.SetConsumed().ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    if (!context.ReceiveContext.IsFaulted)
                        await context.NotifyFaulted(timer.Elapsed, TypeCache<TMessage>.ShortName, exception).ConfigureAwait(false);

                    throw;
                }

                return;
            }

            if (!context.IsOutboxDelivered)
            {
                await DeliverOutboxMessages(context).ConfigureAwait(false);

                await context.ConsumeCompleted.ConfigureAwait(false);

                return;
            }

            await context.RemoveOutboxMessages().ConfigureAwait(false);

            LogContext.Debug?.Log("Outbox Completed: {MessageId} ({ReceiveCount})", context.MessageId, context.ReceiveCount);

            if (context.ReceiveContext is { IsDelivered: false, IsFaulted: false })
                await context.NotifyConsumed(context, timer.Elapsed, _options.ConsumerType).ConfigureAwait(false);

            context.ContinueProcessing = false;
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("outbox");

            _next.Probe(scope);
        }

        async Task DeliverOutboxMessages(OutboxConsumeContext context)
        {
            List<OutboxMessageContext> messages = await context.LoadOutboxMessages().ConfigureAwait(false);

            var messageLimit = _options.MessageDeliveryLimit;
            var messageCount = 0;
            var messageIndex = 0;
            for (; messageIndex < messages.Count && messageCount < messageLimit; messageIndex++)
            {
                var message = messages[messageIndex];

                if (context.LastSequenceNumber != null && context.LastSequenceNumber >= message.SequenceNumber)
                {
                }
                else if (message.DestinationAddress == null)
                {
                    LogContext.Warning?.Log("Outbox message DestinationAddress not present: {SequenceNumber} {MessageId}", message.SequenceNumber,
                        message.MessageId);
                }
                else
                {
                    using var sendToken = new CancellationTokenSource(_options.MessageDeliveryTimeout);
                    using var token = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken, sendToken.Token);

                    var pipe = new OutboxMessageSendPipe(message, message.DestinationAddress);

                    var endpoint = await context.CapturedContext.GetSendEndpoint(message.DestinationAddress).ConfigureAwait(false);

                    var failDelivery = context.GetRetryAttempt() == 0 && (message.Headers.Get<bool>("MT-Fail-Delivery") ?? false);
                    if (failDelivery)
                        throw new ApplicationException("Simulated Delivery Failure Requested");

                    StartedActivity? activity = LogContext.Current?.StartOutboxDeliverActivity(message);
                    StartedInstrument? instrument = LogContext.Current?.StartOutboxDeliveryInstrument(context, message);
                    try
                    {
                        await endpoint.Send(new SerializedMessageBody(), pipe, token.Token).ConfigureAwait(false);
                    }
                    catch (Exception exception)
                    {
                        activity?.AddExceptionEvent(exception);
                        instrument?.AddException(exception);

                        throw;
                    }
                    finally
                    {
                        activity?.Stop();
                        instrument?.Stop();
                    }

                    LogContext.Debug?.Log("Outbox Sent: {InboxMessageId} {SequenceNumber} {MessageId}", context.MessageId, message.SequenceNumber,
                        message.MessageId);

                    await context.NotifyOutboxMessageDelivered(message).ConfigureAwait(false);

                    messageCount++;
                }
            }

            if (messageIndex == messages.Count && messages.Count < messageLimit)
                await context.SetDelivered().ConfigureAwait(false);
        }
    }
}
