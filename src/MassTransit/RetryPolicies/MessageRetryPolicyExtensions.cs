namespace MassTransit.RetryPolicies
{
    using System;
    using System.Threading.Tasks;


    public static class MessageRetryPolicyExtensions
    {
        public static async Task Retry<T>(this IRetryPolicy retryPolicy, ConsumeContext<T> context, Func<ConsumeContext<T>, Task> retryMethod)
            where T : class
        {
            using RetryPolicyContext<ConsumeContext<T>> policyContext = retryPolicy.CreatePolicyContext(context);

            try
            {
                await retryMethod(policyContext.Context).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                if (context.TryGetPayload(out RetryContext<ConsumeContext<T>> retryContext))
                    throw;

                if (!policyContext.CanRetry(exception, out retryContext))
                {
                    context.GetOrAddPayload(() => retryContext);
                    throw;
                }

                await Attempt(retryContext, retryMethod).ConfigureAwait(false);
            }
        }

        static async Task Attempt<T>(RetryContext<ConsumeContext<T>> retryContext, Func<ConsumeContext<T>, Task> retryMethod)
            where T : class
        {
            try
            {
                await retryMethod(retryContext.Context).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                if (retryContext.Context.TryGetPayload(out RetryContext<ConsumeContext<T>> nextRetryContext))
                    throw;

                if (!retryContext.CanRetry(exception, out nextRetryContext))
                {
                    retryContext.Context.GetOrAddPayload(() => nextRetryContext);
                    throw;
                }

                if (nextRetryContext.Delay.HasValue)
                    await Task.Delay(nextRetryContext.Delay.Value).ConfigureAwait(false);

                await Attempt(nextRetryContext, retryMethod).ConfigureAwait(false);
            }
        }
    }
}
