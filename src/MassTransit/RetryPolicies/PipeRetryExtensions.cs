namespace MassTransit.RetryPolicies
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Internals;
    using Middleware;


    public static class PipeRetryExtensions
    {
        public static async Task Retry(this IRetryPolicy retryPolicy, Func<Task> retryMethod, CancellationToken cancellationToken = default)
        {
            var inlinePipeContext = new InlinePipeContext(cancellationToken);

            RetryPolicyContext<InlinePipeContext> policyContext = retryPolicy.CreatePolicyContext(inlinePipeContext);
            try
            {
                await retryMethod().ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                if (!policyContext.CanRetry(exception, out RetryContext<InlinePipeContext> retryContext))
                    throw;

                try
                {
                    await Attempt(inlinePipeContext, retryContext, retryMethod).ConfigureAwait(false);

                    return;
                }
                catch (OperationCanceledException ex) when (ex.CancellationToken == cancellationToken)
                {
                }

                throw;
            }
            finally
            {
                policyContext.Dispose();
            }
        }

        public static async Task<T> Retry<T>(this IRetryPolicy retryPolicy, Func<Task<T>> retryMethod, CancellationToken cancellationToken = default)
        {
            var inlinePipeContext = new InlinePipeContext(cancellationToken);

            RetryPolicyContext<InlinePipeContext> policyContext = retryPolicy.CreatePolicyContext(inlinePipeContext);
            try
            {
                return await retryMethod().ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                if (!policyContext.CanRetry(exception, out RetryContext<InlinePipeContext> retryContext))
                    throw;

                try
                {
                    return await Attempt(inlinePipeContext, retryContext, retryMethod).ConfigureAwait(false);
                }
                catch (OperationCanceledException ex) when (ex.CancellationToken == cancellationToken)
                {
                }

                throw;
            }
            finally
            {
                policyContext.Dispose();
            }
        }

        static async Task Attempt<T>(T context, RetryContext<T> retryContext, Func<Task> retryMethod)
            where T : class, PipeContext
        {
            while (context.CancellationToken.IsCancellationRequested == false)
            {
                LogContext.Warning?.Log(retryContext.Exception, "Retrying {Delay}: {Message}", retryContext.Delay, retryContext.Exception.Message);

                try
                {
                    if (retryContext.Delay.HasValue)
                        await Task.Delay(retryContext.Delay.Value, context.CancellationToken).ConfigureAwait(false);

                    if (!context.CancellationToken.IsCancellationRequested)
                    {
                        await retryMethod().ConfigureAwait(false);

                        return;
                    }
                }
                catch (OperationCanceledException exception) when (exception.CancellationToken == context.CancellationToken)
                {
                    retryContext.Exception?.Rethrow();
                    throw;
                }
                catch (Exception exception)
                {
                    if (!retryContext.CanRetry(exception, out RetryContext<T> nextRetryContext))
                        throw new OperationCanceledException(context.CancellationToken);

                    retryContext = nextRetryContext;
                }
            }

            throw new OperationCanceledException(context.CancellationToken);
        }

        static async Task<TResult> Attempt<T, TResult>(T context, RetryContext<T> retryContext, Func<Task<TResult>> retryMethod)
            where T : class, PipeContext
        {
            while (context.CancellationToken.IsCancellationRequested == false)
            {
                LogContext.Warning?.Log(retryContext.Exception, "Retrying {Delay}: {Message}", retryContext.Delay, retryContext.Exception.Message);

                try
                {
                    if (retryContext.Delay.HasValue)
                        await Task.Delay(retryContext.Delay.Value, context.CancellationToken).ConfigureAwait(false);

                    if (!context.CancellationToken.IsCancellationRequested)
                        return await retryMethod().ConfigureAwait(false);
                }
                catch (OperationCanceledException exception) when (exception.CancellationToken == context.CancellationToken)
                {
                    retryContext.Exception?.Rethrow();
                    throw;
                }
                catch (Exception exception)
                {
                    if (!retryContext.CanRetry(exception, out RetryContext<T> nextRetryContext))
                        throw new OperationCanceledException(context.CancellationToken);

                    retryContext = nextRetryContext;
                }
            }

            throw new OperationCanceledException(context.CancellationToken);
        }


        class InlinePipeContext :
            BasePipeContext
        {
            public InlinePipeContext(CancellationToken cancellationToken)
                : base(cancellationToken)
            {
            }
        }
    }
}
