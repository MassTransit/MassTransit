namespace MassTransit.Serialization.JsonConverters
{
    using System;
    using System.Reflection;
    using Courier.Contracts;
    using Courier.InternalMessages;
    using Events;
    using Metadata;
    using Newtonsoft.Json;
    using Scheduling;


    public class InternalTypeConverter :
        BaseJsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        protected override IConverter ValueFactory(Type objectType)
        {
            var typeInfo = objectType.GetTypeInfo();

            if (typeInfo.IsInterface)
            {
                if (objectType == typeof(Fault))
                    return new CachedConverter<FaultEvent>();
                if (objectType == typeof(ReceiveFault))
                    return new CachedConverter<ReceiveFaultEvent>();
                if (objectType == typeof(ExceptionInfo))
                    return new CachedConverter<FaultExceptionInfo>();
                if (objectType == typeof(HostInfo))
                    return new CachedConverter<BusHostInfo>();
                if (objectType == typeof(ScheduleMessage))
                    return new CachedConverter<ScheduleMessageCommand>();
                if (objectType == typeof(MessageEnvelope))
                    return new CachedConverter<JsonMessageEnvelope>();

                if (objectType == typeof(RoutingSlip))
                    return new CachedConverter<RoutingSlipImpl>();
                if (objectType == typeof(Activity))
                    return new CachedConverter<ActivityImpl>();
                if (objectType == typeof(ActivityLog))
                    return new CachedConverter<ActivityLogImpl>();
                if (objectType == typeof(CompensateLog))
                    return new CachedConverter<CompensateLogImpl>();
                if (objectType == typeof(ActivityException))
                    return new CachedConverter<ActivityExceptionImpl>();
                if (objectType == typeof(Subscription))
                    return new CachedConverter<SubscriptionImpl>();

                if (objectType == typeof(RoutingSlipCompleted))
                    return new CachedConverter<RoutingSlipCompletedMessage>();
                if (objectType == typeof(RoutingSlipFaulted))
                    return new CachedConverter<RoutingSlipFaultedMessage>();
                if (objectType == typeof(RoutingSlipActivityCompleted))
                    return new CachedConverter<RoutingSlipActivityCompletedMessage>();
                if (objectType == typeof(RoutingSlipActivityFaulted))
                    return new CachedConverter<RoutingSlipActivityFaultedMessage>();
                if (objectType == typeof(RoutingSlipActivityCompensated))
                    return new CachedConverter<RoutingSlipActivityCompensatedMessage>();
                if (objectType == typeof(RoutingSlipActivityCompensationFailed))
                    return new CachedConverter<RoutingSlipActivityCompensationFailedMessage>();
                if (objectType == typeof(RoutingSlipTerminated))
                    return new CachedConverter<RoutingSlipTerminatedMessage>();
                if (objectType == typeof(RoutingSlipRevised))
                    return new CachedConverter<RoutingSlipRevisedMessage>();

                if (typeInfo.IsGenericType && !typeInfo.IsGenericTypeDefinition)
                {
                    if (typeInfo.GetGenericTypeDefinition() == typeof(Fault<>))
                    {
                        Type[] arguments = typeInfo.GetGenericArguments();
                        if (arguments.Length == 1 && !arguments[0].IsGenericParameter)
                        {
                            return (IConverter)Activator.CreateInstance(typeof(CachedConverter<>).MakeGenericType(
                                typeof(FaultEvent<>).MakeGenericType(arguments[0])));
                        }
                    }
                    else if (typeInfo.GetGenericTypeDefinition() == typeof(ScheduleMessage<>))
                    {
                        Type[] arguments = typeInfo.GetGenericArguments();
                        if (arguments.Length == 1 && !arguments[0].IsGenericParameter)
                        {
                            return (IConverter)Activator.CreateInstance(typeof(CachedConverter<>).MakeGenericType(
                                typeof(ScheduleMessageCommand<>).MakeGenericType(arguments[0])));
                        }
                    }
                }
            }

            return new Unsupported();
        }


        class CachedConverter<T> :
            IConverter
        {
            object IConverter.Deserialize(JsonReader reader, Type objectType, JsonSerializer serializer)
            {
                return serializer.Deserialize(reader, typeof(T));
            }

            public bool IsSupported => true;
        }
    }
}
