namespace MassTransit.Serialization
{
    using MassTransit.Serialization.JsonConverters;
    using System.Text.Json;

    internal static class SystemTextJsonConfiguration
    {
        internal static JsonSerializerOptions Options;

        static SystemTextJsonConfiguration()
        {
            Options = new JsonSerializerOptions()
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
                WriteIndented = true,
            };

            Options.Converters.Add(new SystemTextJsonConverterFactory());
        }
    }
}
