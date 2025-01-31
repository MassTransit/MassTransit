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
                {
                    var postCreateTask = _observers.PostCreate(policyContext);
                    if (postCreateTask.Status != TaskStatus.RanToCompletion)
                        await postCreateTask.ConfigureAwait(false);
                }

                try
                {
                    await next.Send(policyContext.Context).ConfigureAwait(false);
                }
                catch (OperationCanceledException exception)
                    when (exception.CancellationToken == policyContext.Context.CancellationToken || exception.CancellationToken == context.CancellationToken)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    policyContext.Context.CancellationToken.ThrowIfCancellationRequested();

                    if (!policyContext.CanRetry(exception, out RetryContext<TContext> retryContext))
                    {
                        if (_retryPolicy.IsHandled(exception))
                        {
                            context.GetOrAddPayload(() => retryContext);

                            var retryFaultedTask = retryContext.RetryFaulted(exception);
                            if (retryFaultedTask.Status != TaskStatus.RanToCompletion)
                                await retryFaultedTask.ConfigureAwait(false);

                            if (_observers.Count > 0)
                            {
                                var retryFaultTask = _observers.RetryFault(retryContext);
                                if (retryFaultTask.Status != TaskStatus.RanToCompletion)
                                    await retryFaultTask.ConfigureAwait(false);
                            }
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

                                var retryFaultedTask = retryContext.RetryFaulted(exception);
                                if (retryFaultedTask.Status != TaskStatus.RanToCompletion)
                                    await retryFaultedTask.ConfigureAwait(false);

                                if (_observers.Count > 0)
                                {
                                    var retryFaultTask = _observers.RetryFault(retryContext);
                                    if (retryFaultTask.Status != TaskStatus.RanToCompletion)
                                        await retryFaultTask.ConfigureAwait(false);
                                }
                            }

                            throw;
                        }
                    }

                    if (_observers.Count > 0)
                    {
                        var postFaultTask = _observers.PostFault(retryContext);
                        if (postFaultTask.Status != TaskStatus.RanToCompletion)
                            await postFaultTask.ConfigureAwait(false);
                    }

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
