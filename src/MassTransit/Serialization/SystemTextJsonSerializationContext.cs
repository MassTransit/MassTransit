#if NET8_0_OR_GREATER
    namespace MassTransit.Serialization
    {
        using System.Text.Json;
        using System.Text.Json.Serialization;
        using Courier.Contracts;
        using Courier.Messages;
        using Events;
        using JsonConverters;
        using Metadata;
        using Scheduling;


        [JsonSerializable(typeof(Fault))]
        [JsonSerializable(typeof(FaultEvent))]
        // [JsonSerializable(typeof(FaultEvent<>))]
        [JsonSerializable(typeof(FaultEvent<RoutingSlip>))]
        [JsonSerializable(typeof(ReceiveFault))]
        [JsonSerializable(typeof(ReceiveFaultEvent))]
        [JsonSerializable(typeof(ExceptionInfo))]
        [JsonSerializable(typeof(FaultExceptionInfo))]
        [JsonSerializable(typeof(HostInfo))]
        [JsonSerializable(typeof(BusHostInfo))]
        // [JsonSerializable(typeof(MessageBatch<>))]
        [JsonSerializable(typeof(ScheduleMessage))]
        [JsonSerializable(typeof(ScheduleMessageCommand))]
        // [JsonSerializable(typeof(ScheduleMessageCommand<>))]
        [JsonSerializable(typeof(ScheduleRecurringMessage))]
        [JsonSerializable(typeof(ScheduleRecurringMessageCommand))]
        // [JsonSerializable(typeof(ScheduleRecurringMessageCommand<>))]
        [JsonSerializable(typeof(CancelScheduledMessage))]
        [JsonSerializable(typeof(CancelScheduledRecurringMessage))]
        [JsonSerializable(typeof(CancelScheduledRecurringMessageCommand))]
        [JsonSerializable(typeof(PauseScheduledRecurringMessage))]
        [JsonSerializable(typeof(PauseScheduledRecurringMessageCommand))]
        [JsonSerializable(typeof(ResumeScheduledRecurringMessage))]
        [JsonSerializable(typeof(ResumeScheduledRecurringMessageCommand))]
        [JsonSerializable(typeof(MessageEnvelope))]
        [JsonSerializable(typeof(JsonElement))]
        [JsonSerializable(typeof(JsonMessageEnvelope))]
        [JsonSerializable(typeof(RoutingSlip))]
        [JsonSerializable(typeof(RoutingSlipRoutingSlip))]
        [JsonSerializable(typeof(Activity))]
        [JsonSerializable(typeof(RoutingSlipActivity))]
        [JsonSerializable(typeof(ActivityLog))]
        [JsonSerializable(typeof(RoutingSlipActivityLog))]
        [JsonSerializable(typeof(CompensateLog))]
        [JsonSerializable(typeof(RoutingSlipCompensateLog))]
        [JsonSerializable(typeof(ActivityException))]
        [JsonSerializable(typeof(RoutingSlipActivityException))]
        [JsonSerializable(typeof(Subscription))]
        [JsonSerializable(typeof(RoutingSlipSubscription))]
        [JsonSerializable(typeof(RoutingSlipCompleted))]
        [JsonSerializable(typeof(RoutingSlipCompletedMessage))]
        [JsonSerializable(typeof(RoutingSlipFaulted))]
        [JsonSerializable(typeof(RoutingSlipFaultedMessage))]
        [JsonSerializable(typeof(RoutingSlipActivityCompleted))]
        [JsonSerializable(typeof(RoutingSlipActivityCompletedMessage))]
        [JsonSerializable(typeof(RoutingSlipActivityFaulted))]
        [JsonSerializable(typeof(RoutingSlipActivityFaultedMessage))]
        [JsonSerializable(typeof(RoutingSlipActivityCompensated))]
        [JsonSerializable(typeof(RoutingSlipActivityCompensatedMessage))]
        [JsonSerializable(typeof(RoutingSlipActivityCompensationFailed))]
        [JsonSerializable(typeof(RoutingSlipActivityCompensationFailedMessage))]
        [JsonSerializable(typeof(RoutingSlipCompensationFailed))]
        [JsonSerializable(typeof(RoutingSlipCompensationFailedMessage))]
        [JsonSerializable(typeof(RoutingSlipTerminated))]
        [JsonSerializable(typeof(RoutingSlipTerminatedMessage))]
        [JsonSerializable(typeof(RoutingSlipRevised))]
        [JsonSerializable(typeof(RoutingSlipRevisedMessage))]
        [JsonSerializable(typeof(SystemTextMessageDataReference))]
        partial class SystemTextJsonSerializationContext :
            JsonSerializerContext
        {
        }
    }
#endif
