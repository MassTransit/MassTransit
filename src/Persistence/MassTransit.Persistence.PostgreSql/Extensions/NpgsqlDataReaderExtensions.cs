namespace MassTransit.Persistence.PostgreSql.Extensions
{
    using System.Data;
    using Npgsql;


    public static class NpgsqlDataReaderExtensions
    {
#if !NET8_0_OR_GREATER
        public static bool IsDBNull(this NpgsqlDataReader reader, string columnName)
            => reader.IsDBNull(reader.GetOrdinal(columnName));

        public static Guid GetGuid(this NpgsqlDataReader reader, string columnName)
            => reader.GetGuid(reader.GetOrdinal(columnName));
        
        public static byte GetByte(this NpgsqlDataReader reader, string columnName)
            => reader.GetByte(reader.GetOrdinal(columnName));

        public static int GetInt32(this NpgsqlDataReader reader, string columnName)
            => reader.GetInt32(reader.GetOrdinal(columnName));

        public static long GetInt64(this NpgsqlDataReader reader, string columnName)
            => reader.GetInt64(reader.GetOrdinal(columnName));

        public static short GetInt16(this NpgsqlDataReader reader, string columnName)
            => reader.GetInt16(reader.GetOrdinal(columnName));

        public static decimal GetDecimal(this NpgsqlDataReader reader, string columnName)
            => reader.GetDecimal(reader.GetOrdinal(columnName));

        public static float GetFloat(this NpgsqlDataReader reader, string columnName)
            => reader.GetFloat(reader.GetOrdinal(columnName));

        public static double GetDouble(this NpgsqlDataReader reader, string columnName)
            => reader.GetDouble(reader.GetOrdinal(columnName));

        public static DateTime GetDateTime(this NpgsqlDataReader reader, string columnName)
            => reader.GetDateTime(reader.GetOrdinal(columnName));
        
        public static string GetString(this NpgsqlDataReader reader, string columnName)
            => reader.GetString(reader.GetOrdinal(columnName));

        public static Stream GetStream(this NpgsqlDataReader reader, string columnName)
            => reader.GetStream(reader.GetOrdinal(columnName));

#endif
        public static Guid? GetGuidOrNull(this NpgsqlDataReader reader, string columnName)
            => reader.IsDBNull(columnName) ? null : reader.GetGuid(columnName);

        public static string? GetStringOrNull(this NpgsqlDataReader reader, string columnName)
            => reader.IsDBNull(columnName) ? null : reader.GetString(columnName);

        public static byte? GetByteOrNull(this NpgsqlDataReader reader, string columnName)
            => reader.IsDBNull(columnName) ? null : reader.GetByte(columnName);

        public static int? GetInt32OrNull(this NpgsqlDataReader reader, string columnName)
            => reader.IsDBNull(columnName) ? null : reader.GetInt32(columnName);

        public static long? GetInt64OrNull(this NpgsqlDataReader reader, string columnName)
            => reader.IsDBNull(columnName) ? null : reader.GetInt64(columnName);

        public static short? GetInt16OrNull(this NpgsqlDataReader reader, string columnName)
            => reader.IsDBNull(columnName) ? null : reader.GetInt16(columnName);

        public static decimal? GetDecimalOrNull(this NpgsqlDataReader reader, string columnName)
            => reader.IsDBNull(columnName) ? null : reader.GetDecimal(columnName);

        public static float? GetFloatOrNull(this NpgsqlDataReader reader, string columnName)
            => reader.IsDBNull(columnName) ? null : reader.GetFloat(columnName);

        public static double? GetDoubleOrNull(this NpgsqlDataReader reader, string columnName)
            => reader.IsDBNull(columnName) ? null : reader.GetDouble(columnName);

        public static DateTime? GetDateTimeOrNull(this NpgsqlDataReader reader, string columnName)
            => reader.IsDBNull(columnName) ? null : reader.GetDateTime(columnName);

        public static DateTime? GetDateTimeOffsetOrNull(this NpgsqlDataReader reader, string columnName)
            => reader.IsDBNull(columnName) ? null : reader.GetDateTime(columnName);

        public static TimeSpan? GetTimeSpanOrNull(this NpgsqlDataReader reader, string columnName)
            => reader.IsDBNull(columnName) ? null : reader.GetTimeSpan(reader.GetOrdinal(columnName));

        public static Uri? GetUri(this NpgsqlDataReader reader, string columnName)
            => reader.IsDBNull(columnName) ? null : new Uri(reader.GetString(columnName));
    }
}
