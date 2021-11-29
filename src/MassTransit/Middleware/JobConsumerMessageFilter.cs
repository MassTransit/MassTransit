namespace MassTransit.Middleware
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// Converts the ConsumeContext to a JobContext, and executes the job
    /// </summary>
    /// <typeparam name="TConsumer">The consumer type</typeparam>
    /// <typeparam name="TJob">The message type</typeparam>
    public class JobConsumerMessageFilter<TConsumer, TJob> :
        IConsumerMessageFilter<TConsumer, TJob>
        where TConsumer : class, IJobConsumer<TJob>
        where TJob : class
    {
        readonly IRetryPolicy _retryPolicy;

        public JobConsumerMessageFilter(IRetryPolicy retryPolicy)
        {
            _retryPolicy = retryPolicy;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("consume");
            scope.Add("method", $"Consume(ConsumeContext<{TypeCache<TJob>.ShortName}> context)");
        }

        Task IFilter<ConsumerConsumeContext<TConsumer, TJob>>.Send(ConsumerConsumeContext<TConsumer, TJob> context,
            IPipe<ConsumerConsumeContext<TConsumer, TJob>> next)
        {
            if (context.Consumer is IJobConsumer<TJob> messageConsumer)
                return RunJob(context, messageConsumer);

            var message = $"Consumer type {TypeCache<TConsumer>.ShortName} is not a consumer of job type {TypeCache<TJob>.ShortName}";

            throw new ConsumerMessageException(message);
        }

        async Task RunJob(PipeContext context, IJobConsumer<TJob> jobConsumer)
        {
            var jobContext = context.GetPayload<JobContext<TJob>>();

            RetryPolicyContext<JobContext<TJob>> policyContext = _retryPolicy.CreatePolicyContext(jobContext);

            try
            {
                await jobContext.NotifyStarted().ConfigureAwait(false);

                await jobConsumer.Run(jobContext).ConfigureAwait(false);

                await jobContext.NotifyCompleted().ConfigureAwait(false);
            }
            catch (OperationCanceledException exception)
            {
                if (jobContext.CancellationToken == exception.CancellationToken)
                    await jobContext.NotifyCanceled("Operation canceled").ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                if (!policyContext.CanRetry(exception, out RetryContext<JobContext<TJob>> retryContext))
                {
                    if (_retryPolicy.IsHandled(exception))
                    {
                        context.GetOrAddPayload(() => retryContext);

                        await retryContext.RetryFaulted(exception).ConfigureAwait(false);
                    }

                    await jobContext.NotifyFaulted(exception).ConfigureAwait(false);
                    return;
                }

                var currentRetryAttempt = jobContext.RetryAttempt;
                for (var retryIndex = 0; retryIndex < currentRetryAttempt; retryIndex++)
                {
                    if (!retryContext.CanRetry(exception, out retryContext))
                    {
                        if (_retryPolicy.IsHandled(exception))
                        {
                            context.GetOrAddPayload(() => retryContext);

                            await retryContext.RetryFaulted(exception).ConfigureAwait(false);
                        }

                        await jobContext.NotifyFaulted(exception).ConfigureAwait(false);
                        return;
                    }
                }

                var delay = retryContext.Delay ?? TimeSpan.Zero;

                await jobContext.NotifyFaulted(exception, delay).ConfigureAwait(false);
            }
            finally
            {
                policyContext.Dispose();
            }
        }
    }
}
