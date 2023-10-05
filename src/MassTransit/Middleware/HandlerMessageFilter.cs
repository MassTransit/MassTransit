namespace MassTransit.Middleware
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;


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
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
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
            var timer = Stopwatch.StartNew();
            StartedActivity? activity = LogContext.Current?.StartHandlerActivity(context);
            StartedInstrument? instrument = LogContext.Current?.StartHandlerInstrument(context, timer);

            try
            {
                await _handler(context).ConfigureAwait(false);

                await context.NotifyConsumed(timer.Elapsed, TypeCache<MessageHandler<TMessage>>.ShortName).ConfigureAwait(false);

                Interlocked.Increment(ref _completed);

                await next.Send(context).ConfigureAwait(false);
            }
            catch (Exception exception) when ((exception is OperationCanceledException || exception.GetBaseException() is OperationCanceledException)
                                              && !context.CancellationToken.IsCancellationRequested)
            {
                await context.NotifyFaulted(timer.Elapsed, TypeCache<MessageHandler<TMessage>>.ShortName, exception).ConfigureAwait(false);

                activity?.AddExceptionEvent(exception);

                instrument?.AddException(exception);

                throw new ConsumerCanceledException($"The operation was canceled by the consumer: {TypeCache<MessageHandler<TMessage>>.ShortName}");
            }
            catch (Exception ex)
            {
                await context.NotifyFaulted(timer.Elapsed, TypeCache<MessageHandler<TMessage>>.ShortName, ex).ConfigureAwait(false);

                activity?.AddExceptionEvent(ex);
                instrument?.AddException(ex);

                Interlocked.Increment(ref _faulted);
                throw;
            }
            finally
            {
                activity?.Stop();
                instrument?.Stop();
            }
        }
    }
}
