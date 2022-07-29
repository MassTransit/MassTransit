namespace MassTransit.Middleware
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Context;
    using Logging;
    using Util;


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
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            if (instancePipe == null)
                throw new ArgumentNullException(nameof(instancePipe));

            _instance = instance;
            _instancePipe = instancePipe;
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
            StartedActivity? activity = LogContext.Current?.StartConsumerActivity<TConsumer, TMessage>(context);

            var timer = Stopwatch.StartNew();
            try
            {
                await _instancePipe.Send(new ConsumerConsumeContextScope<TConsumer, TMessage>(context, _instance)).ConfigureAwait(false);

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
