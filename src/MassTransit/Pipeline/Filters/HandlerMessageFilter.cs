namespace MassTransit.Pipeline.Filters
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Logging;
    using Metadata;


    /// <summary>
    /// Consumes a message via a message handler and reports the message as consumed or faulted
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class HandlerMessageFilter<TMessage> :
        IFilter<ConsumeContext<TMessage>>
        where TMessage : class
    {
        readonly MessageHandler<TMessage> _handler;
        long _completed;
        long _faulted;

        // TODO this needs a pipe like instance and consumer, to handle things like retry, etc.
        public HandlerMessageFilter(MessageHandler<TMessage> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            _handler = handler;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateFilterScope("handler");
            scope.Add("completed", _completed);
            scope.Add("faulted", _faulted);
        }

        [DebuggerNonUserCode]
        async Task IFilter<ConsumeContext<TMessage>>.Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            var activity = LogContext.IfEnabled(OperationName.Consumer.Handle)?.StartHandlerActivity(context);

            Stopwatch timer = Stopwatch.StartNew();
            try
            {
                await Task.Yield();

                await _handler(context).ConfigureAwait(false);

                await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<MessageHandler<TMessage>>.ShortName).ConfigureAwait(false);

                Interlocked.Increment(ref _completed);

                await next.Send(context).ConfigureAwait(false);
            }
            catch (OperationCanceledException exception)
            {
                await context.NotifyFaulted(timer.Elapsed, TypeMetadataCache<MessageHandler<TMessage>>.ShortName, exception).ConfigureAwait(false);

                if (exception.CancellationToken == context.CancellationToken)
                    throw;

                throw new ConsumerCanceledException($"The operation was cancelled by the consumer: {TypeMetadataCache<MessageHandler<TMessage>>.ShortName}");
            }
            catch (Exception ex)
            {
                await context.NotifyFaulted(timer.Elapsed, TypeMetadataCache<MessageHandler<TMessage>>.ShortName, ex).ConfigureAwait(false);

                Interlocked.Increment(ref _faulted);
                throw;
            }
            finally
            {
                activity?.Stop();
            }
        }
    }
}
