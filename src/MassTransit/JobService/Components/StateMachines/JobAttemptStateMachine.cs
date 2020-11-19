namespace MassTransit.JobService.Components.StateMachines
{
    using System;
    using Automatonymous;
    using Automatonymous.Binders;
    using Contracts.JobService;
    using Events;
    using GreenPipes;


    public sealed class JobAttemptStateMachine :
        MassTransitStateMachine<JobAttemptSaga>
    {
        public JobAttemptStateMachine(JobServiceOptions options)
        {
            JobTypeSagaEndpointAddress = options.JobTypeSagaEndpointAddress;
            JobSagaEndpointAddress = options.JobSagaEndpointAddress;
            JobAttemptSagaEndpointAddress = options.JobAttemptSagaEndpointAddress;

            SuspectJobRetryCount = options.SuspectJobRetryCount;
            SuspectJobRetryDelay = options.SuspectJobRetryDelay ?? options.SlotWaitTime;

            Event(() => StartJobAttempt, x => x.CorrelateById(context => context.Message.AttemptId));
            Event(() => StartJobFaulted, x => x.CorrelateById(context => context.Message.Message.AttemptId));

            Event(() => AttemptCanceled, x => x.CorrelateById(context => context.Message.AttemptId));
            Event(() => AttemptCompleted, x => x.CorrelateById(context => context.Message.AttemptId));
            Event(() => AttemptFaulted, x => x.CorrelateById(context => context.Message.AttemptId));
            Event(() => AttemptStarted, x => x.CorrelateById(context => context.Message.AttemptId));

            Event(() => AttemptStatus, x => x.CorrelateById(context => context.Message.AttemptId));

            Schedule(() => StatusCheckRequested, instance => instance.StatusCheckTokenId, x =>
            {
                x.Delay = options.StatusCheckInterval;
                x.Received = r => r.CorrelateById(context => context.Message.AttemptId);
            });

            InstanceState(x => x.CurrentState, Starting, Running, Faulted, CheckingStatus, Suspect);

            During(Initial, Starting,
                When(StartJobAttempt)
                    .Then(context =>
                    {
                        context.Instance.JobId = context.Data.JobId;
                        context.Instance.RetryAttempt = context.Data.RetryAttempt;
                        context.Instance.ServiceAddress ??= context.Data.ServiceAddress;
                    })
                    .SendStartJob()
                    .RespondAsync(context => context.Init<JobAttemptCreated>(context.Data))
                    .TransitionTo(Starting));

            During(Starting,
                When(StartJobFaulted)
                    .Then(context =>
                    {
                        context.Instance.Faulted = context.Data.Timestamp;
                        context.Instance.InstanceAddress ??= context.GetPayload<ConsumeContext>().SourceAddress;
                    })
                    .SendJobAttemptFaulted(this)
                    .TransitionTo(Faulted));

            During(Initial, Starting, Running,
                When(AttemptStarted)
                    .Then(context =>
                    {
                        context.Instance.JobId = context.Data.JobId;
                        context.Instance.Started = context.Data.Timestamp;
                        context.Instance.RetryAttempt = context.Data.RetryAttempt;
                        context.Instance.InstanceAddress ??= context.Data.InstanceAddress;
                    })
                    .ScheduleJobStatusCheck(this)
                    .TransitionTo(Running));

            During(Running, CheckingStatus, Suspect,
                When(AttemptCompleted)
                    .Unschedule(StatusCheckRequested)
                    .Finalize(),
                When(AttemptCanceled)
                    .Unschedule(StatusCheckRequested)
                    .Finalize(),
                When(AttemptFaulted)
                    .Then(context =>
                    {
                        context.Instance.Faulted = context.Data.Timestamp;
                        context.Instance.InstanceAddress ??= context.GetPayload<ConsumeContext>().SourceAddress;
                    })
                    .Unschedule(StatusCheckRequested)
                    .TransitionTo(Faulted));

            During(Running,
                When(StatusCheckRequested.Received)
                    .SendCheckJobStatus(this)
                    .TransitionTo(CheckingStatus)
                    .Catch<Exception>(eb => eb.TransitionTo(Suspect))
                    .ScheduleJobStatusCheck(this));

            During(CheckingStatus,
                When(StatusCheckRequested.Received)
                    .SendCheckJobStatus(this)
                    .TransitionTo(Suspect)
                    .Catch<Exception>(eb => eb.TransitionTo(Suspect))
                    .ScheduleJobStatusCheck(this));

            During(Running, CheckingStatus, Suspect,
                When(AttemptStatus, context => context.Data.Status == JobStatus.Running)
                    .TransitionTo(Running),
                When(AttemptStatus, context => context.Data.Status == JobStatus.Canceled || context.Data.Status == JobStatus.Completed)
                    .Unschedule(StatusCheckRequested)
                    .Finalize(),
                When(AttemptStatus, context => context.Data.Status == JobStatus.Faulted)
                    .Unschedule(StatusCheckRequested)
                    .TransitionTo(Faulted));

            During(Suspect,
                When(StatusCheckRequested.Received)
                    .SendJobAttemptFaulted(this)
                    .TransitionTo(Faulted));

            During(Faulted,
                Ignore(StatusCheckRequested.Received),
                Ignore(AttemptFaulted));

            During(Initial,
                When(AttemptCompleted)
                    .Finalize(),
                When(AttemptCanceled)
                    .Finalize(),
                When(StatusCheckRequested.Received)
                    .Finalize(),
                When(AttemptStatus)
                    .Finalize());

            SetCompletedWhenFinalized();
        }

        public int SuspectJobRetryCount { get; }
        public TimeSpan SuspectJobRetryDelay { get; }
        public Uri JobTypeSagaEndpointAddress { get; }
        public Uri JobSagaEndpointAddress { get; }
        public Uri JobAttemptSagaEndpointAddress { get; } // ReSharper disable UnassignedGetOnlyAutoProperty

        // ReSharper disable MemberCanBePrivate.Global
        public State Starting { get; }
        public State Running { get; }
        public State CheckingStatus { get; }
        public State Suspect { get; }
        public State Faulted { get; }

        public Event<StartJobAttempt> StartJobAttempt { get; }
        public Event<Fault<StartJob>> StartJobFaulted { get; }

        public Event<JobAttemptStarted> AttemptStarted { get; }
        public Event<JobAttemptFaulted> AttemptFaulted { get; }
        public Event<JobAttemptCompleted> AttemptCompleted { get; }
        public Event<JobAttemptCanceled> AttemptCanceled { get; }

        public Event<JobAttemptStatus> AttemptStatus { get; }

        public Schedule<JobAttemptSaga, JobStatusCheckRequested> StatusCheckRequested { get; }
    }


    static class JobAttemptStateMachineBehaviorExtensions
    {
        public static EventActivityBinder<JobAttemptSaga, StartJobAttempt> SendStartJob(this EventActivityBinder<JobAttemptSaga, StartJobAttempt> binder)
        {
            return binder.SendAsync(context => context.Instance.ServiceAddress, context => context.Init<StartJob>(new
            {
                context.Data.JobId,
                context.Data.AttemptId,
                context.Data.RetryAttempt,
                context.Data.Job
            }));
        }

        public static EventActivityBinder<JobAttemptSaga, JobStatusCheckRequested> SendCheckJobStatus(this EventActivityBinder<JobAttemptSaga,
            JobStatusCheckRequested> binder, JobAttemptStateMachine machine)
        {
            return binder.SendAsync(context => context.Instance.ServiceAddress, context => context.Init<GetJobAttemptStatus>(new
            {
                context.Instance.JobId,
                AttemptId = context.Instance.CorrelationId
            }), context => context.ResponseAddress = machine.JobAttemptSagaEndpointAddress);
        }

        public static EventActivityBinder<JobAttemptSaga, T> ScheduleJobStatusCheck<T>(this EventActivityBinder<JobAttemptSaga, T> binder,
            JobAttemptStateMachine machine)
            where T : class
        {
            return binder.Schedule(machine.StatusCheckRequested, x => x.Init<JobStatusCheckRequested>(new {AttemptId = x.Instance.CorrelationId}));
        }

        public static EventActivityBinder<JobAttemptSaga, Fault<StartJob>> SendJobAttemptFaulted(
            this EventActivityBinder<JobAttemptSaga, Fault<StartJob>> binder, JobAttemptStateMachine machine)
        {
            return binder.SendAsync(machine.JobSagaEndpointAddress, context => context.Init<JobAttemptFaulted>(new
            {
                context.Instance.JobId,
                AttemptId = context.Instance.CorrelationId,
                context.Instance.RetryAttempt,
                context.Data.Timestamp,
                context.Data.Exceptions
            }));
        }

        public static EventActivityBinder<JobAttemptSaga, T> SendJobAttemptFaulted<T>(this EventActivityBinder<JobAttemptSaga, T> binder,
            JobAttemptStateMachine machine)
            where T : class
        {
            return binder.SendAsync(machine.JobSagaEndpointAddress, context => context.Init<JobAttemptFaulted>(new
            {
                context.Instance.JobId,
                AttemptId = context.Instance.CorrelationId,
                context.Instance.RetryAttempt,
                InVar.Timestamp,
                RetryDelay = context.Instance.RetryAttempt < machine.SuspectJobRetryCount ? machine.SuspectJobRetryDelay : default(TimeSpan?),
                Exceptions = new FaultExceptionInfo(new TimeoutException("The job status check timed out."))
            }));
        }
    }
}
