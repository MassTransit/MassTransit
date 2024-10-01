namespace MassTransit;

using Contracts.JobService;


public static class JobSagaBusConfigurationExtensions
{
    /// <summary>
    /// Add partition key formatters to support partitioned transports
    /// </summary>
    /// <param name="configurator"></param>
    public static void UseJobSagaPartitionKeyFormatters(this IBusFactoryConfigurator configurator)
    {
        //JobTypeSaga
        configurator.SendTopology.UsePartitionKeyFormatter<AllocateJobSlot>(x => x.Message.JobTypeId.ToString("N"));
        configurator.SendTopology.UsePartitionKeyFormatter<JobSlotReleased>(x => x.Message.JobTypeId.ToString("N"));
        configurator.SendTopology.UsePartitionKeyFormatter<SetConcurrentJobLimit>(x => x.Message.JobTypeId.ToString("N"));

        // JobSaga
        configurator.SendTopology.UsePartitionKeyFormatter<JobSubmitted>(x => x.Message.JobId.ToString("N"));
        configurator.SendTopology.UsePartitionKeyFormatter<JobSlotAllocated>(x => x.Message.JobId.ToString("N"));
        configurator.SendTopology.UsePartitionKeyFormatter<JobSlotUnavailable>(x => x.Message.JobId.ToString("N"));
        configurator.SendTopology.UsePartitionKeyFormatter<Fault<AllocateJobSlot>>(x => x.Message.Message.JobId.ToString("N"));
        configurator.SendTopology.UsePartitionKeyFormatter<Fault<StartJobAttempt>>(x => x.Message.Message.JobId.ToString("N"));
        configurator.SendTopology.UsePartitionKeyFormatter<JobAttemptCanceled>(x => x.Message.JobId.ToString("N"));
        configurator.SendTopology.UsePartitionKeyFormatter<JobAttemptCompleted>(x => x.Message.JobId.ToString("N"));
        configurator.SendTopology.UsePartitionKeyFormatter<JobAttemptFaulted>(x => x.Message.JobId.ToString("N"));
        configurator.SendTopology.UsePartitionKeyFormatter<JobAttemptStarted>(x => x.Message.JobId.ToString("N"));
        configurator.SendTopology.UsePartitionKeyFormatter<JobCompleted>(x => x.Message.JobId.ToString("N"));
        configurator.SendTopology.UsePartitionKeyFormatter<GetJobState>(x => x.Message.JobId.ToString("N"));
        configurator.SendTopology.UsePartitionKeyFormatter<StartJob>(x => x.Message.JobId.ToString("N"));
        configurator.SendTopology.UsePartitionKeyFormatter<CancelJob>(x => x.Message.JobId.ToString("N"));
        configurator.SendTopology.UsePartitionKeyFormatter<RetryJob>(x => x.Message.JobId.ToString("N"));
        configurator.SendTopology.UsePartitionKeyFormatter<RunJob>(x => x.Message.JobId.ToString("N"));
        configurator.SendTopology.UsePartitionKeyFormatter<SaveJobState>(x => x.Message.JobId.ToString("N"));
        configurator.SendTopology.UsePartitionKeyFormatter<SetJobProgress>(x => x.Message.JobId.ToString("N"));
        configurator.SendTopology.UsePartitionKeyFormatter<JobSlotWaitElapsed>(x => x.Message.JobId.ToString("N"));
        configurator.SendTopology.UsePartitionKeyFormatter<JobRetryDelayElapsed>(x => x.Message.JobId.ToString("N"));

        // JobAttemptSaga
        configurator.SendTopology.UsePartitionKeyFormatter<StartJobAttempt>(x => x.Message.JobId.ToString("N"));
        configurator.SendTopology.UsePartitionKeyFormatter<FinalizeJobAttempt>(x => x.Message.JobId.ToString("N"));
        configurator.SendTopology.UsePartitionKeyFormatter<CancelJobAttempt>(x => x.Message.JobId.ToString("N"));
        configurator.SendTopology.UsePartitionKeyFormatter<Fault<StartJob>>(x => x.Message.Message.JobId.ToString("N"));
        configurator.SendTopology.UsePartitionKeyFormatter<JobAttemptStatus>(x => x.Message.JobId.ToString("N"));
        configurator.SendTopology.UsePartitionKeyFormatter<JobStatusCheckRequested>(x => x.Message.AttemptId.ToString("N"));

        // Consumers
        configurator.SendTopology.UsePartitionKeyFormatter<CompleteJob>(x => x.Message.JobId.ToString("N"));
        configurator.SendTopology.UsePartitionKeyFormatter<FaultJob>(x => x.Message.JobId.ToString("N"));
        configurator.SendTopology.UsePartitionKeyFormatter<GetJobAttemptStatus>(x => x.Message.JobId.ToString("N"));
    }

    /// <summary>
    /// Configure the job saga receive endpoints to use the SQL transport partitioned receive mode
    /// </summary>
    /// <param name="configurator"></param>
    /// <returns></returns>
    public static IJobSagaRegistrationConfigurator SetPartitionedReceiveMode(this IJobSagaRegistrationConfigurator configurator)
    {
        return configurator.Endpoints(e =>
        {
            e.AddConfigureEndpointCallback(cfg =>
            {
                if (cfg is ISqlReceiveEndpointConfigurator sql)
                    sql.SetReceiveMode(SqlReceiveMode.Partitioned);
            });
        });
    }
}
