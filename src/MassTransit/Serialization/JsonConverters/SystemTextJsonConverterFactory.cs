namespace MassTransit.Serialization.JsonConverters
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Batching;
    using Contracts.JobService;
    using Courier.Contracts;
    using Courier.Messages;
    using Events;
    using Internals;
    using JobService.Messages;
    using Metadata;
    using Scheduling;


    public class SystemTextJsonConverterFactory :
        JsonConverterFactory
    {
        static readonly IDictionary<Type, Func<JsonConverter>> _converterFactory;

        static readonly IDictionary<Type, Type> _openTypeFactory = new Dictionary<Type, Type>
        {
            { typeof(Fault<>), typeof(FaultEvent<>) },
            { typeof(Batch<>), typeof(MessageBatch<>) },
            { typeof(SubmitJob<>), typeof(SubmitJobCommand<>) },
            { typeof(JobCompleted<>), typeof(JobCompletedEvent<>) },
        };

        static SystemTextJsonConverterFactory()
        {
            _converterFactory = new Dictionary<Type, Func<JsonConverter>>()
                .Add<Fault, FaultEvent>()
                .Add<ReceiveFault, ReceiveFaultEvent>()
                .Add<ExceptionInfo, FaultExceptionInfo>()
                .Add<HostInfo, BusHostInfo>()
                .Add<ScheduleMessage, ScheduleMessageCommand>()
                .Add<ScheduleRecurringMessage, ScheduleRecurringMessageCommand>()
                .Add<CancelScheduledMessage, CancelScheduledMessageCommand>()
                .Add<CancelScheduledRecurringMessage, CancelScheduledRecurringMessageCommand>()
                .Add<PauseScheduledRecurringMessage, PauseScheduledRecurringMessageCommand>()
                .Add<ResumeScheduledRecurringMessage, ResumeScheduledRecurringMessageCommand>()
                .Add<MessageEnvelope, JsonMessageEnvelope>()
                .Add<RoutingSlip, RoutingSlipRoutingSlip>()
                .Add<Activity, RoutingSlipActivity>()
                .Add<ActivityLog, RoutingSlipActivityLog>()
                .Add<CompensateLog, RoutingSlipCompensateLog>()
                .Add<ActivityException, RoutingSlipActivityException>()
                .Add<Subscription, RoutingSlipSubscription>()
                .Add<RoutingSlipCompleted, RoutingSlipCompletedMessage>()
                .Add<RoutingSlipFaulted, RoutingSlipFaultedMessage>()
                .Add<RoutingSlipActivityCompleted, RoutingSlipActivityCompletedMessage>()
                .Add<RoutingSlipActivityFaulted, RoutingSlipActivityFaultedMessage>()
                .Add<RoutingSlipActivityCompensated, RoutingSlipActivityCompensatedMessage>()
                .Add<RoutingSlipActivityCompensationFailed, RoutingSlipActivityCompensationFailedMessage>()
                .Add<RoutingSlipCompensationFailed, RoutingSlipCompensationFailedMessage>()
                .Add<RoutingSlipTerminated, RoutingSlipTerminatedMessage>()
                .Add<RoutingSlipRevised, RoutingSlipRevisedMessage>()
                .Add<RecurringJobSchedule, RecurringJobScheduleInfo>()
                .Add<AllocateJobSlot, AllocateJobSlotCommand>()
                .Add<CancelJob, CancelJobCommand>()
                .Add<CancelJobAttempt, CancelJobAttemptCommand>()
                .Add<CompleteJob, CompleteJobCommand>()
                .Add<FaultJob, FaultJobCommand>()
                .Add<FinalizeJob, FinalizeJobCommand>()
                .Add<FinalizeJobAttempt, FinalizeJobAttemptCommand>()
                .Add<GetJobAttemptStatus, GetJobAttemptStatusRequest>()
                .Add<GetJobState, GetJobStateRequest>()
                .Add<JobAttemptCanceled, JobAttemptCanceledEvent>()
                .Add<JobAttemptCompleted, JobAttemptCompletedEvent>()
                .Add<JobAttemptFaulted, JobAttemptFaultedEvent>()
                .Add<JobAttemptStarted, JobAttemptStartedEvent>()
                .Add<JobCanceled, JobCanceledEvent>()
                .Add<JobCompleted, JobCompletedEvent>()
                .Add<JobFaulted, JobFaultedEvent>()
                .Add<JobRetryDelayElapsed, JobRetryDelayElapsedEvent>()
                .Add<JobSlotAllocated, JobSlotAllocatedResponse>()
                .Add<JobSlotReleased, JobSlotReleasedEvent>()
                .Add<JobSlotUnavailable, JobSlotUnavailableResponse>()
                .Add<JobSlotWaitElapsed, JobSlotWaitElapsedEvent>()
                .Add<JobState, JobStateResponse>()
                .Add<JobStarted, JobStartedEvent>()
                .Add<JobStatusCheckRequested, JobStatusCheckRequestedEvent>()
                .Add<JobSubmissionAccepted, JobSubmissionAcceptedResponse>()
                .Add<JobSubmitted, JobSubmittedEvent>()
                .Add<RetryJob, RetryJobCommand>()
                .Add<RunJob, RunJobCommand>()
                .Add<SaveJobState, SaveJobStateCommand>()
                .Add<SetConcurrentJobLimit, SetConcurrentJobLimitCommand>()
                .Add<SetJobProgress, SetJobProgressCommand>()
                .Add<StartJob, StartJobCommand>()
                .Add<StartJobAttempt, StartJobAttemptCommand>();
        }

        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert.IsGenericType)
            {
                if (typeToConvert.ClosesType(typeof(IDictionary<,>), out Type[] elementTypes)
                    || typeToConvert.ClosesType(typeof(IReadOnlyDictionary<,>), out elementTypes)
                    || typeToConvert.ClosesType(typeof(Dictionary<,>), out elementTypes)
                    || (typeToConvert.ClosesType(typeof(IEnumerable<>), out Type[] enumerableType)
                        && enumerableType[0].ClosesType(typeof(KeyValuePair<,>), out elementTypes)
                        && elementTypes[1] == typeof(object)
                        && !typeToConvert.ClosesType(typeof(IReadOnlyList<>))))
                {
                    var keyType = elementTypes[0];

                    if (keyType != typeof(string) && keyType != typeof(Uri))
                        return false;

                    if (typeToConvert.IsFSharpType())
                        return false;

                    return true;
                }
            }

            if (!typeToConvert.IsInterface)
                return false;

            if (_converterFactory.TryGetValue(typeToConvert, out Func<JsonConverter> _))
                return true;

            if (_openTypeFactory.TryGetValue(typeToConvert, out _))
                return true;

            if (IsConvertibleInterfaceType(typeToConvert))
                return true;

            return false;
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (_converterFactory.TryGetValue(typeToConvert, out Func<JsonConverter> converterFactory))
                return converterFactory();

            if (typeToConvert.IsGenericType)
            {
                if (!typeToConvert.IsFSharpType())
                {
                    if (typeToConvert == typeof(IDictionary<string, object>))
                        return new CaseInsensitiveDictionaryStringObjectJsonConverter<IDictionary<string, object>>();
                    if (typeToConvert == typeof(Dictionary<string, object>))
                        return new CaseInsensitiveDictionaryStringObjectJsonConverter<Dictionary<string, object>>();
                    if (typeToConvert == typeof(IReadOnlyDictionary<string, object>))
                        return new CaseInsensitiveDictionaryStringObjectJsonConverter<IReadOnlyDictionary<string, object>>();
                    if (typeToConvert == typeof(IEnumerable<KeyValuePair<string, object>>))
                        return new CaseInsensitiveDictionaryStringObjectJsonConverter<IEnumerable<KeyValuePair<string, object>>>();

                    if (typeToConvert.ClosesType(typeof(IDictionary<,>), out Type[] elementTypes)
                        || typeToConvert.ClosesType(typeof(IReadOnlyDictionary<,>), out elementTypes)
                        || typeToConvert.ClosesType(typeof(Dictionary<,>), out elementTypes)
                        || (typeToConvert.ClosesType(typeof(IEnumerable<>), out Type[] enumerableTypes)
                            && enumerableTypes[0].ClosesType(typeof(KeyValuePair<,>), out elementTypes)
                            && elementTypes[1] == typeof(object)))
                    {
                        if (elementTypes[0] == typeof(string))
                        {
                            return (JsonConverter)Activator.CreateInstance(typeof(CaseInsensitiveDictionaryJsonConverter<,>)
                                .MakeGenericType(typeToConvert, elementTypes[1]));
                        }

                        if (elementTypes[0] == typeof(Uri))
                        {
                            return (JsonConverter)Activator.CreateInstance(typeof(UriDictionarySystemTextJsonConverter<,>)
                                .MakeGenericType(typeToConvert, elementTypes[1]));
                        }
                    }
                }
            }

            if (typeToConvert.IsGenericType && !typeToConvert.IsGenericTypeDefinition)
            {
                var interfaceType = typeToConvert.GetGenericTypeDefinition();

                if (_openTypeFactory.TryGetValue(interfaceType, out var concreteType))
                {
                    Type[] arguments = typeToConvert.GetGenericArguments();

                    if (arguments.Length == 1 && !arguments[0].IsGenericParameter)
                    {
                        interfaceType = interfaceType.MakeGenericType(arguments[0]);
                        concreteType = concreteType.MakeGenericType(arguments[0]);

                        return (JsonConverter)Activator.CreateInstance(typeof(TypeMappingJsonConverter<,>).MakeGenericType(interfaceType, concreteType));
                    }
                }
            }

            if (IsConvertibleInterfaceType(typeToConvert))
            {
                return (JsonConverter)Activator.CreateInstance(
                    typeof(InterfaceJsonConverter<,>).MakeGenericType(typeToConvert, TypeMetadataCache.GetImplementationType(typeToConvert)));
            }

            throw new MassTransitException($"Unsupported type for json serialization {TypeCache.GetShortName(typeToConvert)}");
        }

        static bool IsConvertibleInterfaceType(Type typeToConvert)
        {
            if (!typeToConvert.IsInterfaceOrConcreteClass())
                return false;

            if (!MessageTypeCache.IsValidMessageType(typeToConvert))
                return false;

            if (typeToConvert.IsValueTypeOrObject())
                return false;

            foreach (var attribute in typeToConvert.GetCustomAttributes())
            {
                switch (attribute.GetType().Name)
                {
                    case "JsonDerivedTypeAttribute":
                    case "JsonPolymorphicAttribute":
                        return false;
                }
            }

            return true;
        }
    }


    static class JsonConverterFactoryExtensions
    {
        public static IDictionary<Type, Func<JsonConverter>> Add<T, TImplementation>(this IDictionary<Type, Func<JsonConverter>> dictionary)
            where T : class
            where TImplementation : class, T
        {
            dictionary.Add(typeof(T), () => new TypeMappingJsonConverter<T, TImplementation>());

            return dictionary;
        }
    }
}
