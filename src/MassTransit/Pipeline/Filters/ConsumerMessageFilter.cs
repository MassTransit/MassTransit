namespace MassTransit.Pipeline.Filters
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Logging;
    using Metadata;


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
            scope.Add("type", TypeMetadataCache<TConsumer>.ShortName);

            _consumerFactory.Probe(scope);

            _consumerPipe.Probe(scope);
        }

        [DebuggerNonUserCode]
        async Task IFilter<ConsumeContext<TMessage>>.Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            var activity = LogContext.IfEnabled(OperationName.Consumer.Consume)?.StartConsumerActivity<TConsumer, TMessage>(context);

            var timer = Stopwatch.StartNew();
            try
            {
                await Task.Yield();

                await _consumerFactory.Send(context, _consumerPipe).ConfigureAwait(false);

                await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<TConsumer>.ShortName).ConfigureAwait(false);

                await next.Send(context).ConfigureAwait(false);
            }
            catch (OperationCanceledException exception)
            {
                await context.NotifyFaulted(timer.Elapsed, TypeMetadataCache<TConsumer>.ShortName, exception).ConfigureAwait(false);

                if (exception.CancellationToken == context.CancellationToken)
                    throw;

                throw new ConsumerCanceledException($"The operation was cancelled by the consumer: {TypeMetadataCache<TConsumer>.ShortName}");
            }
            catch (Exception ex)
            {
                await context.NotifyFaulted(timer.Elapsed, TypeMetadataCache<TConsumer>.ShortName, ex).ConfigureAwait(false);
                throw;
            }
            finally
            {
                activity?.Stop();
            }
        }
    }
}
