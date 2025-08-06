namespace MassTransit.Persistence.SqlServer.Extensions
{
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Microsoft.Data.SqlClient;


    public static class ObjectExtensions
    {
        static readonly JsonSerializerOptions SerializerOptions = new()
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            IncludeFields = false,
            PreferredObjectCreationHandling = JsonObjectCreationHandling.Populate
        };

        public static TObj? FromJson<TObj>(this SqlDataReader reader, string columnName)
        {
            var colIndex = reader.GetOrdinal(columnName);

            if (reader.IsDBNull(colIndex))
                return default;

            var value = reader.GetString(colIndex);

            return JsonSerializer.Deserialize<TObj>(
                Encoding.UTF8.GetBytes(value), SerializerOptions
            );
        }

        public static string? ToJson(this object? input)
        {
            if (input is null)
                return null;

            return Encoding.UTF8.GetString(
                JsonSerializer.SerializeToUtf8Bytes(input, SerializerOptions)
            );
        }

        public static object OrDbNull(this object? input)
        {
            return input ?? DBNull.Value;
        }
    }
}
