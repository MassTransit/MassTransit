namespace MassTransit.Middleware
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Logging;
    using Util;


    /// <summary>
    /// Consumes a message via Consumer, resolved through the consumer factory and notifies the context that the message was consumed.
    /// </summary>
    /// <typeparam name="TConsumer">The consumer type</typeparam>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class ConsumerMessageFilter<TConsumer, TMessage> :
        IFilter<ConsumeContext<TMessage>>
        where TConsumer : class
        where TMessage : class
    {
        readonly IConsumerFactory<TConsumer> _consumerFactory;
        readonly IPipe<ConsumerConsumeContext<TConsumer, TMessage>> _consumerPipe;

        public ConsumerMessageFilter(IConsumerFactory<TConsumer> consumerFactory, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> consumerPipe)
        {
            _consumerFactory = consumerFactory;
            _consumerPipe = consumerPipe;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("consumer");
            scope.Add("type", TypeCache<TConsumer>.ShortName);

            _consumerFactory.Probe(scope);

            _consumerPipe.Probe(scope);
        }

        [DebuggerNonUserCode]
        async Task IFilter<ConsumeContext<TMessage>>.Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            StartedActivity? activity = LogContext.Current?.StartConsumerActivity<TConsumer, TMessage>(context);

            var timer = Stopwatch.StartNew();
            try
            {
                await _consumerFactory.Send(context, _consumerPipe).ConfigureAwait(false);

                await context.NotifyConsumed(timer.Elapsed, TypeCache<TConsumer>.ShortName).ConfigureAwait(false);

                await next.Send(context).ConfigureAwait(false);
            }
            catch (OperationCanceledException exception)
            {
                await ThrowHelper.ConsumeFilter.ThrowOperationCancelled<TConsumer, TMessage>(context, timer.Elapsed, exception, activity).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await ThrowHelper.ConsumeFilter.Throw<TConsumer, TMessage>(context, timer.Elapsed, ex, activity).ConfigureAwait(false);
            }
            finally
            {
                activity?.Stop();
            }
        }
    }
}
