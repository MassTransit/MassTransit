namespace MassTransit.Saga.Pipeline.Filters
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Logging;
    using Metadata;


    /// <summary>
    /// Sends the message through the repository using the specified saga policy.
    /// </summary>
    /// <typeparam name="TSaga">The saga type</typeparam>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class CorrelatedSagaFilter<TSaga, TMessage> :
        IFilter<ConsumeContext<TMessage>>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly IPipe<SagaConsumeContext<TSaga, TMessage>> _messagePipe;
        readonly ISagaPolicy<TSaga, TMessage> _policy;
        readonly ISagaRepository<TSaga> _sagaRepository;

        public CorrelatedSagaFilter(ISagaRepository<TSaga> sagaRepository, ISagaPolicy<TSaga, TMessage> policy,
            IPipe<SagaConsumeContext<TSaga, TMessage>> messagePipe)
        {
            _sagaRepository = sagaRepository;
            _messagePipe = messagePipe;
            _policy = policy;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateFilterScope("saga");
            scope.Set(new
            {
                Correlation = "Id"
            });

            _sagaRepository.Probe(scope);

            _messagePipe.Probe(scope);
        }

        async Task IFilter<ConsumeContext<TMessage>>.Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            Stopwatch timer = Stopwatch.StartNew();

            var activity = LogContext.IfEnabled(OperationName.Saga.Send)?.StartActivity(new
            {
                context.CorrelationId,
                SagaType = TypeMetadataCache<TSaga>.ShortName,
                MessageType = TypeMetadataCache<TMessage>.ShortName
            });

            try
            {
                await Task.Yield();
                await _sagaRepository.Send(context, _policy, _messagePipe).ConfigureAwait(false);

                await next.Send(context).ConfigureAwait(false);

                await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<TSaga>.ShortName).ConfigureAwait(false);
            }
            catch (OperationCanceledException exception)
            {
                await context.NotifyFaulted(timer.Elapsed, TypeMetadataCache<TSaga>.ShortName, exception).ConfigureAwait(false);

                if (exception.CancellationToken == context.CancellationToken)
                    throw;

                throw new ConsumerCanceledException($"The operation was cancelled by the consumer: {TypeMetadataCache<TSaga>.ShortName}");
            }
            catch (Exception ex)
            {
                await context.NotifyFaulted(timer.Elapsed, TypeMetadataCache<TSaga>.ShortName, ex).ConfigureAwait(false);
                throw;
            }
            finally
            {
                activity?.Stop();
            }
        }
    }
}
