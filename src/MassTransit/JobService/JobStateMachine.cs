namespace MassTransit
{
    using System;
    using System.Linq;
    using Contracts.JobService;


    public sealed class JobStateMachine :
        MassTransitStateMachine<JobSaga>
    {
        readonly JobServiceOptions _options;

        public JobStateMachine(JobServiceOptions options)
        {
            _options = options;

            Event(() => JobSubmitted, x => x.CorrelateById(m => m.Message.JobId));

            Event(() => JobSlotAllocated, x =>
            {
                x.CorrelateById(m => m.Message.JobId);
                x.ConfigureConsumeTopology = false;
            });
            Event(() => JobSlotUnavailable, x =>
            {
                x.CorrelateById(m => m.Message.JobId);
                x.ConfigureConsumeTopology = false;
            });
            Event(() => AllocateJobSlotFaulted, x =>
            {
                x.CorrelateById(m => m.Message.Message.JobId);
                x.ConfigureConsumeTopology = false;
            });

            Event(() => JobAttemptCreated, x =>
            {
                x.CorrelateById(m => m.Message.JobId);
                x.ConfigureConsumeTopology = false;
            });
            Event(() => StartJobAttemptFaulted, x =>
            {
                x.CorrelateById(m => m.Message.Message.JobId);
                x.ConfigureConsumeTopology = false;
            });

            Event(() => AttemptCanceled, x => x.CorrelateById(m => m.Message.JobId));
            Event(() => AttemptCompleted, x => x.CorrelateById(m => m.Message.JobId));
            Event(() => AttemptFaulted, x => x.CorrelateById(m => m.Message.JobId));
            Event(() => AttemptStarted, x => x.CorrelateById(m => m.Message.JobId));

            Event(() => JobCompleted, x => x.CorrelateById(m => m.Message.JobId));

            Schedule(() => JobSlotWaitElapsed, instance => instance.JobSlotWaitToken, x =>
            {
                x.Delay = options.SlotWaitTime;
                x.Received = r =>
                {
                    r.CorrelateById(context => context.Message.JobId);
                    r.ConfigureConsumeTopology = false;
                };
            });

            Schedule(() => JobRetryDelayElapsed, instance => instance.JobRetryDelayToken, x =>
            {
                x.Received = r =>
                {
                    r.CorrelateById(context => context.Message.JobId);
                    r.ConfigureConsumeTopology = false;
                };
            });

            InstanceState(x => x.CurrentState, Submitted, WaitingToStart, WaitingForSlot, Started, Completed, Faulted, Canceled, StartingJobAttempt,
                AllocatingJobSlot, WaitingToRetry);

            Initially(
                When(JobSubmitted)
                    .InitializeJob()
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
                When(StartJobAttemptFaulted)
                    .Then(context =>
                    {
                        context.Saga.Reason = context.Message.Exceptions.FirstOrDefault()?.Message;
                    })
                    .NotifyJobFaulted()
                    .TransitionTo(Faulted));

            During(Started, Completed, Faulted,
                Ignore(JobAttemptCreated),
                Ignore(StartJobAttemptFaulted));

            During(StartingJobAttempt, WaitingToStart, Started,
                When(AttemptStarted)
                    .Then(context => context.Saga.Started = context.Message.Timestamp)
                    .PublishJobStarted()
                    .TransitionTo(Started));

            During(StartingJobAttempt, WaitingToStart, Started,
                When(AttemptCompleted)
                    .Then(context =>
                    {
                        context.Saga.Completed = context.Message.Timestamp;
                        context.Saga.Duration = context.Message.Duration;
                    })
                    .NotifyJobCompleted()
                    .TransitionTo(Completed));

            During(StartingJobAttempt, WaitingToStart, Started,
                When(AttemptFaulted)
                    .Then(context =>
                    {
                        context.Saga.Faulted = context.Message.Timestamp;
                        context.Saga.Reason = context.Message.Exceptions?.Message ?? "Job Attempt Faulted (unknown reason)";
                    })
                    .IfElse(context => context.Message.RetryDelay.HasValue,
                        retry => retry
                            .Schedule(JobRetryDelayElapsed, context => context.Init<JobRetryDelayElapsed>(new {context.Message.JobId}),
                                context => context.Message.RetryDelay.Value)
                            .TransitionTo(WaitingToRetry),
                        fault => fault
                            .NotifyJobFaulted()
                            .TransitionTo(Faulted)));

            During(Completed,
                When(AttemptCompleted)
                    .NotifyJobCompleted(),
                When(AttemptStarted)
                    .Then(context => context.Saga.Started = context.Message.Timestamp)
                    .PublishJobStarted(),
                When(JobCompleted)
                    .If(_ => options.FinalizeCompleted, x => x.Finalize()));

            During(Faulted,
                When(AttemptFaulted)
                    .NotifyJobFaulted(),
                When(AttemptStarted)
                    .Then(context => context.Saga.Started = context.Message.Timestamp)
                    .PublishJobStarted());

            During(WaitingToRetry,
                Ignore(AttemptFaulted),
                When(JobRetryDelayElapsed.Received)
                    .Then(context =>
                    {
                        context.Saga.AttemptId = NewId.NextGuid();
                        context.Saga.RetryAttempt++;
                    })
                    .RequestJobSlot(this)
            );

            During(StartingJobAttempt, WaitingToStart, Started,
                When(AttemptCanceled)
                    .Then(context =>
                    {
                        context.Saga.Faulted = context.Message.Timestamp;
                        context.Saga.Reason = "Job Attempt Canceled";
                    })
                    .PublishJobCanceled()
                    .TransitionTo(Canceled));

            During(Canceled,
                When(AttemptCanceled)
                    .PublishJobCanceled());

            WhenEnter(Completed, x => x.SendJobSlotReleased(this, JobSlotDisposition.Completed));
            WhenEnter(Canceled, x => x.SendJobSlotReleased(this, JobSlotDisposition.Canceled));
            WhenEnter(Faulted, x => x.SendJobSlotReleased(this, JobSlotDisposition.Faulted));
            WhenEnter(WaitingToRetry, x => x.SendJobSlotReleased(this, JobSlotDisposition.Faulted));

            SetCompletedWhenFinalized();
        }

        public Uri JobTypeSagaEndpointAddress => _options.JobTypeSagaEndpointAddress;
        public Uri JobSagaEndpointAddress => _options.JobSagaEndpointAddress;
        public Uri JobAttemptSagaEndpointAddress => _options.JobAttemptSagaEndpointAddress;

        //
        // ReSharper disable UnassignedGetOnlyAutoProperty
        // ReSharper disable MemberCanBePrivate.Global
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
    }


    static class JobStateMachineBehaviorExtensions
    {
        public static EventActivityBinder<JobSaga, JobSubmitted> InitializeJob(this EventActivityBinder<JobSaga, JobSubmitted> binder)
        {
            return binder.Then(context =>
            {
                context.Saga.Submitted = context.Message.Timestamp;

                context.Saga.Job = context.Message.Job;
                context.Saga.ServiceAddress = context.GetPayload<ConsumeContext>().SourceAddress;
                context.Saga.JobTimeout = context.Message.JobTimeout;
                context.Saga.JobTypeId = context.Message.JobTypeId;

                context.Saga.AttemptId = NewId.NextGuid();
            });
        }

        public static EventActivityBinder<JobSaga, T> RequestJobSlot<T>(this EventActivityBinder<JobSaga, T> binder,
            JobStateMachine machine)
            where T : class
        {
            return binder.SendAsync(context => machine.JobTypeSagaEndpointAddress,
                    context => context.Init<AllocateJobSlot>(new
                    {
                        JobId = context.Saga.CorrelationId,
                        context.Saga.JobTypeId,
                        context.Saga.JobTimeout
                    }), context => context.ResponseAddress = machine.JobSagaEndpointAddress)
                .TransitionTo(machine.AllocatingJobSlot);
        }

        public static EventActivityBinder<JobSaga, JobSlotAllocated> RequestStartJob(this EventActivityBinder<JobSaga, JobSlotAllocated> binder,
            JobStateMachine machine)
        {
            return binder.SendAsync(context => machine.JobAttemptSagaEndpointAddress,
                    context => context.Init<StartJobAttempt>(new
                    {
                        JobId = context.Saga.CorrelationId,
                        context.Saga.AttemptId,
                        context.Saga.ServiceAddress,
                        context.Message.InstanceAddress,
                        context.Saga.RetryAttempt,
                        context.Saga.Job,
                        context.Saga.JobTypeId
                    }), context => context.ResponseAddress = machine.JobSagaEndpointAddress)
                .TransitionTo(machine.StartingJobAttempt);
        }

        public static EventActivityBinder<JobSaga, T> WaitForJobSlot<T>(this EventActivityBinder<JobSaga, T> binder, JobStateMachine machine)
            where T : class
        {
            return binder.Schedule(machine.JobSlotWaitElapsed, context => context.Init<JobSlotWaitElapsed>(new {JobId = context.Saga.CorrelationId}))
                .TransitionTo(machine.WaitingForSlot);
        }

        public static EventActivityBinder<JobSaga> SendJobSlotReleased(this EventActivityBinder<JobSaga> binder, JobStateMachine machine,
            JobSlotDisposition disposition)
        {
            return binder.SendAsync(context => machine.JobTypeSagaEndpointAddress,
                context => context.Init<JobSlotReleased>(new
                {
                    JobId = context.Saga.CorrelationId,
                    context.Saga.JobTypeId,
                    Disposition = disposition == JobSlotDisposition.Faulted && context.Saga.Reason.Contains("(Suspect)")
                        ? JobSlotDisposition.Suspect
                        : disposition
                }));
        }

        public static EventActivityBinder<JobSaga, JobAttemptStarted> PublishJobStarted(this EventActivityBinder<JobSaga, JobAttemptStarted> binder)
        {
            return binder.PublishAsync(context => context.Init<JobStarted>(new
            {
                JobId = context.Saga.CorrelationId,
                context.Message.AttemptId,
                context.Message.RetryAttempt,
                context.Message.Timestamp
            }));
        }

        public static EventActivityBinder<JobSaga, JobAttemptCompleted> NotifyJobCompleted(this EventActivityBinder<JobSaga, JobAttemptCompleted> binder)
        {
            return binder.SendAsync(context => context.Saga.ServiceAddress,
                context => context.Init<CompleteJob>(new
                {
                    JobId = context.Saga.CorrelationId,
                    context.Saga.Job,
                    context.Saga.JobTypeId,
                    context.Message.Timestamp,
                    Duration = context.Message.Timestamp - context.Saga.Started ?? TimeSpan.Zero
                })).PublishAsync(context => context.Init<JobCompleted>(new
            {
                JobId = context.Saga.CorrelationId,
                context.Saga.Job,
                context.Message.Timestamp,
                context.Message.Duration
            }));
        }

        public static EventActivityBinder<JobSaga, JobAttemptFaulted> NotifyJobFaulted(this EventActivityBinder<JobSaga, JobAttemptFaulted> binder)
        {
            return binder.PublishAsync(context => context.Init<JobFaulted>(new
            {
                JobId = context.Saga.CorrelationId,
                context.Saga.Job,
                context.Message.Exceptions,
                context.Message.Timestamp,
                Duration = context.Message.Timestamp - context.Saga.Started
            })).SendAsync(context => context.Saga.ServiceAddress,
                context => context.Init<FaultJob>(new
                {
                    JobId = context.Saga.CorrelationId,
                    context.Saga.Job,
                    context.Saga.JobTypeId,
                    context.Saga.AttemptId,
                    context.Saga.RetryAttempt,
                    context.Message.Exceptions,
                    Duration = context.Message.Timestamp - context.Saga.Started
                }));
        }

        public static EventActivityBinder<JobSaga, JobAttemptCanceled> PublishJobCanceled(this EventActivityBinder<JobSaga, JobAttemptCanceled> binder)
        {
            return binder.PublishAsync(context => context.Init<JobCanceled>(new
            {
                JobId = context.Saga.CorrelationId,
                context.Message.Timestamp
            }));
        }

        public static EventActivityBinder<JobSaga, Fault<StartJobAttempt>> NotifyJobFaulted(this EventActivityBinder<JobSaga, Fault<StartJobAttempt>> binder)
        {
            return binder.SendAsync(context => context.Saga.ServiceAddress,
                context => context.Init<FaultJob>(new
                {
                    JobId = context.Saga.CorrelationId,
                    context.Saga.Job,
                    context.Saga.JobTypeId,
                    context.Saga.AttemptId,
                    context.Saga.RetryAttempt,
                    context.Message.Exceptions,
                    Duration = context.Message.Timestamp - context.Saga.Started
                })).PublishAsync(context => context.Init<JobFaulted>(new
            {
                JobId = context.Saga.CorrelationId,
                context.Saga.Job,
                context.Message.Exceptions,
                context.Message.Timestamp,
                Duration = context.Message.Timestamp - context.Saga.Started
            }));
        }
    }
}
