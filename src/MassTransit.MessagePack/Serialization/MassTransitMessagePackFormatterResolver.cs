#define USE_CONCRETE_MAPPERS
namespace MassTransit.Serialization;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Contracts.JobService;
using Courier.Contracts;
using Courier.Messages;
using Events;
using Internals;
using JobService.Messages;
using MessagePack;
using MessagePack.Formatters;
using MessagePackFormatters;
using Metadata;
using Scheduling;


class MassTransitMessagePackFormatterResolver :
    IFormatterResolver
{
    public static MassTransitMessagePackFormatterResolver Instance { get; } = new();

    readonly IReadOnlyCollection<KeyValuePair<Type, Type>> _mappedFormatterByType;
    readonly ConcurrentDictionary<Type, IMessagePackFormatter> _cachedFormatters;

    MassTransitMessagePackFormatterResolver()
    {
        _mappedFormatterByType = new List<KeyValuePair<Type, Type>>
        {
            new(typeof(MessageData<>), typeof(MessageDataFormatter<>)),
        #if USE_CONCRETE_MAPPERS
            new(typeof(Fault), typeof(InterfaceConcreteMapFormatter<Fault, FaultEvent>)),
            new(typeof(ReceiveFault), typeof(InterfaceConcreteMapFormatter<ReceiveFault, ReceiveFaultEvent>)),
            new(typeof(ExceptionInfo), typeof(InterfaceConcreteMapFormatter<ExceptionInfo, FaultExceptionInfo>)),
            new(typeof(HostInfo), typeof(InterfaceConcreteMapFormatter<HostInfo, BusHostInfo>)),
            new(typeof(ScheduleMessage), typeof(InterfaceConcreteMapFormatter<ScheduleMessage, ScheduleMessageCommand>)),
            new(typeof(ScheduleRecurringMessage), typeof(InterfaceConcreteMapFormatter<ScheduleRecurringMessage, ScheduleRecurringMessageCommand>)),
            new(typeof(CancelScheduledMessage), typeof(InterfaceConcreteMapFormatter<CancelScheduledMessage, CancelScheduledMessageCommand>)),
            new(typeof(CancelScheduledRecurringMessage),
                typeof(InterfaceConcreteMapFormatter<CancelScheduledRecurringMessage, CancelScheduledRecurringMessageCommand>)),
            new(typeof(PauseScheduledRecurringMessage),
                typeof(InterfaceConcreteMapFormatter<PauseScheduledRecurringMessage, PauseScheduledRecurringMessageCommand>)),
            new(typeof(ResumeScheduledRecurringMessage),
                typeof(InterfaceConcreteMapFormatter<ResumeScheduledRecurringMessage, ResumeScheduledRecurringMessageCommand>)),
            new(typeof(RoutingSlip), typeof(InterfaceConcreteMapFormatter<RoutingSlip, RoutingSlipRoutingSlip>)),
            new(typeof(Activity), typeof(InterfaceConcreteMapFormatter<Activity, RoutingSlipActivity>)),
            new(typeof(ActivityLog), typeof(InterfaceConcreteMapFormatter<ActivityLog, RoutingSlipActivityLog>)),
            new(typeof(CompensateLog), typeof(InterfaceConcreteMapFormatter<CompensateLog, RoutingSlipCompensateLog>)),
            new(typeof(ActivityException), typeof(InterfaceConcreteMapFormatter<ActivityException, RoutingSlipActivityException>)),
            new(typeof(Subscription), typeof(InterfaceConcreteMapFormatter<Subscription, RoutingSlipSubscription>)),
            new(typeof(RoutingSlipCompleted), typeof(InterfaceConcreteMapFormatter<RoutingSlipCompleted, RoutingSlipCompletedMessage>)),
            new(typeof(RoutingSlipFaulted), typeof(InterfaceConcreteMapFormatter<RoutingSlipFaulted, RoutingSlipFaultedMessage>)),
            new(typeof(RoutingSlipActivityCompleted), typeof(InterfaceConcreteMapFormatter<RoutingSlipActivityCompleted, RoutingSlipActivityCompletedMessage>)),
            new(typeof(RoutingSlipActivityFaulted), typeof(InterfaceConcreteMapFormatter<RoutingSlipActivityFaulted, RoutingSlipActivityFaultedMessage>)),
            new(typeof(RoutingSlipActivityCompensated),
                typeof(InterfaceConcreteMapFormatter<RoutingSlipActivityCompensated, RoutingSlipActivityCompensatedMessage>)),
            new(typeof(RoutingSlipActivityCompensationFailed),
                typeof(InterfaceConcreteMapFormatter<RoutingSlipActivityCompensationFailed, RoutingSlipActivityCompensationFailedMessage>)),
            new(typeof(RoutingSlipCompensationFailed),
                typeof(InterfaceConcreteMapFormatter<RoutingSlipCompensationFailed, RoutingSlipCompensationFailedMessage>)),
            new(typeof(RoutingSlipTerminated), typeof(InterfaceConcreteMapFormatter<RoutingSlipTerminated, RoutingSlipTerminatedMessage>)),
            new(typeof(RoutingSlipRevised), typeof(InterfaceConcreteMapFormatter<RoutingSlipRevised, RoutingSlipRevisedMessage>)),
            new(typeof(RecurringJobSchedule), typeof(InterfaceConcreteMapFormatter<RecurringJobSchedule, RecurringJobScheduleInfo>)),
            new(typeof(AllocateJobSlot), typeof(InterfaceConcreteMapFormatter<AllocateJobSlot, AllocateJobSlotCommand>)),
            new(typeof(CancelJob), typeof(InterfaceConcreteMapFormatter<CancelJob, CancelJobCommand>)),
            new(typeof(CancelJobAttempt), typeof(InterfaceConcreteMapFormatter<CancelJobAttempt, CancelJobAttemptCommand>)),
            new(typeof(CompleteJob), typeof(InterfaceConcreteMapFormatter<CompleteJob, CompleteJobCommand>)),
            new(typeof(FaultJob), typeof(InterfaceConcreteMapFormatter<FaultJob, FaultJobCommand>)),
            new(typeof(FinalizeJob), typeof(InterfaceConcreteMapFormatter<FinalizeJob, FinalizeJobCommand>)),
            new(typeof(FinalizeJobAttempt), typeof(InterfaceConcreteMapFormatter<FinalizeJobAttempt, FinalizeJobAttemptCommand>)),
            new(typeof(GetJobAttemptStatus), typeof(InterfaceConcreteMapFormatter<GetJobAttemptStatus, GetJobAttemptStatusRequest>)),
            new(typeof(GetJobState), typeof(InterfaceConcreteMapFormatter<GetJobState, GetJobStateRequest>)),
            new(typeof(JobAttemptCanceled), typeof(InterfaceConcreteMapFormatter<JobAttemptCanceled, JobAttemptCanceledEvent>)),
            new(typeof(JobAttemptCompleted), typeof(InterfaceConcreteMapFormatter<JobAttemptCompleted, JobAttemptCompletedEvent>)),
            new(typeof(JobAttemptFaulted), typeof(InterfaceConcreteMapFormatter<JobAttemptFaulted, JobAttemptFaultedEvent>)),
            new(typeof(JobAttemptStarted), typeof(InterfaceConcreteMapFormatter<JobAttemptStarted, JobAttemptStartedEvent>)),
            new(typeof(JobCanceled), typeof(InterfaceConcreteMapFormatter<JobCanceled, JobCanceledEvent>)),
            new(typeof(JobCompleted), typeof(InterfaceConcreteMapFormatter<JobCompleted, JobCompletedEvent>)),
            new(typeof(JobFaulted), typeof(InterfaceConcreteMapFormatter<JobFaulted, JobFaultedEvent>)),
            new(typeof(JobRetryDelayElapsed), typeof(InterfaceConcreteMapFormatter<JobRetryDelayElapsed, JobRetryDelayElapsedEvent>)),
            new(typeof(JobSlotAllocated), typeof(InterfaceConcreteMapFormatter<JobSlotAllocated, JobSlotAllocatedResponse>)),
            new(typeof(JobSlotReleased), typeof(InterfaceConcreteMapFormatter<JobSlotReleased, JobSlotReleasedEvent>)),
            new(typeof(JobSlotUnavailable), typeof(InterfaceConcreteMapFormatter<JobSlotUnavailable, JobSlotUnavailableResponse>)),
            new(typeof(JobSlotWaitElapsed), typeof(InterfaceConcreteMapFormatter<JobSlotWaitElapsed, JobSlotWaitElapsedEvent>)),
            new(typeof(JobState), typeof(InterfaceConcreteMapFormatter<JobState, JobStateResponse>)),
            new(typeof(JobStarted), typeof(InterfaceConcreteMapFormatter<JobStarted, JobStartedEvent>)),
            new(typeof(JobStatusCheckRequested), typeof(InterfaceConcreteMapFormatter<JobStatusCheckRequested, JobStatusCheckRequestedEvent>)),
            new(typeof(JobSubmissionAccepted), typeof(InterfaceConcreteMapFormatter<JobSubmissionAccepted, JobSubmissionAcceptedResponse>)),
            new(typeof(JobSubmitted), typeof(InterfaceConcreteMapFormatter<JobSubmitted, JobSubmittedEvent>)),
            new(typeof(RetryJob), typeof(InterfaceConcreteMapFormatter<RetryJob, RetryJobCommand>)),
            new(typeof(RunJob), typeof(InterfaceConcreteMapFormatter<RunJob, RunJobCommand>)),
            new(typeof(SaveJobState), typeof(InterfaceConcreteMapFormatter<SaveJobState, SaveJobStateCommand>)),
            new(typeof(SetConcurrentJobLimit), typeof(InterfaceConcreteMapFormatter<SetConcurrentJobLimit, SetConcurrentJobLimitCommand>)),
            new(typeof(SetJobProgress), typeof(InterfaceConcreteMapFormatter<SetJobProgress, SetJobProgressCommand>)),
            new(typeof(StartJob), typeof(InterfaceConcreteMapFormatter<StartJob, StartJobCommand>)),
            new(typeof(StartJobAttempt), typeof(InterfaceConcreteMapFormatter<StartJobAttempt, StartJobAttemptCommand>))
        #endif
        };

        _cachedFormatters = new ConcurrentDictionary<Type, IMessagePackFormatter>();
    }

    public IMessagePackFormatter<T>? GetFormatter<T>()
    {
        var tType = typeof(T);

        if (_cachedFormatters.TryGetValue(tType, out var cachedFormatter))
        {
            return (IMessagePackFormatter<T>)cachedFormatter;
        }

        if (TryGetMappedType(tType, out var mappedPair))
        {
            var formatterType = mappedPair.Value;

            EnsureFormatterTypeHasGenericParametersOfOriginalType(tType, ref formatterType);

            var createdFormatterInstance = Activator.CreateInstance(formatterType);

            if (createdFormatterInstance is null)
            {
                throw new InvalidOperationException($"Failed to create an instance of {formatterType}.");
            }

            var formatter = (IMessagePackFormatter)createdFormatterInstance;
            _cachedFormatters[tType] = formatter;
            return (IMessagePackFormatter<T>)formatter;
        }

        if (!typeof(T).IsInterface)
        {
            // If we have no mapper for the type, and it's not an interface, we can't create a formatter.
            return null;
        }

        var createdConcreteFormatter = new InterfaceMessagePackFormatter<T>();
        _cachedFormatters[tType] = createdConcreteFormatter;

        return createdConcreteFormatter;
    }

    static void EnsureFormatterTypeHasGenericParametersOfOriginalType(Type tType, ref Type formatterType)
    {
        if (!formatterType.IsGenericTypeDefinition)
        {
            return;
        }

        formatterType = formatterType
            .MakeGenericType(tType.GenericTypeArguments);
    }

    bool TryGetMappedType(Type originType, out KeyValuePair<Type, Type> mappedPair)
    {
        foreach (var mapPair in _mappedFormatterByType)
        {
            /*
             * If the mapped type is not an open generic, and the origin type is not the same as the mapped type, skip.
             */
            if ((!mapPair.Key.IsOpenGeneric() || !originType.ClosesType(mapPair.Key)) && originType != mapPair.Key)
            {
                continue;
            }

            mappedPair = mapPair;
            return true;
        }

        mappedPair = default;
        return false;
    }
}
