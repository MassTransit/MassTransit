namespace MassTransit.JobService.Components.StateMachines
{
    using System;
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
                    .IfElse(context => context.IsSlotAvailable(),
                        allocate => allocate
                            .Then(context =>
                            {
                                context.Instance.ActiveJobCount++;
                                context.Instance.ActiveJobs.Add(new ActiveJob
                                {
                                    JobId = context.Data.JobId,
                                    Deadline = DateTime.Now + context.Data.JobTimeout
                                });

                                LogContext.Debug?.Log("Allocated Job Slot: {JobId} ({JobCount})", context.Data.JobId, context.Instance.ActiveJobCount);
                            })
                            .RespondAsync(context => context.Init<JobSlotAllocated>(new {context.Data.JobId}))
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

                                    LogContext.Debug?.Log("Released Job Slot: {JobId} ({JobCount})", context.Data.JobId, context.Instance.ActiveJobCount);
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
                    .SetConcurrentLimit());
        } // ReSharper disable UnassignedGetOnlyAutoProperty
        public State Active { get; }
        public State Idle { get; }

        public Event<AllocateJobSlot> JobSlotRequested { get; }
        public Event<JobSlotReleased> JobSlotReleased { get; }
        public Event<SetConcurrentJobLimit> SetConcurrentJobLimit { get; }
    }


    static class TurnoutJobTypeStateMachineBehaviorExtensions
    {
        public static bool IsSlotAvailable(this BehaviorContext<JobTypeSaga, AllocateJobSlot> context)
        {
            if (context.Instance.OverrideLimitExpiration.HasValue)
            {
                if (context.Instance.OverrideLimitExpiration.Value <= DateTime.Now)
                {
                    context.Instance.OverrideLimitExpiration = default;
                    context.Instance.OverrideJobLimit = default;
                }
            }

            return context.Instance.ActiveJobCount < (context.Instance.OverrideJobLimit ?? context.Instance.ConcurrentJobLimit)
                && context.Instance.ActiveJobs.All(x => x.JobId != context.Data.JobId);
        }

        public static EventActivityBinder<JobTypeSaga, SetConcurrentJobLimit> SetConcurrentLimit(
            this EventActivityBinder<JobTypeSaga, SetConcurrentJobLimit> binder)
        {
            return binder.Then(context =>
            {
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
