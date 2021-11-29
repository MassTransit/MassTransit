namespace MassTransit.Middleware
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;


    /// <summary>
    /// Consumes a message via a message handler and reports the message as consumed or faulted
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class ObserverMessageFilter<TMessage> :
        IFilter<ConsumeContext<TMessage>>
        where TMessage : class
    {
        readonly IObserver<ConsumeContext<TMessage>> _observer;
        readonly string _observerType;

        public ObserverMessageFilter(IObserver<ConsumeContext<TMessage>> observer)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            _observer = observer;
            _observerType = TypeCache.GetShortName(observer.GetType());
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("observer");
            scope.Add("observerType", _observerType);
        }

        [DebuggerNonUserCode]
        async Task IFilter<ConsumeContext<TMessage>>.Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            var timer = Stopwatch.StartNew();
            try
            {
                await Task.Yield();

                _observer.OnNext(context);

                await context.NotifyConsumed(timer.Elapsed, _observerType).ConfigureAwait(false);

                await next.Send(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await context.NotifyFaulted(timer.Elapsed, _observerType, ex).ConfigureAwait(false);

                _observer.OnError(ex);

                throw;
            }
        }
    }
}
