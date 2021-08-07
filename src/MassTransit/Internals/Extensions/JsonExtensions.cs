namespace MassTransit.Internals.Extensions
{
    using System;
    using System.Text.Json;


    public static class JsonExtensions
    {
        public static object ToObject(this JsonElement element, Type returnType, JsonSerializerOptions options = null)
        {
            var json = element.GetRawText();
            return JsonSerializer.Deserialize(json, returnType, options);
        }

        public static T ToObject<T>(this JsonElement element, JsonSerializerOptions options = null)
            where T : class
        {
            var json = element.GetRawText();
            return JsonSerializer.Deserialize<T>(json, options);
        }
    }
}
