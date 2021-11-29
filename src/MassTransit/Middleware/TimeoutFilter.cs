namespace MassTransit.Middleware
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public class TimeoutFilter<TContext, TResult> :
        IFilter<TContext>
        where TContext : class, ConsumeContext
        where TResult : TContext
    {
        readonly Func<TContext, CancellationToken, TResult> _contextFactory;
        readonly TimeSpan _timeout;

        public TimeoutFilter(Func<TContext, CancellationToken, TResult> contextFactory, TimeSpan timeout)
        {
            _contextFactory = contextFactory;
            _timeout = timeout;
        }

        public async Task Send(TContext context, IPipe<TContext> next)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken);
            try
            {
                cts.CancelAfter(_timeout);

                var timeoutContext = _contextFactory(context, cts.Token);

                await next.Send(timeoutContext).ConfigureAwait(false);

                await timeoutContext.ConsumeCompleted.ConfigureAwait(false);
            }
            catch (OperationCanceledException ex) when (ex.CancellationToken == cts.Token)
            {
                throw new ConsumerCanceledException("The operation was canceled by the timeout filter");
            }
            finally
            {
                cts.Dispose();
            }
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("timeout");
            scope.Add("timeout", _timeout);
        }
    }
}
