namespace MassTransit.Middleware
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Observables;


    /// <summary>
    /// Uses a retry policy to handle exceptions, retrying the operation in according
    /// with the policy
    /// </summary>
    public class RetryFilter<TContext> :
        IFilter<TContext>
        where TContext : class, PipeContext
    {
        readonly RetryObservable _observers;
        readonly IRetryPolicy _retryPolicy;

        public RetryFilter(IRetryPolicy retryPolicy, RetryObservable observers)
        {
            _retryPolicy = retryPolicy;
            _observers = observers;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("retry");

            _retryPolicy.Probe(scope);
        }

        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        async Task IFilter<TContext>.Send(TContext context, IPipe<TContext> next)
        {
            RetryPolicyContext<TContext> policyContext = _retryPolicy.CreatePolicyContext(context);
            try
            {
                if (_observers.Count > 0)
                {
                    var postCreateTask = _observers.PostCreate(policyContext);
                    if (postCreateTask.Status != TaskStatus.RanToCompletion)
                        await postCreateTask.ConfigureAwait(false);
                }

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

                if (policyContext.Context.TryGetPayload(out RetryContext<TContext> payloadRetryContext))
                {
                    if (_retryPolicy.IsHandled(exception))
                    {
                        var retryFaultedTask = policyContext.RetryFaulted(exception);
                        if (retryFaultedTask.Status != TaskStatus.RanToCompletion)
                            await retryFaultedTask.ConfigureAwait(false);

                        if (_observers.Count > 0)
                        {
                            var retryFaultTask = _observers.RetryFault(payloadRetryContext);
                            if (retryFaultTask.Status != TaskStatus.RanToCompletion)
                                await retryFaultTask.ConfigureAwait(false);
                        }
                    }

                    context.GetOrAddPayload(() => payloadRetryContext);

                    throw;
                }

                if (policyContext.Context.TryGetPayload(out RetryContext genericRetryContext))
                {
                    if (_retryPolicy.IsHandled(exception))
                    {
                        var retryFaultedTask = policyContext.RetryFaulted(exception);
                        if (retryFaultedTask.Status != TaskStatus.RanToCompletion)
                            await retryFaultedTask.ConfigureAwait(false);

                        if (_observers.Count > 0)
                        {
                            var retryFaultTask = _observers.RetryFault(genericRetryContext);
                            if (retryFaultTask.Status != TaskStatus.RanToCompletion)
                                await retryFaultTask.ConfigureAwait(false);
                        }
                    }

                    context.GetOrAddPayload(() => genericRetryContext);

                    throw;
                }

                if (!policyContext.CanRetry(exception, out RetryContext<TContext> retryContext))
                {
                    if (_retryPolicy.IsHandled(exception))
                    {
                        var retryFaultedTask = retryContext.RetryFaulted(exception);
                        if (retryFaultedTask.Status != TaskStatus.RanToCompletion)
                            await retryFaultedTask.ConfigureAwait(false);

                        if (_observers.Count > 0)
                        {
                            var retryFaultTask = _observers.RetryFault(retryContext);
                            if (retryFaultTask.Status != TaskStatus.RanToCompletion)
                                await retryFaultTask.ConfigureAwait(false);
                        }

                        context.GetOrAddPayload(() => retryContext);
                    }

                    throw;
                }

                if (_observers.Count > 0)
                {
                    var postFaultTask = _observers.PostFault(retryContext);
                    if (postFaultTask.Status != TaskStatus.RanToCompletion)
                        await postFaultTask.ConfigureAwait(false);
                }

                await Attempt(context, retryContext, next).ConfigureAwait(false);
            }
            finally
            {
                policyContext.Dispose();
            }
        }

        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        async Task Attempt(TContext context, RetryContext<TContext> retryContext, IPipe<TContext> next)
        {
            while (!retryContext.CancellationToken.IsCancellationRequested)
            {
                if (retryContext.Delay.HasValue)
                    await Task.Delay(retryContext.Delay.Value, retryContext.CancellationToken).ConfigureAwait(false);

                var preRetryContextTask = retryContext.PreRetry();
                if (preRetryContextTask.Status != TaskStatus.RanToCompletion)
                    await preRetryContextTask.ConfigureAwait(false);

                if (_observers.Count > 0)
                {
                    var preRetryTask = _observers.PreRetry(retryContext);
                    if (preRetryTask.Status != TaskStatus.RanToCompletion)
                        await preRetryTask.ConfigureAwait(false);
                }

                try
                {
                    await next.Send(retryContext.Context).ConfigureAwait(false);

                    if (_observers.Count > 0)
                    {
                        var retryCompleteTask = _observers.RetryComplete(retryContext);
                        if (retryCompleteTask.Status != TaskStatus.RanToCompletion)
                            await retryCompleteTask.ConfigureAwait(false);
                    }

                    return;
                }
                catch (OperationCanceledException exception)
                    when (exception.CancellationToken == retryContext.CancellationToken || exception.CancellationToken == context.CancellationToken)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();

                    if (retryContext.Context.TryGetPayload(out RetryContext<TContext> payloadRetryContext))
                    {
                        if (_retryPolicy.IsHandled(exception))
                        {
                            var retryFaultedTask = retryContext.RetryFaulted(exception);
                            if (retryFaultedTask.Status != TaskStatus.RanToCompletion)
                                await retryFaultedTask.ConfigureAwait(false);

                            if (_observers.Count > 0)
                            {
                                var retryFaultTask = _observers.RetryFault(payloadRetryContext);
                                if (retryFaultTask.Status != TaskStatus.RanToCompletion)
                                    await retryFaultTask.ConfigureAwait(false);
                            }
                        }

                        context.GetOrAddPayload(() => payloadRetryContext);

                        throw;
                    }

                    if (retryContext.Context.TryGetPayload(out RetryContext genericRetryContext))
                    {
                        if (_retryPolicy.IsHandled(exception))
                        {
                            var retryFaultedTask = retryContext.RetryFaulted(exception);
                            if (retryFaultedTask.Status != TaskStatus.RanToCompletion)
                                await retryFaultedTask.ConfigureAwait(false);

                            if (_observers.Count > 0)
                            {
                                var retryFaultTask = _observers.RetryFault(genericRetryContext);
                                if (retryFaultTask.Status != TaskStatus.RanToCompletion)
                                    await retryFaultTask.ConfigureAwait(false);
                            }
                        }

                        context.GetOrAddPayload(() => genericRetryContext);

                        throw;
                    }

                    if (!retryContext.CanRetry(exception, out RetryContext<TContext> nextRetryContext))
                    {
                        if (_retryPolicy.IsHandled(exception))
                        {
                            var retryFaultedTask = nextRetryContext.RetryFaulted(exception);
                            if (retryFaultedTask.Status != TaskStatus.RanToCompletion)
                                await retryFaultedTask.ConfigureAwait(false);

                            if (_observers.Count > 0)
                            {
                                var retryFaultTask = _observers.RetryFault(nextRetryContext);
                                if (retryFaultTask.Status != TaskStatus.RanToCompletion)
                                    await retryFaultTask.ConfigureAwait(false);
                            }

                            context.GetOrAddPayload(() => nextRetryContext);
                        }

                        throw;
                    }

                    if (_observers.Count > 0)
                    {
                        var postFaultTask = _observers.PostFault(nextRetryContext);
                        if (postFaultTask.Status != TaskStatus.RanToCompletion)
                            await postFaultTask.ConfigureAwait(false);
                    }

                    retryContext = nextRetryContext;
                }
            }

            context.CancellationToken.ThrowIfCancellationRequested();
        }
    }
}
