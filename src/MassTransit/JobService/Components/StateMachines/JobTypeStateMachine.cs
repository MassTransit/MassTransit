namespace MassTransit.JobService.Components.StateMachines
{
    using System;
    using System.Linq;
    using Automatonymous;
    using Automatonymous.Binders;
    using Context;
    using MassTransit.Contracts.JobService;


    public sealed class JobTypeStateMachine :
        MassTransitStateMachine<JobTypeSaga>
    {
        public JobTypeStateMachine()
        {
            InstanceState(x => x.CurrentState, Active, Idle);

            Event(() => JobSlotRequested, x => x.CorrelateById(context => context.Message.JobTypeId));
            Event(() => JobSlotReleased, x => x.CorrelateById(context => context.Message.JobTypeId));
            Event(() => SetConcurrentJobLimit, x => x.CorrelateById(context => context.Message.ConsumerTypeId));

            During(Initial, Active, Idle,
                When(JobSlotRequested)
                    .TransitionTo(Active)
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
                            .RespondAsync(context => context.Init<JobSlotAllocated>(new {context.Data.JobId})),
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
                            })
                            .If(context => context.Instance.ActiveJobCount == 0,
                                empty => empty.TransitionTo(Idle))));


            During(Initial,
                When(SetConcurrentJobLimit)
                    .SetConcurrentLimit()
                    .TransitionTo(Idle));

            During(Active, Idle,
                When(SetConcurrentJobLimit)
                    .SetConcurrentLimit());
        }

        public State Active { get; private set; }
        public State Idle { get; private set; }

        public Event<AllocateJobSlot> JobSlotRequested { get; private set; }
        public Event<JobSlotReleased> JobSlotReleased { get; private set; }
        public Event<SetConcurrentJobLimit> SetConcurrentJobLimit { get; private set; }
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
                    context.Instance.ConcurrentJobLimit = context.Data.ConcurrentLimit;
                else if (context.Data.Kind == ConcurrentLimitKind.Override)
                {
                    context.Instance.OverrideJobLimit = context.Data.ConcurrentLimit;
                    context.Instance.OverrideLimitExpiration = DateTime.Now + (context.Data.Duration ?? TimeSpan.FromMinutes(30));
                }
            });
        }
    }
}
