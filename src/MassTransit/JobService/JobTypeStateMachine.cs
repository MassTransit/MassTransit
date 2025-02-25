namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Configuration;
    using Contracts.JobService;
    using Internals;
    using JobService;
    using JobService.Messages;
    using Microsoft.Extensions.DependencyInjection;


    public sealed class JobTypeStateMachine :
        MassTransitStateMachine<JobTypeSaga>
    {
        public JobTypeStateMachine()
        {
            Event(() => JobSlotRequested, x =>
            {
                x.CorrelateById(m => m.Message.JobTypeId);
                x.ConfigureConsumeTopology = false;
            });
            Event(() => JobSlotReleased, x =>
            {
                x.CorrelateById(m => m.Message.JobTypeId);
                x.ConfigureConsumeTopology = false;
            });
            Event(() => SetConcurrentJobLimit, x => x.CorrelateById(m => m.Message.JobTypeId));

            InstanceState(x => x.CurrentState, Active, Idle);

            During(Initial, Active, Idle,
                When(JobSlotRequested)
                    .IfElseAsync(context => context.IsSlotAvailable(context.GetPayload<JobSagaSettings>().HeartbeatTimeout),
                        allocate => allocate
                            .TransitionTo(Active),
                        unavailable => unavailable
                            .Respond<JobTypeSaga, AllocateJobSlot, JobSlotUnavailable>(context =>
                                new JobSlotUnavailableResponse { JobId = context.Message.JobId })));

            During(Active,
                When(JobSlotReleased)
                    .If(context => context.Saga.ActiveJobs.Any(x => x.JobId == context.Message.JobId),
                        release => release
                            .Then(context =>
                            {
                                var activeJob = context.Saga.ActiveJobs.FirstOrDefault(x => x.JobId == context.Message.JobId);
                                if (activeJob != null)
                                {
                                    context.Saga.ActiveJobs.Remove(activeJob);
                                    context.Saga.ActiveJobCount--;

                                    LogContext.Debug?.Log("Released Job Slot: {JobId} ({JobCount}): {InstanceAddress}", activeJob.JobId,
                                        context.Saga.ActiveJobCount, activeJob.InstanceAddress);

                                    if (context.Message.Disposition == JobSlotDisposition.Suspect)
                                    {
                                        if (context.Saga.Instances.Remove(activeJob.InstanceAddress))
                                            LogContext.Warning?.Log("Removed Suspect Job Service Instance: {InstanceAddress}", activeJob.InstanceAddress);
                                    }
                                }
                            }))
                    .If(context => context.Saga.ActiveJobCount == 0,
                        empty => empty.TransitionTo(Idle)));

            During(Idle,
                Ignore(JobSlotReleased));

            During(Initial,
                When(SetConcurrentJobLimit)
                    .SetConcurrentLimit()
                    .TransitionTo(Idle));

            During(Active, Idle,
                When(SetConcurrentJobLimit)
                    .SetConcurrentLimit()
            );
        }

        //
        // ReSharper disable UnassignedGetOnlyAutoProperty
        // ReSharper disable MemberCanBePrivate.Global
        public State Active { get; }
        public State Idle { get; }

        public Event<AllocateJobSlot> JobSlotRequested { get; }
        public Event<JobSlotReleased> JobSlotReleased { get; }
        public Event<SetConcurrentJobLimit> SetConcurrentJobLimit { get; }
    }


    static class JobTypeStateMachineBehaviorExtensions
    {
        public static async Task<bool> IsSlotAvailable(this BehaviorContext<JobTypeSaga, AllocateJobSlot> context, TimeSpan heartbeatTimeout)
        {
            if (context.Saga.OverrideLimitExpiration.HasValue)
            {
                if (context.Saga.OverrideLimitExpiration.Value <= DateTime.Now)
                {
                    context.Saga.OverrideLimitExpiration = default;
                    context.Saga.OverrideJobLimit = default;
                }
            }

            var jobId = context.Message.JobId;

            if (context.Saga.ActiveJobs.Any(x => x.JobId == jobId))
                return false;

            var timestamp = DateTime.UtcNow;

            List<KeyValuePair<Uri, JobTypeInstance>> expiredInstances =
                context.Saga.Instances.Where(x => timestamp - x.Value.Updated > heartbeatTimeout).ToList();

            foreach (KeyValuePair<Uri, JobTypeInstance> instance in expiredInstances)
                context.Saga.Instances.Remove(instance.Key);

            if (context.Saga.GlobalConcurrentJobLimit.HasValue && context.Saga.ActiveJobCount >= context.Saga.GlobalConcurrentJobLimit)
                return false;

            var strategy = context.GetJobDistributionStrategyOrUseDefault();

            var activeJob = await strategy.IsJobSlotAvailable(context, context.Saga).ConfigureAwait(false);
            if (activeJob == null)
                return false;

            var activeInstance = context.Saga.Instances.TryGetValue(activeJob.InstanceAddress, out var value) ? value : null;
            if (activeInstance == null)
            {
                LogContext.Warning?.Log("Job Distribution Strategy returned unknown instance address: {InstanceAddress}", activeJob.InstanceAddress);
                return false;
            }

            activeInstance.Used = timestamp;

            activeJob.Deadline = timestamp + context.Message.JobTimeout;
            activeJob.Properties = context.Message.JobProperties;

            context.Saga.ActiveJobCount++;
            context.Saga.ActiveJobs.Add(activeJob);

            LogContext.Debug?.Log("Allocated Job Slot: {JobId} ({JobCount}): {InstanceAddress} ({InstanceCount})", jobId, context.Saga.ActiveJobCount,
                activeJob.InstanceAddress, context.Saga.ActiveJobs.Count(x => x.InstanceAddress == activeJob.InstanceAddress));

            await context.RespondAsync<JobSlotAllocated>(new JobSlotAllocatedResponse
            {
                JobId = jobId,
                InstanceAddress = activeJob.InstanceAddress,
            });

            return true;
        }

        static IJobDistributionStrategy GetJobDistributionStrategyOrUseDefault(this ConsumeContext context)
        {
            IJobDistributionStrategy strategy = null;

            if (context.TryGetPayload(out IServiceScope serviceScope))
                strategy = serviceScope.ServiceProvider.GetService<IJobDistributionStrategy>();
            else if (context.TryGetPayload(out IServiceProvider serviceProvider))
                strategy = serviceProvider.GetService<IJobDistributionStrategy>();

            return strategy ?? DefaultJobDistributionStrategy.Instance;
        }

        public static EventActivityBinder<JobTypeSaga, SetConcurrentJobLimit> SetConcurrentLimit(
            this EventActivityBinder<JobTypeSaga, SetConcurrentJobLimit> binder)
        {
            return binder.Then(context =>
            {
                var instanceAddress = context.Message.InstanceAddress;
                if (instanceAddress != null)
                {
                    DateTime? instanceUpdated = context.SentTime;

                    if (context.Saga.Instances.TryGetValue(instanceAddress, out var instance))
                    {
                        if (context.Message.Kind == ConcurrentLimitKind.Stopped)
                        {
                            LogContext.Debug?.Log("Job Service Instance Stopped: {InstanceAddress}", instanceAddress);

                            context.Saga.Instances.Remove(instanceAddress);
                        }
                        else if (instanceUpdated > instance.Updated)
                            instance.Updated = instanceUpdated;
                    }
                    else
                    {
                        if (context.Message.Kind != ConcurrentLimitKind.Stopped)
                        {
                            instance = new JobTypeInstance { Updated = instanceUpdated };

                            context.Saga.Instances.Add(instanceAddress, instance);

                            LogContext.Debug?.Log("Job Service Instance Started: {InstanceAddress}", instanceAddress);
                        }
                    }

                    if (context.Message.Kind != ConcurrentLimitKind.Stopped)
                    {
                        if (context.Message.InstanceProperties is { Count: > 0 })
                        {
                            instance.Properties ??= new Dictionary<string, object>(context.Message.JobTypeProperties.Count, StringComparer.OrdinalIgnoreCase);
                            instance.Properties.SetValues(context.Message.InstanceProperties);
                        }
                    }
                }

                if (context.Message.Kind == ConcurrentLimitKind.Configured)
                {
                    context.Saga.ConcurrentJobLimit = context.Message.ConcurrentJobLimit;
                    context.Saga.GlobalConcurrentJobLimit = context.Message.GlobalConcurrentJobLimit;
                    context.Saga.Name = context.Message.JobTypeName;

                    if (context.Message.JobTypeProperties is { Count: > 0 })
                    {
                        context.Saga.Properties ??= new Dictionary<string, object>(context.Message.JobTypeProperties.Count, StringComparer.OrdinalIgnoreCase);

                        context.Saga.Properties.SetValues(context.Message.JobTypeProperties);
                    }

                    LogContext.Debug?.Log("Concurrent Job Limit: {ConcurrencyLimit} {JobTypeName}", context.Saga.ConcurrentJobLimit,
                        context.Message.JobTypeName);
                }
                else if (context.Message.Kind == ConcurrentLimitKind.Override)
                {
                    context.Saga.OverrideJobLimit = context.Message.ConcurrentJobLimit;
                    context.Saga.OverrideLimitExpiration = DateTime.Now + (context.Message.Duration ?? TimeSpan.FromMinutes(30));

                    LogContext.Debug?.Log("Override Concurrent Job Limit: {ConcurrencyLimit}", context.Saga.OverrideJobLimit);
                }
            });
        }
    }
}
