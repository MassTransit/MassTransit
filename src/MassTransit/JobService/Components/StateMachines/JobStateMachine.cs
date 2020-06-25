namespace MassTransit.JobService.Components.StateMachines
{
    using System;
    using System.Linq;
    using Automatonymous;
    using Automatonymous.Binders;
    using Automatonymous.Events;
    using GreenPipes;
    using MassTransit.Contracts.JobService;


    public sealed class JobStateMachine :
        MassTransitStateMachine<JobSaga>
    {
        public JobStateMachine(JobServiceOptions options)
        {
            Event(() => JobSubmitted, x => x.CorrelateById(m => m.Message.JobId));

            Event(() => AttemptCanceled, x => x.CorrelateById(m => m.Message.JobId));
            Event(() => AttemptCompleted, x => x.CorrelateById(m => m.Message.JobId));
            Event(() => AttemptFaulted, x => x.CorrelateById(m => m.Message.JobId));
            Event(() => AttemptStarted, x => x.CorrelateById(m => m.Message.JobId));

            Schedule(() => JobSlotWaitElapsed, instance => instance.JobSlotWaitToken, x =>
            {
                x.Delay = options.SlotWaitTime;
                x.Received = r => r.CorrelateById(context => context.Message.JobId);
            });

            Schedule(() => JobRetryDelayElapsed, instance => instance.JobRetryDelayToken, x =>
            {
                x.Received = r => r.CorrelateById(context => context.Message.JobId);
            });

            Request(() => RequestStartJob, instance => instance.StartJobRequestId, x =>
            {
                x.Timeout = options.StartJobTimeout;
                x.ServiceAddress = options.JobAttemptSagaEndpointAddress;
            });

            Request(() => RequestJobSlot, instance => instance.JobSlotRequestId, x =>
            {
                x.ServiceAddress = options.JobTypeSagaEndpointAddress;
                x.Timeout = options.JobSlotRequestTimeout;
            });

            InstanceState(x => x.CurrentState, Submitted, WaitingToStart, WaitingForSlot, Started, Completed, Faulted, Canceled,
                RequestStartJob.Pending, RequestJobSlot.Pending, WaitingToRetry);

            Initially(
                When(JobSubmitted)
                    .Then(OnJobSubmitted)
                    .RequestJobSlot(this));

            During(RequestJobSlot.Pending,
                When(RequestJobSlot.Completed)
                    .RequestStartJob(this),
                When(RequestJobSlot.Completed2)
                    .WaitForJobSlot(this),
                When(RequestJobSlot.Faulted)
                    .WaitForJobSlot(this));

            During(WaitingForSlot,
                When(JobSlotWaitElapsed.Received)
                    .RequestJobSlot(this));

            During(RequestStartJob.Pending,
                When(RequestStartJob.Completed)
                    .If(context => context.Data.AttemptId == context.Instance.AttemptId,
                        x => x.TransitionTo(WaitingToStart)),
                When(RequestStartJob.Faulted)
                    .Then(context =>
                    {
                        context.Instance.Reason = context.Data.Exceptions.FirstOrDefault()?.Message;
                    })
                    .PublishJobFaulted()
                    .TransitionTo(Faulted),
                When(RequestStartJob.TimeoutExpired)
                    .Then(context =>
                    {
                        context.Instance.Reason = "AttemptJob request timeout";
                    })
                    .PublishJobFaulted()
                    .TransitionTo(Faulted));

            During(Started, Completed, Faulted,
                Ignore(RequestStartJob.Completed),
                Ignore(RequestStartJob.TimeoutExpired),
                Ignore(RequestStartJob.Faulted));

            During(RequestStartJob.Pending, WaitingToStart, Started,
                When(AttemptStarted)
                    .Then(context => context.Instance.Started = context.Data.Timestamp)
                    .PublishJobStarted()
                    .TransitionTo(Started));


            During(RequestStartJob.Pending, WaitingToStart, Started,
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
                    .PublishJobCompleted());


            During(RequestStartJob.Pending, WaitingToStart, Started,
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

            During(RequestStartJob.Pending, WaitingToStart, Started,
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
        }

        // ReSharper disable UnassignedGetOnlyAutoProperty
        public State Submitted { get; }
        public State WaitingToStart { get; }
        public State WaitingToRetry { get; }
        public State WaitingForSlot { get; }
        public State Started { get; }
        public State Completed { get; }
        public State Canceled { get; }
        public State Faulted { get; }

        public Event<JobSubmitted> JobSubmitted { get; }

        public Event<JobAttemptStarted> AttemptStarted { get; }
        public Event<JobAttemptCompleted> AttemptCompleted { get; }
        public Event<JobAttemptCanceled> AttemptCanceled { get; }
        public Event<JobAttemptFaulted> AttemptFaulted { get; }

        public Schedule<JobSaga, JobSlotWaitElapsed> JobSlotWaitElapsed { get; }

        public Schedule<JobSaga, JobRetryDelayElapsed> JobRetryDelayElapsed { get; }

        public Request<JobSaga, AllocateJobSlot, JobSlotAllocated, JobSlotUnavailable> RequestJobSlot { get; }
        public Request<JobSaga, StartJobAttempt, JobAttemptCreated> RequestStartJob { get; }

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
            return binder.Request(machine.RequestJobSlot, context => context.Init<AllocateJobSlot>(new
                {
                    JobId = context.Instance.CorrelationId,
                    context.Instance.JobTypeId,
                    context.Instance.JobTimeout
                }))
                .TransitionTo(machine.RequestJobSlot.Pending);
        }

        public static EventActivityBinder<JobSaga, JobSlotAllocated> RequestStartJob(this EventActivityBinder<JobSaga, JobSlotAllocated> binder,
            JobStateMachine machine)
        {
            return binder.Request(machine.RequestStartJob, context => context.Init<StartJobAttempt>(new
                {
                    context.Data.JobId,
                    context.Instance.AttemptId,
                    context.Instance.ServiceAddress,
                    context.Instance.RetryAttempt,
                    context.Instance.Job
                }))
                .TransitionTo(machine.RequestStartJob.Pending);
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

        public static EventActivityBinder<JobSaga, RequestTimeoutExpired<StartJobAttempt>> PublishJobFaulted(
            this EventActivityBinder<JobSaga, RequestTimeoutExpired<StartJobAttempt>> binder)
        {
            return binder.PublishAsync(context => context.Init<JobFaulted>(new
            {
                JobId = context.Instance.CorrelationId,
                Exceptions = new TimeoutException(),
                InVar.Timestamp,
                context.Instance.Job
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
