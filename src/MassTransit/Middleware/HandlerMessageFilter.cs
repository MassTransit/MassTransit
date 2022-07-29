namespace MassTransit.Middleware
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using Util;


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
            var scope = context.CreateFilterScope("handler");
            scope.Add("completed", _completed);
            scope.Add("faulted", _faulted);
        }

        [DebuggerNonUserCode]
        async Task IFilter<ConsumeContext<TMessage>>.Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            StartedActivity? activity = LogContext.Current?.StartHandlerActivity(context);

            var timer = Stopwatch.StartNew();
            try
            {
                await _handler(context).ConfigureAwait(false);

                await context.NotifyConsumed(timer.Elapsed, TypeCache<MessageHandler<TMessage>>.ShortName).ConfigureAwait(false);

                Interlocked.Increment(ref _completed);

                await next.Send(context).ConfigureAwait(false);
            }
            catch (OperationCanceledException exception)
            {
                await ThrowHelper.ConsumeFilter.ThrowOperationCancelled<MessageHandler<TMessage>, TMessage>(context, timer.Elapsed, exception, activity)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _faulted);

                await ThrowHelper.ConsumeFilter.Throw<MessageHandler<TMessage>, TMessage>(context, timer.Elapsed, ex, activity).ConfigureAwait(false);

                throw;
            }
            finally
            {
                activity?.Stop();
            }
        }
    }
}
