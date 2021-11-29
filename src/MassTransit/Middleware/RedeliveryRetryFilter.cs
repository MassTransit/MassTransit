namespace MassTransit.Middleware
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Observables;


    /// <summary>
    /// Uses the message redelivery mechanism, if available, to delay a retry without blocking message delivery
    /// </summary>
    /// <typeparam name="TContext">The context type</typeparam>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class RedeliveryRetryFilter<TContext, TMessage> :
        IFilter<TContext>
        where TContext : class, ConsumeContext<TMessage>
        where TMessage : class
    {
        readonly RetryObservable _observers;
        readonly IRetryPolicy _retryPolicy;

        public RedeliveryRetryFilter(IRetryPolicy retryPolicy, RetryObservable observers)
        {
            _retryPolicy = retryPolicy;
            _observers = observers;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("retry");
            scope.Add("type", "redelivery");

            _retryPolicy.Probe(scope);
        }

        [DebuggerNonUserCode]
        public async Task Send(TContext context, IPipe<TContext> next)
        {
            using (RetryPolicyContext<TContext> policyContext = _retryPolicy.CreatePolicyContext(context))
            {
                if (_observers.Count > 0)
                    await _observers.PostCreate(policyContext).ConfigureAwait(false);

                try
                {
                    await next.Send(policyContext.Context).ConfigureAwait(false);
                }
                catch (OperationCanceledException exception)
                    when (exception.CancellationToken == context.CancellationToken || exception.CancellationToken == policyContext.Context.CancellationToken)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    if (policyContext.Context.CancellationToken.IsCancellationRequested)
                        policyContext.Context.CancellationToken.ThrowIfCancellationRequested();

                    if (!policyContext.CanRetry(exception, out RetryContext<TContext> retryContext))
                    {
                        if (_retryPolicy.IsHandled(exception))
                        {
                            context.GetOrAddPayload(() => retryContext);

                            await retryContext.RetryFaulted(exception).ConfigureAwait(false);

                            if (_observers.Count > 0)
                                await _observers.RetryFault(retryContext).ConfigureAwait(false);
                        }

                        throw;
                    }

                    var previousDeliveryCount = context.GetRedeliveryCount();
                    for (var retryIndex = 0; retryIndex < previousDeliveryCount; retryIndex++)
                    {
                        if (!retryContext.CanRetry(exception, out retryContext))
                        {
                            if (_retryPolicy.IsHandled(exception))
                            {
                                context.GetOrAddPayload(() => retryContext);

                                await retryContext.RetryFaulted(exception).ConfigureAwait(false);

                                if (_observers.Count > 0)
                                    await _observers.RetryFault(retryContext).ConfigureAwait(false);
                            }

                            throw;
                        }
                    }

                    if (_observers.Count > 0)
                        await _observers.PostFault(retryContext).ConfigureAwait(false);

                    try
                    {
                        var redeliveryContext = context.GetPayload<MessageRedeliveryContext>();

                        var delay = retryContext.Delay ?? TimeSpan.Zero;

                        await redeliveryContext.ScheduleRedelivery(delay).ConfigureAwait(false);

                        await context.NotifyConsumed(context, context.ReceiveContext.ElapsedTime,
                            TypeCache<RedeliveryRetryFilter<TContext, TMessage>>.ShortName).ConfigureAwait(false);
                    }
                    catch (Exception redeliveryException)
                    {
                        throw new TransportException(context.ReceiveContext.InputAddress, "The message delivery could not be rescheduled",
                            new AggregateException(redeliveryException, exception));
                    }
                }
            }
        }
    }
}
