namespace MassTransit.JobService.Components.StateMachines
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Automatonymous;
    using Automatonymous.Binders;
    using Context;
    using Contracts.JobService;
    using Topology.Topologies;


    public sealed class JobTypeStateMachine :
        MassTransitStateMachine<JobTypeSaga>
    {
        static JobTypeStateMachine()
        {
            GlobalTopology.Send.UseCorrelationId<AllocateJobSlot>(x => x.JobTypeId);
            GlobalTopology.Send.UseCorrelationId<JobSlotReleased>(x => x.JobTypeId);
            GlobalTopology.Send.UseCorrelationId<SetConcurrentJobLimit>(x => x.JobTypeId);
        }

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
                            .RespondAsync(context => context.Init<JobSlotUnavailable>(new {context.Data.JobId}))));

            During(Active,
                When(JobSlotReleased)
                    .If(context => context.Instance.ActiveJobs.Any(x => x.JobId == context.Data.JobId),
                        release => release
                            .Then(context =>
                            {
                                var activeJob = context.Instance.ActiveJobs.FirstOrDefault(x => x.JobId == context.Data.JobId);
                                if (activeJob != null)
                                {
                                    context.Instance.ActiveJobs.Remove(activeJob);
                                    context.Instance.ActiveJobCount--;

                                    LogContext.Debug?.Log("Released Job Slot: {JobId} ({JobCount}): {InstanceAddress}", activeJob.JobId,
                                        context.Instance.ActiveJobCount, activeJob.InstanceAddress);
                                }
                            }))
                    .If(context => context.Instance.ActiveJobCount == 0,
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
            if (context.Instance.OverrideLimitExpiration.HasValue)
            {
                if (context.Instance.OverrideLimitExpiration.Value <= DateTime.Now)
                {
                    context.Instance.OverrideLimitExpiration = default;
                    context.Instance.OverrideJobLimit = default;
                }
            }

            var jobId = context.Data.JobId;

            if (context.Instance.ActiveJobs.Any(x => x.JobId == jobId))
                return false;

            var timestamp = DateTime.UtcNow;

            List<KeyValuePair<Uri, JobTypeInstance>> expiredInstances =
                context.Instance.Instances.Where(x => timestamp - x.Value.Updated > heartbeatTimeout).ToList();
            foreach (KeyValuePair<Uri, JobTypeInstance> instance in expiredInstances)
                context.Instance.Instances.Remove(instance.Key);

            var concurrentJobLimit = context.Instance.OverrideJobLimit ?? context.Instance.ConcurrentJobLimit;

            var instances = from i in context.Instance.Instances
                join a in context.Instance.ActiveJobs on i.Key equals a.InstanceAddress into ai
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

            context.Instance.ActiveJobCount++;
            context.Instance.ActiveJobs.Add(new ActiveJob
            {
                JobId = jobId,
                Deadline = timestamp + context.Data.JobTimeout,
                InstanceAddress = nextInstance.InstanceAddress
            });

            context.Instance.Instances[nextInstance.InstanceAddress].Used = timestamp;

            LogContext.Debug?.Log("Allocated Job Slot: {JobId} ({JobCount}): {InstanceAddress} ({InstanceCount})", jobId, context.Instance.ActiveJobCount,
                nextInstance.InstanceAddress, nextInstance.InstanceCount + 1);

            context.CreateConsumeContext().RespondAsync<JobSlotAllocated>(new
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
                var instanceAddress = context.Data.InstanceAddress;
                if (instanceAddress != null)
                {
                    DateTime? instanceUpdated = context.CreateConsumeContext().SentTime;

                    if (context.Instance.Instances.TryGetValue(instanceAddress, out var instance))
                    {
                        if (context.Data.Kind == ConcurrentLimitKind.Stopped)
                        {
                            LogContext.Debug?.Log("Job Service Instance Stopped: {InstanceAddress}", instanceAddress);

                            context.Instance.Instances.Remove(instanceAddress);
                        }
                        else if (instanceUpdated > instance.Updated)
                            instance.Updated = instanceUpdated;
                    }
                    else
                    {
                        if (context.Data.Kind != ConcurrentLimitKind.Stopped)
                        {
                            context.Instance.Instances.Add(instanceAddress, new JobTypeInstance {Updated = instanceUpdated});

                            LogContext.Debug?.Log("Job Service Instance Started: {InstanceAddress}", instanceAddress);
                        }
                    }
                }

                if (context.Data.Kind == ConcurrentLimitKind.Configured)
                {
                    context.Instance.ConcurrentJobLimit = context.Data.ConcurrentJobLimit;

                    LogContext.Debug?.Log("Concurrent Job Limit: {ConcurrencyLimit}", context.Instance.ConcurrentJobLimit);
                }
                else if (context.Data.Kind == ConcurrentLimitKind.Override)
                {
                    context.Instance.OverrideJobLimit = context.Data.ConcurrentJobLimit;
                    context.Instance.OverrideLimitExpiration = DateTime.Now + (context.Data.Duration ?? TimeSpan.FromMinutes(30));

                    LogContext.Debug?.Log("Override Concurrent Job Limit: {ConcurrencyLimit}", context.Instance.OverrideJobLimit);
                }
            });
        }
    }
}
