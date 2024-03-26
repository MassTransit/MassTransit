namespace MassTransit.Licensing
{
    using System.Text.Json;


    public static class LicenseSettings
    {
        public static readonly JsonSerializerOptions SerializerOptions;

        static LicenseSettings()
        {
            SerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }
    }
}
