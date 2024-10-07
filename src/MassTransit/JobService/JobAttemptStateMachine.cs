namespace MassTransit
{
    using System;
    using System.Linq;
    using Configuration;
    using Contracts.JobService;
    using Events;
    using JobService.Messages;


    public sealed class JobAttemptStateMachine :
        MassTransitStateMachine<JobAttemptSaga>
    {
        public JobAttemptStateMachine()
        {
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
            Event(() => FinalizeJobAttempt, x =>
            {
                x.CorrelateById(context => context.Message.AttemptId);
                x.ConfigureConsumeTopology = false;
            });
            Event(() => CancelJobAttempt, x =>
            {
                x.CorrelateById(context => context.Message.AttemptId);
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
                x.DelayProvider = context => context.GetPayload<JobSagaSettings>().StatusCheckInterval;
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
                    .ScheduleJobStatusCheck(this)
                    .SendStartJob()
                    .TransitionTo(Starting));

            During(Starting,
                When(StartJobFaulted)
                    .Then(context =>
                    {
                        context.Saga.Faulted = context.Message.Timestamp;
                        context.Saga.InstanceAddress ??= context.SourceAddress;
                    })
                    .SendJobAttemptFaulted()
                    .TransitionTo(Faulted));

            During(Starting,
                When(StatusCheckRequested.Received)
                    .SendJobAttemptStartTimeout()
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
                When(CancelJobAttempt)
                    .SendCancelJobAttempt(),
                When(AttemptFaulted)
                    .Then(context =>
                    {
                        context.Saga.Faulted = context.Message.Timestamp;
                        context.Saga.InstanceAddress ??= context.SourceAddress;
                    })
                    .Unschedule(StatusCheckRequested)
                    .TransitionTo(Faulted));

            During(Running,
                When(StatusCheckRequested.Received)
                    .ScheduleJobStatusCheck(this)
                    .SendCheckJobStatus()
                    .TransitionTo(CheckingStatus)
            );

            During(CheckingStatus,
                When(StatusCheckRequested.Received)
                    .ScheduleJobStatusCheck(this)
                    .SendCheckJobStatus()
                    .TransitionTo(Suspect)
            );

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
                    .SendJobAttemptFaulted()
                    .TransitionTo(Faulted));

            During(Faulted,
                Ignore(StatusCheckRequested.Received),
                Ignore(AttemptFaulted));

            During([Initial, Faulted, CheckingStatus, Suspect],
                When(FinalizeJobAttempt)
                    .Finalize());

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

        //
        // ReSharper disable UnassignedGetOnlyAutoProperty
        // ReSharper disable MemberCanBePrivate.Global
        public State Starting { get; }
        public State Running { get; }
        public State CheckingStatus { get; }
        public State Suspect { get; }
        public State Faulted { get; }

        public Event<StartJobAttempt> StartJobAttempt { get; }
        public Event<Fault<StartJob>> StartJobFaulted { get; }
        public Event<FinalizeJobAttempt> FinalizeJobAttempt { get; }
        public Event<CancelJobAttempt> CancelJobAttempt { get; }

        public Event<JobAttemptStarted> AttemptStarted { get; }
        public Event<JobAttemptFaulted> AttemptFaulted { get; }
        public Event<JobAttemptCompleted> AttemptCompleted { get; }
        public Event<JobAttemptCanceled> AttemptCanceled { get; }

        public Event<JobAttemptStatus> AttemptStatus { get; }

        public Schedule<JobAttemptSaga, JobStatusCheckRequested> StatusCheckRequested { get; }
    }


    static class JobAttemptStateMachineBehaviorExtensions
    {
        public static TimeSpan? GetRetryDelay<T>(this BehaviorContext<JobAttemptSaga, T> context)
            where T : class
        {
            var settings = context.GetPayload<JobSagaSettings>();

            return context.Saga.RetryAttempt < settings.SuspectJobRetryCount
                ? settings.SuspectJobRetryDelay ?? settings.SlotWaitTime
                : default(TimeSpan?);
        }

        static Uri GetJobSagaAddress(this SagaConsumeContext<JobAttemptSaga> context)
        {
            return context.GetPayload<JobSagaSettings>().JobSagaEndpointAddress;
        }

        static Uri GetJobAttemptSagaAddress(this SagaConsumeContext<JobAttemptSaga> context)
        {
            return context.GetPayload<JobSagaSettings>().JobAttemptSagaEndpointAddress;
        }

        public static EventActivityBinder<JobAttemptSaga, StartJobAttempt> SendStartJob(this EventActivityBinder<JobAttemptSaga, StartJobAttempt> binder)
        {
            return binder.Send<JobAttemptSaga, StartJobAttempt, StartJob>(context => context.Saga.InstanceAddress ?? context.Saga.ServiceAddress,
                context => new StartJobCommand
                {
                    JobId = context.Message.JobId,
                    AttemptId = context.Message.AttemptId,
                    RetryAttempt = context.Message.RetryAttempt,
                    Job = context.Message.Job,
                    JobTypeId = context.Message.JobTypeId,
                    LastProgressValue = context.Message.LastProgressValue,
                    LastProgressLimit = context.Message.LastProgressLimit,
                    JobState = context.Message.JobState,
                    JobProperties = context.Message.JobProperties
                }, (behaviorContext, context) => context.FaultAddress = behaviorContext.GetJobAttemptSagaAddress());
        }

        public static EventActivityBinder<JobAttemptSaga, JobStatusCheckRequested> SendCheckJobStatus(this EventActivityBinder<JobAttemptSaga,
            JobStatusCheckRequested> binder)
        {
            return binder.Send<JobAttemptSaga, JobStatusCheckRequested, GetJobAttemptStatus>(
                context => context.Saga.InstanceAddress ?? context.Saga.ServiceAddress, context => new GetJobAttemptStatusRequest
                {
                    JobId = context.Saga.JobId,
                    AttemptId = context.Saga.CorrelationId
                }, (behaviorContext, context) =>
                {
                    context.RequestId = behaviorContext.Saga.CorrelationId;
                    context.ResponseAddress = behaviorContext.GetJobAttemptSagaAddress();
                });
        }

        public static EventActivityBinder<JobAttemptSaga, CancelJobAttempt> SendCancelJobAttempt(this EventActivityBinder<JobAttemptSaga,
            CancelJobAttempt> binder)
        {
            return binder.Send(context => context.Saga.InstanceAddress ?? context.Saga.ServiceAddress,
                context => context.Message,
                (behaviorContext, context) =>
                {
                    context.RequestId = behaviorContext.Saga.CorrelationId;
                    context.ResponseAddress = behaviorContext.GetJobAttemptSagaAddress();
                });
        }

        public static EventActivityBinder<JobAttemptSaga, T> ScheduleJobStatusCheck<T>(this EventActivityBinder<JobAttemptSaga, T> binder,
            JobAttemptStateMachine machine)
            where T : class
        {
            return binder.Schedule(machine.StatusCheckRequested, x => new JobStatusCheckRequestedEvent { AttemptId = x.Saga.CorrelationId });
        }

        public static EventActivityBinder<JobAttemptSaga, Fault<StartJob>> SendJobAttemptFaulted(
            this EventActivityBinder<JobAttemptSaga, Fault<StartJob>> binder)
        {
            return binder.Send<JobAttemptSaga, Fault<StartJob>, JobAttemptFaulted>(context => context.GetJobSagaAddress(),
                context => new JobAttemptFaultedEvent
                {
                    JobId = context.Saga.JobId,
                    AttemptId = context.Saga.CorrelationId,
                    RetryAttempt = context.Saga.RetryAttempt,
                    Timestamp = context.Message.Timestamp,
                    Exceptions = context.Message.Exceptions?.FirstOrDefault()
                });
        }

        public static EventActivityBinder<JobAttemptSaga, T> SendJobAttemptFaulted<T>(this EventActivityBinder<JobAttemptSaga, T> binder)
            where T : class
        {
            return binder.Send<JobAttemptSaga, T, JobAttemptFaulted>(context => context.GetJobSagaAddress(),
                context => new JobAttemptFaultedEvent
                {
                    JobId = context.Saga.JobId,
                    AttemptId = context.Saga.CorrelationId,
                    RetryAttempt = context.Saga.RetryAttempt,
                    Timestamp = DateTime.UtcNow,
                    RetryDelay = context.GetRetryDelay(),
                    Exceptions = new FaultExceptionInfo(new TimeoutException("The job status check timed out."))
                });
        }

        public static EventActivityBinder<JobAttemptSaga, T> SendJobAttemptStartTimeout<T>(this EventActivityBinder<JobAttemptSaga, T> binder)
            where T : class
        {
            return binder.Send<JobAttemptSaga, T, JobAttemptFaulted>(context => context.GetJobSagaAddress(),
                context => new JobAttemptFaultedEvent
                {
                    JobId = context.Saga.JobId,
                    AttemptId = context.Saga.CorrelationId,
                    RetryAttempt = context.Saga.RetryAttempt,
                    Timestamp = DateTime.UtcNow,
                    RetryDelay = context.GetRetryDelay(),
                    Exceptions = new FaultExceptionInfo(new TimeoutException($"The job service failed to respond: {context.Saga.InstanceAddress} (Suspect)"))
                });
        }
    }
}
