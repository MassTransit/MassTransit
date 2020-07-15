namespace MassTransit.JobService.Components.StateMachines
{
    using System;
    using System.Linq;
    using Automatonymous;
    using Automatonymous.Binders;
    using GreenPipes;
    using MassTransit.Contracts.JobService;


    public sealed class JobStateMachine :
        MassTransitStateMachine<JobSaga>
    {
        public JobStateMachine(JobServiceOptions options)
        {
            JobTypeSagaEndpointAddress = options.JobTypeSagaEndpointAddress;
            JobSagaEndpointAddress = options.JobSagaEndpointAddress;
            JobAttemptSagaEndpointAddress = options.JobAttemptSagaEndpointAddress;

            Event(() => JobSubmitted, x => x.CorrelateById(m => m.Message.JobId));

            Event(() => JobSlotAllocated, x => x.CorrelateById(m => m.Message.JobId));
            Event(() => JobSlotUnavailable, x => x.CorrelateById(m => m.Message.JobId));
            Event(() => AllocateJobSlotFaulted, x => x.CorrelateById(m => m.Message.Message.JobId));

            Event(() => JobAttemptCreated, x => x.CorrelateById(m => m.Message.JobId));
            Event(() => StartJobAttemptFaulted, x => x.CorrelateById(m => m.Message.Message.JobId));

            Event(() => AttemptCanceled, x => x.CorrelateById(m => m.Message.JobId));
            Event(() => AttemptCompleted, x => x.CorrelateById(m => m.Message.JobId));
            Event(() => AttemptFaulted, x => x.CorrelateById(m => m.Message.JobId));
            Event(() => AttemptStarted, x => x.CorrelateById(m => m.Message.JobId));

            Event(() => JobCompleted, x => x.CorrelateById(m => m.Message.JobId));

            Schedule(() => JobSlotWaitElapsed, instance => instance.JobSlotWaitToken, x =>
            {
                x.Delay = options.SlotWaitTime;
                x.Received = r => r.CorrelateById(context => context.Message.JobId);
            });

            Schedule(() => JobRetryDelayElapsed, instance => instance.JobRetryDelayToken, x =>
            {
                x.Received = r => r.CorrelateById(context => context.Message.JobId);
            });

            InstanceState(x => x.CurrentState, Submitted, WaitingToStart, WaitingForSlot, Started, Completed, Faulted, Canceled, StartingJobAttempt,
                AllocatingJobSlot, WaitingToRetry);

            Initially(
                When(JobSubmitted)
                    .Then(OnJobSubmitted)
                    .RequestJobSlot(this));

            During(AllocatingJobSlot,
                When(JobSlotAllocated)
                    .RequestStartJob(this),
                When(JobSlotUnavailable)
                    .WaitForJobSlot(this),
                When(AllocateJobSlotFaulted)
                    .WaitForJobSlot(this));

            During(WaitingForSlot,
                When(JobSlotWaitElapsed.Received)
                    .RequestJobSlot(this));

            During(StartingJobAttempt,
                When(JobAttemptCreated)
                    .If(context => context.Data.AttemptId == context.Instance.AttemptId,
                        x => x.TransitionTo(WaitingToStart)),
                When(StartJobAttemptFaulted)
                    .Then(context =>
                    {
                        context.Instance.Reason = context.Data.Exceptions.FirstOrDefault()?.Message;
                    })
                    .PublishJobFaulted()
                    .TransitionTo(Faulted));

            During(Started, Completed, Faulted,
                Ignore(JobAttemptCreated),
                Ignore(StartJobAttemptFaulted));

            During(StartingJobAttempt, WaitingToStart, Started,
                When(AttemptStarted)
                    .Then(context => context.Instance.Started = context.Data.Timestamp)
                    .PublishJobStarted()
                    .TransitionTo(Started));

            During(StartingJobAttempt, WaitingToStart, Started,
                When(AttemptCompleted)
                    .Then(context =>
                    {
                        context.Instance.Completed = context.Data.Timestamp;
                        context.Instance.Duration = context.Data.Duration;
                    })
                    .PublishJobCompleted()
                    .TransitionTo(Completed));

            During(Completed,
                When(AttemptCompleted)
                    .PublishJobCompleted(),
                When(JobCompleted)
                    .If(_ => options.FinalizeCompleted, x => x.Finalize()));

            During(StartingJobAttempt, WaitingToStart, Started,
                When(AttemptFaulted)
                    .Then(context =>
                    {
                        context.Instance.Faulted = context.Data.Timestamp;
                    })
                    .IfElse(context => context.Data.RetryDelay.HasValue,
                        retry => retry
                            .Schedule(JobRetryDelayElapsed, context => context.Init<JobRetryDelayElapsed>(new {context.Data.JobId}),
                                context => context.Data.RetryDelay.Value)
                            .TransitionTo(WaitingToRetry),
                        fault => fault
                            .PublishJobFaulted()
                            .TransitionTo(Faulted)));

            During(Faulted,
                When(AttemptFaulted)
                    .PublishJobFaulted());

            During(WaitingToRetry,
                Ignore(AttemptFaulted),
                When(JobRetryDelayElapsed.Received)
                    .Then(context =>
                    {
                        context.Instance.AttemptId = NewId.NextGuid();
                        context.Instance.RetryAttempt++;
                    })
                    .RequestJobSlot(this)
            );

            During(StartingJobAttempt, WaitingToStart, Started,
                When(AttemptCanceled)
                    .Then(context =>
                    {
                        context.Instance.Faulted = context.Data.Timestamp;
                    })
                    .PublishJobCanceled()
                    .TransitionTo(Canceled));

            During(Canceled,
                When(AttemptCanceled)
                    .PublishJobCanceled());

            WhenEnter(Completed, x => x.SendJobSlotReleased(options.JobTypeSagaEndpointAddress));
            WhenEnter(Canceled, x => x.SendJobSlotReleased(options.JobTypeSagaEndpointAddress));
            WhenEnter(Faulted, x => x.SendJobSlotReleased(options.JobTypeSagaEndpointAddress));
            WhenEnter(WaitingToRetry, x => x.SendJobSlotReleased(options.JobTypeSagaEndpointAddress));

            SetCompletedWhenFinalized();
        }

        public Uri JobTypeSagaEndpointAddress { get; }
        public Uri JobSagaEndpointAddress { get; }
        public Uri JobAttemptSagaEndpointAddress { get; }

        // ReSharper disable UnassignedGetOnlyAutoProperty
        public State Submitted { get; }
        public State WaitingToStart { get; }
        public State WaitingToRetry { get; }
        public State WaitingForSlot { get; }
        public State Started { get; }
        public State Completed { get; }
        public State Canceled { get; }
        public State Faulted { get; }
        public State AllocatingJobSlot { get; }
        public State StartingJobAttempt { get; }

        public Event<JobSlotAllocated> JobSlotAllocated { get; }
        public Event<JobSlotUnavailable> JobSlotUnavailable { get; }
        public Event<Fault<AllocateJobSlot>> AllocateJobSlotFaulted { get; }

        public Event<JobAttemptCreated> JobAttemptCreated { get; }
        public Event<Fault<StartJobAttempt>> StartJobAttemptFaulted { get; }

        public Event<JobSubmitted> JobSubmitted { get; }

        public Event<JobAttemptStarted> AttemptStarted { get; }
        public Event<JobAttemptCompleted> AttemptCompleted { get; }
        public Event<JobAttemptCanceled> AttemptCanceled { get; }
        public Event<JobAttemptFaulted> AttemptFaulted { get; }

        public Event<JobCompleted> JobCompleted { get; }

        public Schedule<JobSaga, JobSlotWaitElapsed> JobSlotWaitElapsed { get; }

        public Schedule<JobSaga, JobRetryDelayElapsed> JobRetryDelayElapsed { get; }

        static void OnJobSubmitted(BehaviorContext<JobSaga, JobSubmitted> context)
        {
            context.Instance.Submitted = context.Data.Timestamp;

            context.Instance.Job = context.Data.Job;
            context.Instance.ServiceAddress = context.GetPayload<ConsumeContext>().SourceAddress;
            context.Instance.JobTimeout = context.Data.JobTimeout;
            context.Instance.JobTypeId = context.Data.JobTypeId;

            context.Instance.AttemptId = NewId.NextGuid();
        }
    }


    static class TurnoutJobStateMachineBehaviorExtensions
    {
        public static EventActivityBinder<JobSaga, T> RequestJobSlot<T>(this EventActivityBinder<JobSaga, T> binder,
            JobStateMachine machine)
            where T : class
        {
            return binder.SendAsync(machine.JobTypeSagaEndpointAddress, context => context.Init<AllocateJobSlot>(new
                {
                    JobId = context.Instance.CorrelationId,
                    context.Instance.JobTypeId,
                    context.Instance.JobTimeout
                }), context => context.ResponseAddress = machine.JobSagaEndpointAddress)
                .TransitionTo(machine.AllocatingJobSlot);
        }

        public static EventActivityBinder<JobSaga, JobSlotAllocated> RequestStartJob(this EventActivityBinder<JobSaga, JobSlotAllocated> binder,
            JobStateMachine machine)
        {
            return binder.SendAsync(machine.JobAttemptSagaEndpointAddress, context => context.Init<StartJobAttempt>(new
                {
                    context.Data.JobId,
                    context.Instance.AttemptId,
                    context.Instance.ServiceAddress,
                    context.Instance.RetryAttempt,
                    context.Instance.Job
                }), context => context.ResponseAddress = machine.JobSagaEndpointAddress)
                .TransitionTo(machine.StartingJobAttempt);
        }

        public static EventActivityBinder<JobSaga, T> WaitForJobSlot<T>(this EventActivityBinder<JobSaga, T> binder, JobStateMachine machine)
            where T : class
        {
            return binder.Schedule(machine.JobSlotWaitElapsed, context => context.Init<JobSlotWaitElapsed>(new {JobId = context.Instance.CorrelationId}))
                .TransitionTo(machine.WaitingForSlot);
        }

        public static EventActivityBinder<JobSaga> SendJobSlotReleased(this EventActivityBinder<JobSaga> binder, Uri destinationAddress)
        {
            return binder.SendAsync(destinationAddress, context => context.Init<JobSlotReleased>(new
            {
                JobId = context.Instance.CorrelationId,
                context.Instance.JobTypeId
            }));
        }

        public static EventActivityBinder<JobSaga, JobAttemptStarted> PublishJobStarted(this EventActivityBinder<JobSaga, JobAttemptStarted> binder)
        {
            return binder.PublishAsync(context => context.Init<JobStarted>(new
            {
                context.Data.JobId,
                context.Data.AttemptId,
                context.Data.RetryAttempt,
                context.Data.Timestamp
            }));
        }

        public static EventActivityBinder<JobSaga, JobAttemptCompleted> PublishJobCompleted(this EventActivityBinder<JobSaga, JobAttemptCompleted> binder)
        {
            return binder.PublishAsync(context => context.Init<JobCompleted>(new
            {
                context.Data.JobId,
                context.Data.Timestamp,
                context.Data.Duration,
                context.Data.Job
            }));
        }

        public static EventActivityBinder<JobSaga, JobAttemptFaulted> PublishJobFaulted(this EventActivityBinder<JobSaga, JobAttemptFaulted> binder)
        {
            return binder.PublishAsync(context => context.Init<JobFaulted>(new
            {
                context.Data.JobId,
                context.Data.Exceptions,
                context.Data.Timestamp,
                context.Data.Job
            }));
        }

        public static EventActivityBinder<JobSaga, JobAttemptCanceled> PublishJobCanceled(this EventActivityBinder<JobSaga, JobAttemptCanceled> binder)
        {
            return binder.PublishAsync(context => context.Init<JobCanceled>(new
            {
                context.Data.JobId,
                context.Data.Timestamp
            }));
        }

        public static EventActivityBinder<JobSaga, Fault<StartJobAttempt>> PublishJobFaulted(this EventActivityBinder<JobSaga, Fault<StartJobAttempt>> binder)
        {
            return binder.PublishAsync(context => context.Init<JobFaulted>(new
            {
                context.Data.Message.JobId,
                context.Data.Message.Job,
                context.Data.Timestamp,
                context.Data.Exceptions
            }));
        }
    }
}
