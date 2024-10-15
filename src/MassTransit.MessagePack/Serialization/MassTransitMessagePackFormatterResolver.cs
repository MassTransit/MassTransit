#define USE_CONCRETE_MAPPERS
namespace MassTransit.Serialization;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Contracts.JobService;
using Courier.Contracts;
using Courier.Messages;
using Events;
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

    readonly Dictionary<Type, Type> _mappedNonGenericTypes;
    readonly Dictionary<Type, Type> _mappedGenericTypes;

    readonly ConcurrentDictionary<Type, IMessagePackFormatter> _cachedFormatters;

    MassTransitMessagePackFormatterResolver()
    {
        _mappedGenericTypes = new Dictionary<Type, Type>
        {
            // May only contain open generic types.
            { typeof(MessageData<>), typeof(MessageDataFormatter<>) },
        };

        _mappedNonGenericTypes = new Dictionary<Type, Type>
        {
            // May only contain non-open generic types.
        #if USE_CONCRETE_MAPPERS
            { typeof(Fault), typeof(InterfaceConcreteMapFormatter<Fault, FaultEvent>) },
            { typeof(ReceiveFault), typeof(InterfaceConcreteMapFormatter<ReceiveFault, ReceiveFaultEvent>) },
            { typeof(ExceptionInfo), typeof(InterfaceConcreteMapFormatter<ExceptionInfo, FaultExceptionInfo>) },
            { typeof(HostInfo), typeof(InterfaceConcreteMapFormatter<HostInfo, BusHostInfo>) },
            { typeof(ScheduleMessage), typeof(InterfaceConcreteMapFormatter<ScheduleMessage, ScheduleMessageCommand>) },
            { typeof(ScheduleRecurringMessage), typeof(InterfaceConcreteMapFormatter<ScheduleRecurringMessage, ScheduleRecurringMessageCommand>) },
            { typeof(CancelScheduledMessage), typeof(InterfaceConcreteMapFormatter<CancelScheduledMessage, CancelScheduledMessageCommand>) },
            {
                typeof(CancelScheduledRecurringMessage),
                typeof(InterfaceConcreteMapFormatter<CancelScheduledRecurringMessage, CancelScheduledRecurringMessageCommand>)
            },
            {
                typeof(PauseScheduledRecurringMessage),
                typeof(InterfaceConcreteMapFormatter<PauseScheduledRecurringMessage, PauseScheduledRecurringMessageCommand>)
            },
            {
                typeof(ResumeScheduledRecurringMessage),
                typeof(InterfaceConcreteMapFormatter<ResumeScheduledRecurringMessage, ResumeScheduledRecurringMessageCommand>)
            },
            { typeof(RoutingSlip), typeof(InterfaceConcreteMapFormatter<RoutingSlip, RoutingSlipRoutingSlip>) },
            { typeof(Activity), typeof(InterfaceConcreteMapFormatter<Activity, RoutingSlipActivity>) },
            { typeof(ActivityLog), typeof(InterfaceConcreteMapFormatter<ActivityLog, RoutingSlipActivityLog>) },
            { typeof(CompensateLog), typeof(InterfaceConcreteMapFormatter<CompensateLog, RoutingSlipCompensateLog>) },
            { typeof(ActivityException), typeof(InterfaceConcreteMapFormatter<ActivityException, RoutingSlipActivityException>) },
            { typeof(Subscription), typeof(InterfaceConcreteMapFormatter<Subscription, RoutingSlipSubscription>) },
            { typeof(RoutingSlipCompleted), typeof(InterfaceConcreteMapFormatter<RoutingSlipCompleted, RoutingSlipCompletedMessage>) },
            { typeof(RoutingSlipFaulted), typeof(InterfaceConcreteMapFormatter<RoutingSlipFaulted, RoutingSlipFaultedMessage>) },
            { typeof(RoutingSlipActivityCompleted), typeof(InterfaceConcreteMapFormatter<RoutingSlipActivityCompleted, RoutingSlipActivityCompletedMessage>) },
            { typeof(RoutingSlipActivityFaulted), typeof(InterfaceConcreteMapFormatter<RoutingSlipActivityFaulted, RoutingSlipActivityFaultedMessage>) },
            {
                typeof(RoutingSlipActivityCompensated),
                typeof(InterfaceConcreteMapFormatter<RoutingSlipActivityCompensated, RoutingSlipActivityCompensatedMessage>)
            },
            {
                typeof(RoutingSlipActivityCompensationFailed),
                typeof(InterfaceConcreteMapFormatter<RoutingSlipActivityCompensationFailed, RoutingSlipActivityCompensationFailedMessage>)
            },
            {
                typeof(RoutingSlipCompensationFailed),
                typeof(InterfaceConcreteMapFormatter<RoutingSlipCompensationFailed, RoutingSlipCompensationFailedMessage>)
            },
            { typeof(RoutingSlipTerminated), typeof(InterfaceConcreteMapFormatter<RoutingSlipTerminated, RoutingSlipTerminatedMessage>) },
            { typeof(RoutingSlipRevised), typeof(InterfaceConcreteMapFormatter<RoutingSlipRevised, RoutingSlipRevisedMessage>) },
            { typeof(RecurringJobSchedule), typeof(InterfaceConcreteMapFormatter<RecurringJobSchedule, RecurringJobScheduleInfo>) },
            { typeof(AllocateJobSlot), typeof(InterfaceConcreteMapFormatter<AllocateJobSlot, AllocateJobSlotCommand>) },
            { typeof(CancelJob), typeof(InterfaceConcreteMapFormatter<CancelJob, CancelJobCommand>) },
            { typeof(CancelJobAttempt), typeof(InterfaceConcreteMapFormatter<CancelJobAttempt, CancelJobAttemptCommand>) },
            { typeof(CompleteJob), typeof(InterfaceConcreteMapFormatter<CompleteJob, CompleteJobCommand>) },
            { typeof(FaultJob), typeof(InterfaceConcreteMapFormatter<FaultJob, FaultJobCommand>) },
            { typeof(FinalizeJob), typeof(InterfaceConcreteMapFormatter<FinalizeJob, FinalizeJobCommand>) },
            { typeof(FinalizeJobAttempt), typeof(InterfaceConcreteMapFormatter<FinalizeJobAttempt, FinalizeJobAttemptCommand>) },
            { typeof(GetJobAttemptStatus), typeof(InterfaceConcreteMapFormatter<GetJobAttemptStatus, GetJobAttemptStatusRequest>) },
            { typeof(GetJobState), typeof(InterfaceConcreteMapFormatter<GetJobState, GetJobStateRequest>) },
            { typeof(JobAttemptCanceled), typeof(InterfaceConcreteMapFormatter<JobAttemptCanceled, JobAttemptCanceledEvent>) },
            { typeof(JobAttemptCompleted), typeof(InterfaceConcreteMapFormatter<JobAttemptCompleted, JobAttemptCompletedEvent>) },
            { typeof(JobAttemptFaulted), typeof(InterfaceConcreteMapFormatter<JobAttemptFaulted, JobAttemptFaultedEvent>) },
            { typeof(JobAttemptStarted), typeof(InterfaceConcreteMapFormatter<JobAttemptStarted, JobAttemptStartedEvent>) },
            { typeof(JobCanceled), typeof(InterfaceConcreteMapFormatter<JobCanceled, JobCanceledEvent>) },
            { typeof(JobCompleted), typeof(InterfaceConcreteMapFormatter<JobCompleted, JobCompletedEvent>) },
            { typeof(JobFaulted), typeof(InterfaceConcreteMapFormatter<JobFaulted, JobFaultedEvent>) },
            { typeof(JobRetryDelayElapsed), typeof(InterfaceConcreteMapFormatter<JobRetryDelayElapsed, JobRetryDelayElapsedEvent>) },
            { typeof(JobSlotAllocated), typeof(InterfaceConcreteMapFormatter<JobSlotAllocated, JobSlotAllocatedResponse>) },
            { typeof(JobSlotReleased), typeof(InterfaceConcreteMapFormatter<JobSlotReleased, JobSlotReleasedEvent>) },
            { typeof(JobSlotUnavailable), typeof(InterfaceConcreteMapFormatter<JobSlotUnavailable, JobSlotUnavailableResponse>) },
            { typeof(JobSlotWaitElapsed), typeof(InterfaceConcreteMapFormatter<JobSlotWaitElapsed, JobSlotWaitElapsedEvent>) },
            { typeof(JobState), typeof(InterfaceConcreteMapFormatter<JobState, JobStateResponse>) },
            { typeof(JobStarted), typeof(InterfaceConcreteMapFormatter<JobStarted, JobStartedEvent>) },
            { typeof(JobStatusCheckRequested), typeof(InterfaceConcreteMapFormatter<JobStatusCheckRequested, JobStatusCheckRequestedEvent>) },
            { typeof(JobSubmissionAccepted), typeof(InterfaceConcreteMapFormatter<JobSubmissionAccepted, JobSubmissionAcceptedResponse>) },
            { typeof(JobSubmitted), typeof(InterfaceConcreteMapFormatter<JobSubmitted, JobSubmittedEvent>) },
            { typeof(RetryJob), typeof(InterfaceConcreteMapFormatter<RetryJob, RetryJobCommand>) },
            { typeof(RunJob), typeof(InterfaceConcreteMapFormatter<RunJob, RunJobCommand>) },
            { typeof(SaveJobState), typeof(InterfaceConcreteMapFormatter<SaveJobState, SaveJobStateCommand>) },
            { typeof(SetConcurrentJobLimit), typeof(InterfaceConcreteMapFormatter<SetConcurrentJobLimit, SetConcurrentJobLimitCommand>) },
            { typeof(SetJobProgress), typeof(InterfaceConcreteMapFormatter<SetJobProgress, SetJobProgressCommand>) },
            { typeof(StartJob), typeof(InterfaceConcreteMapFormatter<StartJob, StartJobCommand>) },
            { typeof(StartJobAttempt), typeof(InterfaceConcreteMapFormatter<StartJobAttempt, StartJobAttemptCommand>) }
        #endif
        };

        _cachedFormatters = new ConcurrentDictionary<Type, IMessagePackFormatter>();
    }

    public IMessagePackFormatter<T>? GetFormatter<T>()
    {
        var tType = typeof(T);

        if (_cachedFormatters.TryGetValue(tType, out var cachedFormatter))
            return (IMessagePackFormatter<T>)cachedFormatter;

        if (TryGetMappedType(tType, out Type? formatterType))
        {
            var createdFormatterInstance = Activator.CreateInstance(formatterType);

            if (createdFormatterInstance is null)
                throw new InvalidOperationException($"Failed to create an instance of {formatterType}.");

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


    bool TryGetMappedType(Type originType, [NotNullWhen(true)] out Type? mappedTargetType)
    {
        // If the type is not generic, or it is a generic type definition, we use the non-generic mapping.
        return !originType.IsGenericType || originType.IsGenericTypeDefinition
            ? TryGetNonGenericMappedType(originType, out mappedTargetType)
            : TryGetOpenGenericMappedType(originType, out mappedTargetType);
    }

    bool TryGetNonGenericMappedType(Type originType, out Type? mappedTargetType)
    {
        return _mappedNonGenericTypes.TryGetValue(originType, out mappedTargetType);
    }

    bool TryGetOpenGenericMappedType(Type originType, out Type? mappedTargetType)
    {
        var genericTypeDefinition = originType.GetGenericTypeDefinition();
        if (!_mappedGenericTypes.TryGetValue(genericTypeDefinition, out Type? openGenericMappedType))
        {
            mappedTargetType = null;
            return false;
        }

        mappedTargetType = openGenericMappedType.MakeGenericType(originType.GenericTypeArguments);
        return true;
    }
}
