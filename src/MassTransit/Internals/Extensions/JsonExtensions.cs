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
    }
}
