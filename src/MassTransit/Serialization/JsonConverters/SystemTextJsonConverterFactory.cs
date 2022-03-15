namespace MassTransit.Serialization.JsonConverters
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Batching;
    using Courier.Contracts;
    using Courier.Messages;
    using Events;
    using Internals;
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
        };

        static SystemTextJsonConverterFactory()
        {
            _converterFactory = new Dictionary<Type, Func<JsonConverter>>()
                .Add<Fault, FaultEvent>()
                .Add<ReceiveFault, ReceiveFaultEvent>()
                .Add<ExceptionInfo, FaultExceptionInfo>()
                .Add<HostInfo, BusHostInfo>()
                .Add<ScheduleMessage, ScheduleMessageCommand>()
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
                .Add<RoutingSlipRevised, RoutingSlipRevisedMessage>();
        }

        public override bool CanConvert(Type typeToConvert)
        {
            var typeInfo = typeToConvert.GetTypeInfo();

            if (typeInfo.IsGenericType)
            {
                if (typeInfo.ClosesType(typeof(IDictionary<,>), out Type[] elementTypes)
                    || typeInfo.ClosesType(typeof(IReadOnlyDictionary<,>), out elementTypes)
                    || typeInfo.ClosesType(typeof(Dictionary<,>), out elementTypes)
                    || typeInfo.ClosesType(typeof(IEnumerable<>), out Type[] enumerableType)
                    && enumerableType[0].ClosesType(typeof(KeyValuePair<,>), out elementTypes))
                {
                    var keyType = elementTypes[0];
                    var valueType = elementTypes[1];

                    if (keyType != typeof(string) && keyType != typeof(Uri))
                        return false;

                    if (typeInfo.IsFSharpType())
                        return false;

                    return true;
                }
            }

            if (!typeInfo.IsInterface)
                return false;

            if (_converterFactory.TryGetValue(typeInfo, out Func<JsonConverter> _))
                return true;

            if (_openTypeFactory.TryGetValue(typeInfo, out _))
                return true;

            if (typeToConvert.IsInterfaceOrConcreteClass() && MessageTypeCache.IsValidMessageType(typeToConvert) && !typeToConvert.IsValueTypeOrObject())
                return true;

            return false;
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var typeInfo = typeToConvert.GetTypeInfo();

            if (_converterFactory.TryGetValue(typeInfo, out Func<JsonConverter> converterFactory))
                return converterFactory();

            if (typeInfo.IsGenericType)
            {
                if (!typeInfo.IsFSharpType())
                {
                    if (typeToConvert == typeof(IDictionary<string, object>))
                        return new CaseInsensitiveDictionaryStringObjectJsonConverter<IDictionary<string, object>>();
                    if (typeToConvert == typeof(Dictionary<string, object>))
                        return new CaseInsensitiveDictionaryStringObjectJsonConverter<Dictionary<string, object>>();
                    if (typeToConvert == typeof(IReadOnlyDictionary<string, object>))
                        return new CaseInsensitiveDictionaryStringObjectJsonConverter<IReadOnlyDictionary<string, object>>();
                    if (typeToConvert == typeof(IEnumerable<KeyValuePair<string, object>>))
                        return new CaseInsensitiveDictionaryStringObjectJsonConverter<IEnumerable<KeyValuePair<string, object>>>();

                    if (typeInfo.ClosesType(typeof(IDictionary<,>), out Type[] elementTypes)
                        || typeInfo.ClosesType(typeof(IReadOnlyDictionary<,>), out elementTypes)
                        || typeInfo.ClosesType(typeof(Dictionary<,>), out elementTypes)
                        || (typeInfo.ClosesType(typeof(IEnumerable<>), out Type[] enumerableTypes)
                            && enumerableTypes[0].ClosesType(typeof(KeyValuePair<,>), out elementTypes)))
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

            if (typeToConvert.IsInterfaceOrConcreteClass() && MessageTypeCache.IsValidMessageType(typeToConvert) && !typeToConvert.IsValueTypeOrObject())
            {
                return (JsonConverter)Activator.CreateInstance(
                    typeof(InterfaceJsonConverter<,>).MakeGenericType(typeToConvert, TypeMetadataCache.GetImplementationType(typeToConvert)));
            }

            throw new MassTransitException($"Unsupported type for json serialization {TypeCache.GetShortName(typeInfo)}");
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
