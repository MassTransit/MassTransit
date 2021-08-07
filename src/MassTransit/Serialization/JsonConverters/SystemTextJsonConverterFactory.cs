namespace MassTransit.Serialization.JsonConverters
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Courier.Contracts;
    using Courier.InternalMessages;
    using Events;
    using Internals.Extensions;
    using Metadata;
    using Scheduling;


    public class SystemTextJsonConverterFactory :
        JsonConverterFactory
    {
        static readonly IDictionary<Type, Func<JsonConverter>> _converterFactory = new Dictionary<Type, Func<JsonConverter>>
        {
            { typeof(Fault), () => new TypeMappingJsonConverter<Fault, FaultEvent>() },
            { typeof(ReceiveFault), () => new TypeMappingJsonConverter<ReceiveFault, ReceiveFaultEvent>() },
            { typeof(ExceptionInfo), () => new TypeMappingJsonConverter<ExceptionInfo, FaultExceptionInfo>() },
            { typeof(HostInfo), () => new TypeMappingJsonConverter<HostInfo, BusHostInfo>() },
            { typeof(ScheduleMessage), () => new TypeMappingJsonConverter<ScheduleMessage, ScheduleMessageCommand>() },
            { typeof(MessageEnvelope), () => new TypeMappingJsonConverter<MessageEnvelope, JsonMessageEnvelope>() },
            { typeof(RoutingSlip), () => new TypeMappingJsonConverter<RoutingSlip, RoutingSlipImpl>() },
            { typeof(Activity), () => new TypeMappingJsonConverter<Activity, ActivityImpl>() },
            { typeof(ActivityLog), () => new TypeMappingJsonConverter<ActivityLog, ActivityLogImpl>() },
            { typeof(CompensateLog), () => new TypeMappingJsonConverter<CompensateLog, CompensateLogImpl>() },
            { typeof(ActivityException), () => new TypeMappingJsonConverter<ActivityException, ActivityExceptionImpl>() },
            { typeof(Subscription), () => new TypeMappingJsonConverter<Subscription, SubscriptionImpl>() },
            { typeof(RoutingSlipCompleted), () => new TypeMappingJsonConverter<RoutingSlipCompleted, RoutingSlipCompletedMessage>() },
            { typeof(RoutingSlipFaulted), () => new TypeMappingJsonConverter<RoutingSlipFaulted, RoutingSlipFaultedMessage>() },
            { typeof(RoutingSlipActivityCompleted), () => new TypeMappingJsonConverter<RoutingSlipActivityCompleted, RoutingSlipActivityCompletedMessage>() },
            { typeof(RoutingSlipActivityFaulted), () => new TypeMappingJsonConverter<RoutingSlipActivityFaulted, RoutingSlipActivityFaultedMessage>() },
            {
                typeof(RoutingSlipActivityCompensated),
                () => new TypeMappingJsonConverter<RoutingSlipActivityCompensated, RoutingSlipActivityCompensatedMessage>()
            },
            {
                typeof(RoutingSlipActivityCompensationFailed),
                () => new TypeMappingJsonConverter<RoutingSlipActivityCompensationFailed, RoutingSlipActivityCompensationFailedMessage>()
            },
            {
                typeof(RoutingSlipCompensationFailed), () => new TypeMappingJsonConverter<RoutingSlipCompensationFailed, RoutingSlipCompensationFailedMessage>()
            },
            { typeof(RoutingSlipTerminated), () => new TypeMappingJsonConverter<RoutingSlipTerminated, RoutingSlipTerminatedMessage>() },
            { typeof(RoutingSlipRevised), () => new TypeMappingJsonConverter<RoutingSlipRevised, RoutingSlipRevisedMessage>() },
        };

        static readonly IDictionary<Type, Type> _openTypeFactory = new Dictionary<Type, Type>
        {
            { typeof(Fault<>), typeof(FaultEvent<>) },
            { typeof(ScheduleMessage<>), typeof(ScheduleMessageCommand<>) }
        };

        public override bool CanConvert(Type typeToConvert)
        {
            var typeInfo = typeToConvert.GetTypeInfo();

            if (!typeInfo.IsInterface)
                return false;

            if (_converterFactory.TryGetValue(typeInfo, out Func<JsonConverter> _))
                return true;

            if (_openTypeFactory.TryGetValue(typeInfo, out _))
                return true;

            if (typeToConvert.IsInterfaceOrConcreteClass() && TypeMetadataCache.IsValidMessageType(typeToConvert) && !typeToConvert.IsValueTypeOrObject())
                return true;

            return false;
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var typeInfo = typeToConvert.GetTypeInfo();

            if (_converterFactory.TryGetValue(typeInfo, out Func<JsonConverter> converterFactory))
                return converterFactory();

            if (typeInfo.IsGenericType && !typeInfo.IsGenericTypeDefinition)
            {
                var interfaceType = typeInfo.GetGenericTypeDefinition();

                if (_openTypeFactory.TryGetValue(interfaceType, out var concreteType))
                {
                    Type[] arguments = typeInfo.GetGenericArguments();

                    if (arguments.Length == 1 && !arguments[0].IsGenericParameter)
                    {
                        interfaceType = interfaceType.MakeGenericType(arguments[0]);
                        concreteType = concreteType.MakeGenericType(arguments[0]);

                        return (JsonConverter)Activator.CreateInstance(typeof(TypeMappingJsonConverter<,>).MakeGenericType(interfaceType, concreteType));
                    }
                }
            }

            if (typeToConvert.IsInterfaceOrConcreteClass() && TypeMetadataCache.IsValidMessageType(typeToConvert) && !typeToConvert.IsValueTypeOrObject())
            {
                return (JsonConverter)Activator.CreateInstance(
                    typeof(InterfaceJsonConverter<,>).MakeGenericType(typeToConvert, TypeMetadataCache.GetImplementationType(typeToConvert)));
            }

            throw new MassTransitException($"Unsupported type for json serialization {TypeMetadataCache.GetShortName(typeInfo)}");
        }
    }
}
