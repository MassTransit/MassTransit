namespace MassTransit
{
    using System;
    using Contracts.JobService;
    using Events;


    public sealed class JobAttemptStateMachine :
        MassTransitStateMachine<JobAttemptSaga>
    {
        readonly JobServiceOptions _options;

        public JobAttemptStateMachine(JobServiceOptions options)
        {
            _options = options;

            SuspectJobRetryCount = options.SuspectJobRetryCount;
            SuspectJobRetryDelay = options.SuspectJobRetryDelay ?? options.SlotWaitTime;

            Event(() => StartJobAttempt, x =>
            {
                x.CorrelateById(context => context.Message.AttemptId);
                x.ConfigureConsumeTopology = false;
            });
            Event(() => StartJobFaulted, x =>
            {
                x.CorrelateById(context => context.Message.Message.AttemptId);
                x.ConfigureConsumeTopology = false;
            });

            Event(() => AttemptCanceled, x => x.CorrelateById(context => context.Message.AttemptId));
            Event(() => AttemptCompleted, x => x.CorrelateById(context => context.Message.AttemptId));
            Event(() => AttemptFaulted, x => x.CorrelateById(context => context.Message.AttemptId));
            Event(() => AttemptStarted, x => x.CorrelateById(context => context.Message.AttemptId));

            Event(() => AttemptStatus, x =>
            {
                x.CorrelateById(context => context.Message.AttemptId);
                x.ConfigureConsumeTopology = false;
            });

            Schedule(() => StatusCheckRequested, instance => instance.StatusCheckTokenId, x =>
            {
                x.Delay = options.StatusCheckInterval;
                x.Received = r =>
                {
                    r.CorrelateById(context => context.Message.AttemptId);
                    r.ConfigureConsumeTopology = false;
                };
            });

            InstanceState(x => x.CurrentState, Starting, Running, Faulted, CheckingStatus, Suspect);

            During(Initial, Starting,
                When(StartJobAttempt)
                    .Then(context =>
                    {
                        context.Saga.JobId = context.Message.JobId;
                        context.Saga.RetryAttempt = context.Message.RetryAttempt;
                        context.Saga.InstanceAddress ??= context.Message.InstanceAddress;
                        context.Saga.ServiceAddress ??= context.Message.ServiceAddress;
                    })
                    .SendStartJob(this)
                    .ScheduleJobStatusCheck(this)
                    .TransitionTo(Starting));

            During(Starting,
                When(StartJobFaulted)
                    .Then(context =>
                    {
                        context.Saga.Faulted = context.Message.Timestamp;
                        context.Saga.InstanceAddress ??= context.GetPayload<ConsumeContext>().SourceAddress;
                    })
                    .SendJobAttemptFaulted(this)
                    .TransitionTo(Faulted));

            During(Starting,
                When(StatusCheckRequested.Received)
                    .SendJobAttemptStartTimeout(this)
                    .TransitionTo(Faulted));

            During(Initial, Starting, Running,
                When(AttemptStarted)
                    .Then(context =>
                    {
                        context.Saga.JobId = context.Message.JobId;
                        context.Saga.Started = context.Message.Timestamp;
                        context.Saga.RetryAttempt = context.Message.RetryAttempt;
                        context.Saga.InstanceAddress ??= context.Message.InstanceAddress;
                    })
                    .TransitionTo(Running));

            During(Faulted,
                When(AttemptStarted)
                    .Then(context =>
                    {
                        context.Saga.Started = context.Message.Timestamp;
                        context.Saga.RetryAttempt = context.Message.RetryAttempt;
                        context.Saga.InstanceAddress ??= context.Message.InstanceAddress;
                    }));

            During(Starting, Running, CheckingStatus, Suspect,
                When(AttemptCompleted)
                    .Unschedule(StatusCheckRequested)
                    .Finalize(),
                When(AttemptCanceled)
                    .Unschedule(StatusCheckRequested)
                    .Finalize(),
                When(AttemptFaulted)
                    .Then(context =>
                    {
                        context.Saga.Faulted = context.Message.Timestamp;
                        context.Saga.InstanceAddress ??= context.GetPayload<ConsumeContext>().SourceAddress;
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
                When(AttemptStatus, context => context.Message.Status == JobStatus.Running)
                    .TransitionTo(Running),
                When(AttemptStatus, context => context.Message.Status == JobStatus.Canceled || context.Message.Status == JobStatus.Completed)
                    .Unschedule(StatusCheckRequested)
                    .Finalize(),
                When(AttemptStatus, context => context.Message.Status == JobStatus.Faulted)
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

        public Uri JobSagaEndpointAddress => _options.JobSagaEndpointAddress;
        public Uri JobTypeSagaEndpointAddress => _options.JobTypeSagaEndpointAddress;
        public Uri JobAttemptSagaEndpointAddress => _options.JobAttemptSagaEndpointAddress; // ReSharper disable UnassignedGetOnlyAutoProperty
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
        public static EventActivityBinder<JobAttemptSaga, StartJobAttempt> SendStartJob(this EventActivityBinder<JobAttemptSaga, StartJobAttempt> binder,
            JobAttemptStateMachine machine)
        {
            return binder.SendAsync(context => context.Saga.InstanceAddress ?? context.Saga.ServiceAddress,
                context => context.Init<StartJob>(new
                {
                    context.Message.JobId,
                    context.Message.AttemptId,
                    context.Message.RetryAttempt,
                    context.Message.Job,
                    context.Message.JobTypeId,
                }), context => context.ResponseAddress = machine.JobAttemptSagaEndpointAddress);
        }

        public static EventActivityBinder<JobAttemptSaga, JobStatusCheckRequested> SendCheckJobStatus(this EventActivityBinder<JobAttemptSaga,
            JobStatusCheckRequested> binder, JobAttemptStateMachine machine)
        {
            return binder.SendAsync(context => context.Saga.InstanceAddress ?? context.Saga.ServiceAddress,
                    context => context.Init<GetJobAttemptStatus>(new
                    {
                        context.Saga.JobId,
                        AttemptId = context.Saga.CorrelationId
                    }), context => context.ResponseAddress = machine.JobAttemptSagaEndpointAddress)
                .Catch<Exception>(ex => ex.Then(context => LogContext.Error?.Log(context.Exception, "Failed sending GetJobAttemptStatus")));
        }

        public static EventActivityBinder<JobAttemptSaga, T> ScheduleJobStatusCheck<T>(this EventActivityBinder<JobAttemptSaga, T> binder,
            JobAttemptStateMachine machine)
            where T : class
        {
            return binder.Schedule(machine.StatusCheckRequested, x => x.Init<JobStatusCheckRequested>(new { AttemptId = x.Saga.CorrelationId }));
        }

        public static EventActivityBinder<JobAttemptSaga, Fault<StartJob>> SendJobAttemptFaulted(
            this EventActivityBinder<JobAttemptSaga, Fault<StartJob>> binder, JobAttemptStateMachine machine)
        {
            return binder.SendAsync(context => machine.JobSagaEndpointAddress, context => context.Init<JobAttemptFaulted>(new
            {
                context.Saga.JobId,
                AttemptId = context.Saga.CorrelationId,
                context.Saga.RetryAttempt,
                context.Message.Timestamp,
                context.Message.Exceptions
            }));
        }

        public static EventActivityBinder<JobAttemptSaga, T> SendJobAttemptFaulted<T>(this EventActivityBinder<JobAttemptSaga, T> binder,
            JobAttemptStateMachine machine)
            where T : class
        {
            return binder.SendAsync(context => machine.JobSagaEndpointAddress, context => context.Init<JobAttemptFaulted>(new
            {
                context.Saga.JobId,
                AttemptId = context.Saga.CorrelationId,
                context.Saga.RetryAttempt,
                InVar.Timestamp,
                RetryDelay = context.Saga.RetryAttempt < machine.SuspectJobRetryCount ? machine.SuspectJobRetryDelay : default(TimeSpan?),
                Exceptions = new FaultExceptionInfo(new TimeoutException("The job status check timed out."))
            }));
        }

        public static EventActivityBinder<JobAttemptSaga, T> SendJobAttemptStartTimeout<T>(this EventActivityBinder<JobAttemptSaga, T> binder,
            JobAttemptStateMachine machine)
            where T : class
        {
            return binder.SendAsync(context => machine.JobSagaEndpointAddress, context => context.Init<JobAttemptFaulted>(new
            {
                context.Saga.JobId,
                AttemptId = context.Saga.CorrelationId,
                context.Saga.RetryAttempt,
                InVar.Timestamp,
                RetryDelay = context.Saga.RetryAttempt < machine.SuspectJobRetryCount ? machine.SuspectJobRetryDelay : default(TimeSpan?),
                Exceptions = new FaultExceptionInfo(new TimeoutException($"The job service failed to respond: {context.Saga.InstanceAddress} (Suspect)"))
            }));
        }
    }
}
