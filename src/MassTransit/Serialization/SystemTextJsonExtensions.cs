#nullable enable
namespace MassTransit.Serialization;

using System;
using System.Text.Json;
using Metadata;


public static class SystemTextJsonExtensions
{
    public static T? GetObject<T>(this JsonElement jsonElement, JsonSerializerOptions options)
        where T : class
    {
        if (typeof(T).IsInterface && MessageTypeCache<T>.IsValidMessageType)
        {
            var messageType = TypeMetadataCache<T>.ImplementationType;

            if (jsonElement.Deserialize(messageType, options) is T obj)
                return obj;
        }

        return jsonElement.Deserialize<T>(options);
    }

    public static T? Transform<T>(this object objectToTransform, JsonSerializerOptions options)
        where T : class
    {
        var jsonElement = JsonSerializer.SerializeToElement(objectToTransform, options);

        return jsonElement.GetObject<T>(options);
    }

    public static object? Transform(this object objectToTransform, Type targetType, JsonSerializerOptions options)
    {
        var jsonElement = JsonSerializer.SerializeToElement(objectToTransform, options);

        if (targetType.IsInterface && MessageTypeCache.IsValidMessageType(targetType))
            targetType = TypeMetadataCache.GetImplementationType(targetType);

        return jsonElement.Deserialize(targetType, options);
    }
}
