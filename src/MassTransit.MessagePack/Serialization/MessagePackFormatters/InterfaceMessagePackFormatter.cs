namespace MassTransit.Serialization.MessagePackFormatters;

using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Internals;
using MessagePack;
using MessagePack.Formatters;
using Metadata;


delegate void SerializeDelegate<in TConcrete>(ref MessagePackWriter writer, TConcrete value, MessagePackSerializerOptions options);


delegate TConcrete DeserializeDelegate<out TConcrete>(ref MessagePackReader reader, MessagePackSerializerOptions options);


public class InterfaceMessagePackFormatter<TInterface> :
    IMessagePackFormatter<TInterface>
{
    static readonly FormatterProxyInfo _formatterProxyInfo;

    static InterfaceMessagePackFormatter()
    {
        var proxyType = TypeMetadataCache.GetImplementationType(typeof(TInterface));
        _formatterProxyInfo = GetFormatterProxyInfoFromType(proxyType);
    }

    public void Serialize(ref MessagePackWriter writer, TInterface value, MessagePackSerializerOptions options)
    {
        FormatterProxyInfo formatterProxyInfoToUse;

        var typeOfValue = value?.GetType();

        // If the value is not null and not an interface, use the formatter for the concrete type.
        if (typeOfValue != null && !typeOfValue.IsInterface)
            formatterProxyInfoToUse = GetFormatterProxyInfoFromType(typeOfValue);
        else
            formatterProxyInfoToUse = _formatterProxyInfo;


        // IMessagePackFormatter of unknown type
        var formatter = formatterProxyInfoToUse.GetFormatterMethodInfo.Invoke(options.Resolver, BindingFlags.Default, null, null, null);

        // Call Serialize method of IMessagePackFormatter
        var writerParameter = Expression.Parameter(typeof(MessagePackWriter).MakeByRefType(), "writer");
        var valueParameterForCall = Expression.Parameter(formatterProxyInfoToUse.TargetType, "value");
        var optionsParameter = Expression.Parameter(typeof(MessagePackSerializerOptions), "options");

        var formatterInstance = Expression.Constant(formatter);
        var call = Expression.Call(formatterInstance, formatterProxyInfoToUse.SerializeMethodInfo, writerParameter, valueParameterForCall, optionsParameter);

        var delegateType = typeof(SerializeDelegate<>).MakeGenericType(formatterProxyInfoToUse.TargetType);

        var proxyFuncDelegate = Expression
            .Lambda(delegateType, call, writerParameter, valueParameterForCall, optionsParameter)
            .CompileFast();

        var proxyFunc = Unsafe.As<SerializeDelegate<TInterface>>(proxyFuncDelegate);

        proxyFunc(ref writer, value, options);
    }

    public TInterface Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        var formatter = _formatterProxyInfo.GetFormatterMethodInfo.Invoke(options.Resolver, BindingFlags.Default, null, null, null);

        var readerParameter = Expression.Parameter(typeof(MessagePackReader).MakeByRefType(), "reader");
        var optionsParameter = Expression.Parameter(typeof(MessagePackSerializerOptions), "options");

        var formatterInstance = Expression.Constant(formatter);
        var call = Expression.Call(formatterInstance, _formatterProxyInfo.DeserializeMethodInfo, readerParameter, optionsParameter);
        DeserializeDelegate<TInterface>? proxyFunc = Expression
            .Lambda<DeserializeDelegate<TInterface>>(call, readerParameter, optionsParameter)
            .CompileFast();

        return proxyFunc(ref reader, options);
    }

    static FormatterProxyInfo GetFormatterProxyInfoFromType(Type targetType)
    {
        // The null-forgiving operator (!) is used because the methods are guaranteed to exist,
        // during normal operation. In case it doesn't, we likely won't even get here.

        var getFormatterMethodInfo = typeof(IFormatterResolver)
            .GetMethod(nameof(IFormatterResolver.GetFormatter))!
            .MakeGenericMethod(targetType);

        var formatterType = typeof(IMessagePackFormatter<>)
            .MakeGenericType(targetType);

        var serializeMethodInfo = formatterType
            .GetMethod(nameof(IMessagePackFormatter<object>.Serialize))!;

        var deserializeMethodInfo = formatterType
            .GetMethod(nameof(IMessagePackFormatter<object>.Deserialize))!;

        return new FormatterProxyInfo
        {
            TargetType = targetType,
            GetFormatterMethodInfo = getFormatterMethodInfo,
            SerializeMethodInfo = serializeMethodInfo,
            DeserializeMethodInfo = deserializeMethodInfo
        };
    }


    struct FormatterProxyInfo
    {
        public Type TargetType { get; set; }
        public MethodInfo GetFormatterMethodInfo { get; set; }
        public MethodInfo SerializeMethodInfo { get; set; }
        public MethodInfo DeserializeMethodInfo { get; set; }
    }
}
