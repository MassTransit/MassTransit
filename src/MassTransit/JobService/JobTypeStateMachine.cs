namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Contracts.JobService;


    public sealed class JobTypeStateMachine :
        MassTransitStateMachine<JobTypeSaga>
    {
        public JobTypeStateMachine(JobServiceOptions options)
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
                    .IfElse(context => context.IsSlotAvailable(options.HeartbeatTimeout),
                        allocate => allocate
                            .TransitionTo(Active),
                        unavailable => unavailable
                            .RespondAsync(context => context.Init<JobSlotUnavailable>(new { context.Message.JobId }))));

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
        public static bool IsSlotAvailable(this BehaviorContext<JobTypeSaga, AllocateJobSlot> context, TimeSpan heartbeatTimeout)
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

            var concurrentJobLimit = context.Saga.OverrideJobLimit ?? context.Saga.ConcurrentJobLimit;

            var instances = from i in context.Saga.Instances
                join a in context.Saga.ActiveJobs on i.Key equals a.InstanceAddress into ai
                where ai.Count() < concurrentJobLimit
                orderby ai.Count(), i.Value.Used
                select new
                {
                    InstanceAddress = i.Key,
                    InstanceCount = ai.Count()
                };

            var nextInstance = instances.FirstOrDefault();
            if (nextInstance == null)
                return false;

            context.Saga.ActiveJobCount++;
            context.Saga.ActiveJobs.Add(new ActiveJob
            {
                JobId = jobId,
                Deadline = timestamp + context.Message.JobTimeout,
                InstanceAddress = nextInstance.InstanceAddress
            });

            context.Saga.Instances[nextInstance.InstanceAddress].Used = timestamp;

            LogContext.Debug?.Log("Allocated Job Slot: {JobId} ({JobCount}): {InstanceAddress} ({InstanceCount})", jobId, context.Saga.ActiveJobCount,
                nextInstance.InstanceAddress, nextInstance.InstanceCount + 1);

            context.RespondAsync<JobSlotAllocated>(new
            {
                jobId,
                nextInstance.InstanceAddress,
            });

            return true;
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
                            context.Saga.Instances.Add(instanceAddress, new JobTypeInstance { Updated = instanceUpdated });

                            LogContext.Debug?.Log("Job Service Instance Started: {InstanceAddress}", instanceAddress);
                        }
                    }
                }

                if (context.Message.Kind == ConcurrentLimitKind.Configured)
                {
                    context.Saga.ConcurrentJobLimit = context.Message.ConcurrentJobLimit;

                    LogContext.Debug?.Log("Concurrent Job Limit: {ConcurrencyLimit}", context.Saga.ConcurrentJobLimit);
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
