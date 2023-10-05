namespace MassTransit.Middleware
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Context;
    using Logging;


    /// <summary>
    /// Consumes a message via an existing class instance
    /// </summary>
    /// <typeparam name="TConsumer"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class InstanceMessageFilter<TConsumer, TMessage> :
        IFilter<ConsumeContext<TMessage>>
        where TConsumer : class
        where TMessage : class
    {
        readonly TConsumer _instance;
        readonly IPipe<ConsumerConsumeContext<TConsumer, TMessage>> _instancePipe;

        public InstanceMessageFilter(TConsumer instance, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> instancePipe)
        {
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
            _instancePipe = instancePipe ?? throw new ArgumentNullException(nameof(instancePipe));
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("instance");
            scope.Add("type", TypeCache<TConsumer>.ShortName);

            _instancePipe.Probe(scope);
        }

        [DebuggerNonUserCode]
        async Task IFilter<ConsumeContext<TMessage>>.Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            var timer = Stopwatch.StartNew();

            StartedActivity? activity = LogContext.Current?.StartConsumerActivity<TConsumer, TMessage>(context);
            StartedInstrument? instrument = LogContext.Current?.StartConsumeInstrument<TConsumer, TMessage>(context, timer);

            try
            {
                await _instancePipe.Send(new ConsumerConsumeContextScope<TConsumer, TMessage>(context, _instance)).ConfigureAwait(false);

                await context.NotifyConsumed(timer.Elapsed, TypeCache<TConsumer>.ShortName).ConfigureAwait(false);

                await next.Send(context).ConfigureAwait(false);
            }
            catch (Exception exception) when ((exception is OperationCanceledException || exception.GetBaseException() is OperationCanceledException)
                                              && !context.CancellationToken.IsCancellationRequested)
            {
                await context.NotifyFaulted(timer.Elapsed, TypeCache<TConsumer>.ShortName, exception).ConfigureAwait(false);

                activity?.AddExceptionEvent(exception);
                instrument?.AddException(exception);

                throw new ConsumerCanceledException($"The operation was canceled by the consumer: {TypeCache<TConsumer>.ShortName}");
            }
            catch (Exception exception)
            {
                await context.NotifyFaulted(timer.Elapsed, TypeCache<TConsumer>.ShortName, exception).ConfigureAwait(false);

                activity?.AddExceptionEvent(exception);
                instrument?.AddException(exception);

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
