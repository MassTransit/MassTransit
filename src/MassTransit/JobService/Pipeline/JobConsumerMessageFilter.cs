namespace MassTransit.JobService.Pipeline
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Pipeline;
    using Metadata;


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
        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("consume");
            scope.Add("method", $"Consume(ConsumeContext<{TypeMetadataCache<TJob>.ShortName}> context)");
        }

        Task IFilter<ConsumerConsumeContext<TConsumer, TJob>>.Send(ConsumerConsumeContext<TConsumer, TJob> context,
            IPipe<ConsumerConsumeContext<TConsumer, TJob>> next)
        {
            if (context.Consumer is IJobConsumer<TJob> messageConsumer)
                return RunJob(context, messageConsumer);

            var message = $"Consumer type {TypeMetadataCache<TConsumer>.ShortName} is not a consumer of job type {TypeMetadataCache<TJob>.ShortName}";

            throw new ConsumerMessageException(message);
        }

        static async Task RunJob(PipeContext context, IJobConsumer<TJob> jobConsumer)
        {
            var jobContext = context.GetPayload<JobContext<TJob>>();

            try
            {
                await jobContext.NotifyStarted().ConfigureAwait(false);

                await jobConsumer.Run(jobContext).ConfigureAwait(false);

                await jobContext.NotifyCompleted().ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                await jobContext.NotifyCanceled("Task canceled").ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                await jobContext.NotifyCanceled("Operation canceled").ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                await jobContext.NotifyFaulted(exception).ConfigureAwait(false);
            }
        }
    }
}
