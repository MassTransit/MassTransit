namespace MassTransit.Util
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using Serialization;


    public static class ConvertObject
    {
        public static Dictionary<string, object> ToDictionary(object values)
        {
            return values == null
                ? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                : JsonSerializer.SerializeToElement(values, SystemTextJsonMessageSerializer.Options).Deserialize<Dictionary<string, object>>();
        }

        public static Dictionary<string, object> ToDictionary(object values, JsonSerializerOptions options)
        {
            return values == null
                ? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                : JsonSerializer.SerializeToElement(values, options).Deserialize<Dictionary<string, object>>();
        }
    }
}
