namespace MassTransit
{
    using System;
    using System.Linq;
    using Configuration;
    using Contracts.JobService;
    using JobService.Messages;
    using JobService.Scheduling;


    public sealed class JobStateMachine :
        MassTransitStateMachine<JobSaga>
    {
        public JobStateMachine()
        {
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

            Event(() => CancelJob, x => x.CorrelateById(m => m.Message.JobId));
            Event(() => RetryJob, x => x.CorrelateById(m => m.Message.JobId));

            Event(() => GetJobState, x =>
            {
                x.CorrelateById(m => m.Message.JobId);
                x.ReadOnly = true;
                x.OnMissingInstance(i => i.ExecuteAsync(context => context.RespondAsync<JobState>(new
                {
                    context.Message.JobId,
                    CurrentState = "NotFound"
                })));
            });

            Schedule(() => JobSlotWaitElapsed, instance => instance.JobSlotWaitToken, x =>
            {
                x.DelayProvider = context => context.GetPayload<JobSagaSettings>().SlotWaitTime;
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
                    .IfElse(context => context.IsScheduledJob(),
                        scheduled => scheduled
                            .IfElse(context => context.CalculateNextStartDate(),
                                start => start
                                    .WaitForNextScheduledTime(this),
                                noStart => noStart
                                    .IfElse(context => context.GetPayload<JobSagaSettings>().FinalizeCompleted,
                                        final => final.Finalize(),
                                        complete => complete.TransitionTo(Completed)
                                    )
                            ),
                        immediate => immediate
                            .RequestJobSlot(this)
                    )
            );

            During(AllocatingJobSlot,
                When(JobSlotAllocated)
                    .RequestStartJob(this),
                When(JobSlotUnavailable)
                    .WaitForJobSlot(this),
                When(AllocateJobSlotFaulted)
                    .WaitForJobSlot(this),
                Ignore(CancelJob),
                Ignore(RetryJob)
            );

            During(WaitingForSlot,
                When(JobSlotWaitElapsed.Received)
                    .RequestJobSlot(this),
                When(CancelJob)
                    .Then(context =>
                    {
                        context.Saga.Faulted = DateTime.UtcNow;
                        context.Saga.Reason = "Job Cancellation Requested";
                    })
                    .PublishJobCanceled()
                    .TransitionTo(Canceled),
                Ignore(RetryJob)
            );

            During(StartingJobAttempt,
                When(StartJobAttemptFaulted)
                    .Then(context =>
                    {
                        context.Saga.Reason = context.Message.Exceptions.FirstOrDefault()?.Message;
                    })
                    .NotifyJobFaulted()
                    .TransitionTo(Faulted),
                Ignore(CancelJob),
                Ignore(RetryJob)
            );

            During(WaitingToStart,
                Ignore(CancelJob),
                Ignore(RetryJob)
            );

            During(Started, Completed, Faulted,
                Ignore(JobAttemptCreated),
                Ignore(StartJobAttemptFaulted),
                Ignore(CancelJob)
            );

            During(Started, Completed,
                Ignore(RetryJob));

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
                            .Schedule(JobRetryDelayElapsed, context => new JobRetryDelayElapsedEvent { JobId = context.Message.JobId },
                                context => context.Message.RetryDelay.Value)
                            .TransitionTo(WaitingToRetry),
                        fault => fault
                            .NotifyJobFaulted()
                            .IfElse(context => context.IsScheduledJob(),
                                scheduled => scheduled
                                    .DetermineNextStartDate()
                                    .IfElse(context => context.Saga.NextStartDate.HasValue,
                                        start => start
                                            .WaitForNextScheduledTime(this),
                                        noStart => noStart
                                            .TransitionTo(Faulted)
                                    ),
                                notScheduled => notScheduled
                                    .TransitionTo(Faulted)
                            )
                    )
            );

            During(Completed,
                When(AttemptCompleted)
                    .NotifyJobCompleted(),
                When(AttemptStarted)
                    .Then(context => context.Saga.Started = context.Message.Timestamp)
                    .PublishJobStarted(),
                When(JobCompleted)
                    .IfElse(context => context.IsScheduledJob(),
                        scheduled => scheduled
                            .DetermineNextStartDate()
                            .IfElse(context => context.Saga.NextStartDate.HasValue,
                                start => start
                                    .WaitForNextScheduledTime(this),
                                noStart => noStart
                                    .If(context => context.GetPayload<JobSagaSettings>().FinalizeCompleted, x => x.Finalize()
                                    )
                            ),
                        notScheduled => notScheduled
                            .If(context => context.GetPayload<JobSagaSettings>().FinalizeCompleted, x => x.Finalize())
                    )
            );

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
                    .RequestJobSlot(this),
                When(CancelJob)
                    .Then(context =>
                    {
                        context.Saga.Faulted = DateTime.UtcNow;
                        context.Saga.Reason = "Job Cancellation Requested";
                    })
                    .PublishJobCanceled()
                    .TransitionTo(Canceled)
            );

            During(WaitingToRetry, Faulted, Canceled,
                When(RetryJob)
                    .Unschedule(JobRetryDelayElapsed)
                    .Then(context =>
                    {
                        context.Saga.AttemptId = NewId.NextGuid();
                        context.Saga.RetryAttempt++;
                    })
                    .RequestJobSlot(this));

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
                Ignore(CancelJob),
                When(AttemptCanceled)
                    .PublishJobCanceled());

            DuringAny(
                When(GetJobState)
                    .RespondAsync(async context => new JobStateResponse
                    {
                        JobId = context.Message.JobId,
                        Submitted = context.Saga.Submitted,
                        Started = context.Saga.Started,
                        Completed = context.Saga.Completed,
                        Duration = context.Saga.Duration,
                        Faulted = context.Saga.Faulted,
                        Reason = context.Saga.Reason,
                        LastRetryAttempt = context.Saga.RetryAttempt,
                        CurrentState = (await Accessor.Get(context).ConfigureAwait(false)).Name,
                        NextStartDate = context.Saga.NextStartDate?.UtcDateTime,
                        IsRecurring = !string.IsNullOrWhiteSpace(context.Saga.CronExpression),
                        StartDate = context.Saga.StartDate?.UtcDateTime,
                        EndDate = context.Saga.EndDate?.UtcDateTime
                    })
            );

            DuringAny(
                When(JobSubmitted)
                    .If(context => context.IsScheduledJob(), x => x.UpdateRecurringJob())
            );

            During(WaitingForSlot, Canceled, Completed, Faulted,
                When(JobSubmitted)
                    .If(context => context.IsScheduledJob() && context.CalculateNextStartDate(),
                        start => start
                            .WaitForNextScheduledTime(this)
                    )
            );


            WhenEnter(Completed, x => x.SendJobSlotReleased(JobSlotDisposition.Completed));
            WhenEnter(Canceled, x => x.SendJobSlotReleased(JobSlotDisposition.Canceled));
            WhenEnter(Faulted, x => x.SendJobSlotReleased(JobSlotDisposition.Faulted));
            WhenEnter(WaitingToRetry, x => x.SendJobSlotReleased(JobSlotDisposition.Faulted));

            SetCompletedWhenFinalized();
        }

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
        public Event<CancelJob> CancelJob { get; }
        public Event<RetryJob> RetryJob { get; }

        public Event<GetJobState> GetJobState { get; }

        public Schedule<JobSaga, JobSlotWaitElapsed> JobSlotWaitElapsed { get; }

        public Schedule<JobSaga, JobRetryDelayElapsed> JobRetryDelayElapsed { get; }
    }


    static class JobStateMachineBehaviorExtensions
    {
        static Uri GetJobAttemptSagaAddress(this SagaConsumeContext<JobSaga> context)
        {
            return context.GetPayload<JobSagaSettings>().JobAttemptSagaEndpointAddress;
        }

        static Uri GetJobTypeSagaAddress(this SagaConsumeContext<JobSaga> context)
        {
            return context.GetPayload<JobSagaSettings>().JobTypeSagaEndpointAddress;
        }

        public static bool IsScheduledJob(this SagaConsumeContext<JobSaga> context)
        {
            return !string.IsNullOrWhiteSpace(context.Saga.CronExpression) || context.Saga.StartDate is not null;
        }

        public static bool CalculateNextStartDate(this SagaConsumeContext<JobSaga> context)
        {
            if (string.IsNullOrWhiteSpace(context.Saga.CronExpression))
            {
                if (context.Saga.StartDate is not null)
                {
                    // if the start date hasn't changed, clear it and return false (no schedule change)
                    if (context.Saga.StartDate == context.Saga.NextStartDate)
                    {
                        context.Saga.StartDate = null;
                        return false;
                    }

                    context.Saga.NextStartDate = context.Saga.StartDate.Value;
                    context.Saga.StartDate = null;
                    return true;
                }
            }

            var cronExpression = new CronExpression(context.Saga.CronExpression) { TimeZone = TimeZoneInfo.Utc };

            var now = DateTimeOffset.UtcNow;

            DateTimeOffset? nextStartDate = cronExpression.GetTimeAfter(context.Saga.StartDate.HasValue
                ? context.Saga.StartDate.Value > now
                    ? context.Saga.StartDate.Value
                    : now
                : now);

            if (nextStartDate != null)
            {
                if (nextStartDate.Value > context.Saga.EndDate)
                    nextStartDate = null;
            }

            // the next start date didn't change, so don't bother with it
            if (nextStartDate == context.Saga.NextStartDate)
            {
                return false;
            }

            context.Saga.NextStartDate = nextStartDate;
            return true;
        }

        public static EventActivityBinder<JobSaga, JobSubmitted> InitializeJob(this EventActivityBinder<JobSaga, JobSubmitted> binder)
        {
            return binder.Then(context =>
            {
                context.Saga.Submitted = context.Message.Timestamp;

                context.Saga.Job = context.Message.Job;
                context.Saga.ServiceAddress = context.SourceAddress;
                context.Saga.JobTimeout = context.Message.JobTimeout;
                context.Saga.JobTypeId = context.Message.JobTypeId;

                if (context.Message.Schedule != null)
                {
                    context.Saga.CronExpression = context.Message.Schedule.CronExpression;
                    context.Saga.TimeZoneId = context.Message.Schedule.TimeZoneId;
                    context.Saga.StartDate = context.Message.Schedule.Start;
                    context.Saga.EndDate = context.Message.Schedule.End;
                }

                context.Saga.AttemptId = NewId.NextGuid();
            });
        }

        public static EventActivityBinder<JobSaga, JobSubmitted> UpdateRecurringJob(this EventActivityBinder<JobSaga, JobSubmitted> binder)
        {
            return binder.Then(context =>
            {
                context.Saga.Job = context.Message.Job;

                context.Saga.CronExpression = context.Message.Schedule.CronExpression;
                context.Saga.TimeZoneId = context.Message.Schedule.TimeZoneId;
                context.Saga.StartDate = context.Message.Schedule.Start;
                context.Saga.EndDate = context.Message.Schedule.End;
            });
        }

        public static EventActivityBinder<JobSaga, T> RequestJobSlot<T>(this EventActivityBinder<JobSaga, T> binder, JobStateMachine machine)
            where T : class
        {
            return binder
                .Send<JobSaga, T, AllocateJobSlot>(context => context.GetJobTypeSagaAddress(),
                    context => new AllocateJobSlotCommand
                    {
                        JobId = context.Saga.CorrelationId,
                        JobTypeId = context.Saga.JobTypeId,
                        JobTimeout = context.Saga.JobTimeout ?? TimeSpan.Zero
                    }, (behaviorContext, context) => context.ResponseAddress = behaviorContext.ReceiveContext.InputAddress)
                .TransitionTo(machine.AllocatingJobSlot);
        }

        public static EventActivityBinder<JobSaga, JobSlotAllocated> RequestStartJob(this EventActivityBinder<JobSaga, JobSlotAllocated> binder,
            JobStateMachine machine)
        {
            return binder
                .Send<JobSaga, JobSlotAllocated, StartJobAttempt>(context => context.GetJobAttemptSagaAddress(),
                    context => new StartJobAttemptCommand
                    {
                        JobId = context.Saga.CorrelationId,
                        AttemptId = context.Saga.AttemptId,
                        ServiceAddress = context.Saga.ServiceAddress,
                        InstanceAddress = context.Message.InstanceAddress,
                        RetryAttempt = context.Saga.RetryAttempt,
                        Job = context.Saga.Job,
                        JobTypeId = context.Saga.JobTypeId
                    }, (behaviorContext, context) => context.ResponseAddress = behaviorContext.ReceiveContext.InputAddress)
                .TransitionTo(machine.StartingJobAttempt);
        }

        public static EventActivityBinder<JobSaga, T> WaitForJobSlot<T>(this EventActivityBinder<JobSaga, T> binder, JobStateMachine machine)
            where T : class
        {
            return binder.Schedule(machine.JobSlotWaitElapsed, context => new JobSlotWaitElapsedEvent { JobId = context.Saga.CorrelationId })
                .TransitionTo(machine.WaitingForSlot);
        }

        public static EventActivityBinder<JobSaga, T> WaitForNextScheduledTime<T>(this EventActivityBinder<JobSaga, T> binder, JobStateMachine machine)
            where T : class
        {
            return binder.Schedule(machine.JobSlotWaitElapsed, context => new JobSlotWaitElapsedEvent { JobId = context.Saga.CorrelationId },
                    context => context.Saga.NextStartDate.Value.DateTime)
                .TransitionTo(machine.WaitingForSlot);
        }

        public static EventActivityBinder<JobSaga, T> DetermineNextStartDate<T>(this EventActivityBinder<JobSaga, T> binder)
            where T : class
        {
            return binder.Then(context =>
            {
                context.CalculateNextStartDate();
            });
        }

        public static EventActivityBinder<JobSaga> SendJobSlotReleased(this EventActivityBinder<JobSaga> binder, JobSlotDisposition disposition)
        {
            return binder.Send<JobSaga, JobSlotReleased>(context => context.GetJobTypeSagaAddress(), context => new JobSlotReleasedEvent
            {
                JobId = context.Saga.CorrelationId,
                JobTypeId = context.Saga.JobTypeId,
                Disposition = disposition == JobSlotDisposition.Faulted && context.Saga.Reason.Contains("(Suspect)")
                    ? JobSlotDisposition.Suspect
                    : disposition
            });
        }

        public static EventActivityBinder<JobSaga, JobAttemptStarted> PublishJobStarted(this EventActivityBinder<JobSaga, JobAttemptStarted> binder)
        {
            return binder.Publish<JobSaga, JobAttemptStarted, JobStarted>(context => new JobStartedEvent
            {
                JobId = context.Saga.CorrelationId,
                AttemptId = context.Message.AttemptId,
                RetryAttempt = context.Message.RetryAttempt,
                Timestamp = context.Message.Timestamp
            });
        }

        public static EventActivityBinder<JobSaga, JobAttemptCompleted> NotifyJobCompleted(this EventActivityBinder<JobSaga, JobAttemptCompleted> binder)
        {
            return binder
                .Send<JobSaga, JobAttemptCompleted, CompleteJob>(context => context.Saga.ServiceAddress,
                    context => new CompleteJobCommand
                    {
                        JobId = context.Saga.CorrelationId,
                        Job = context.Saga.Job,
                        JobTypeId = context.Saga.JobTypeId,
                        Timestamp = context.Message.Timestamp,
                        Duration = context.Message.Timestamp - context.Saga.Started ?? TimeSpan.Zero
                    })
                .Publish<JobSaga, JobAttemptCompleted, JobCompleted>(context => new JobCompletedEvent
                {
                    JobId = context.Saga.CorrelationId,
                    Job = context.Saga.Job,
                    Timestamp = context.Message.Timestamp,
                    Duration = context.Message.Duration
                });
        }

        public static EventActivityBinder<JobSaga, JobAttemptFaulted> NotifyJobFaulted(this EventActivityBinder<JobSaga, JobAttemptFaulted> binder)
        {
            return binder
                .Send<JobSaga, JobAttemptFaulted, FaultJob>(context => context.Saga.ServiceAddress,
                    context => new FaultJobCommand
                    {
                        JobId = context.Saga.CorrelationId,
                        Job = context.Saga.Job,
                        JobTypeId = context.Saga.JobTypeId,
                        AttemptId = context.Saga.AttemptId,
                        RetryAttempt = context.Saga.RetryAttempt,
                        Exceptions = context.Message.Exceptions,
                        Duration = context.Message.Timestamp - context.Saga.Started
                    })
                .Publish<JobSaga, JobAttemptFaulted, JobFaulted>(context => new JobFaultedEvent
                {
                    JobId = context.Saga.CorrelationId,
                    Job = context.Saga.Job,
                    Exceptions = context.Message.Exceptions,
                    Timestamp = context.Message.Timestamp,
                    Duration = context.Message.Timestamp - context.Saga.Started
                });
        }

        public static EventActivityBinder<JobSaga, JobAttemptCanceled> PublishJobCanceled(this EventActivityBinder<JobSaga, JobAttemptCanceled> binder)
        {
            return binder.Publish<JobSaga, JobAttemptCanceled, JobCanceled>(context => new JobCanceledEvent
            {
                JobId = context.Saga.CorrelationId,
                Timestamp = context.Message.Timestamp
            });
        }

        public static EventActivityBinder<JobSaga, CancelJob> PublishJobCanceled(this EventActivityBinder<JobSaga, CancelJob> binder)
        {
            return binder.Publish<JobSaga, CancelJob, JobCanceled>(context => new JobCanceledEvent
            {
                JobId = context.Saga.CorrelationId,
                Timestamp = context.Message.Timestamp
            });
        }

        public static EventActivityBinder<JobSaga, Fault<StartJobAttempt>> NotifyJobFaulted(this EventActivityBinder<JobSaga, Fault<StartJobAttempt>> binder)
        {
            return binder
                .Send<JobSaga, Fault<StartJobAttempt>, FaultJob>(context => context.Saga.ServiceAddress,
                    context => new FaultJobCommand
                    {
                        JobId = context.Saga.CorrelationId,
                        Job = context.Saga.Job,
                        JobTypeId = context.Saga.JobTypeId,
                        AttemptId = context.Saga.AttemptId,
                        RetryAttempt = context.Saga.RetryAttempt,
                        Exceptions = context.Message.Exceptions?.FirstOrDefault(),
                        Duration = context.Message.Timestamp - context.Saga.Started
                    })
                .Publish<JobSaga, Fault<StartJobAttempt>, JobFaulted>(context => new JobFaultedEvent
                {
                    JobId = context.Saga.CorrelationId,
                    Job = context.Saga.Job,
                    Exceptions = context.Message.Exceptions?.FirstOrDefault(),
                    Timestamp = context.Message.Timestamp,
                    Duration = context.Message.Timestamp - context.Saga.Started
                });
        }
    }
}
