namespace MassTransit.SqlTransport.PostgreSql
{
    using System;
    using System.Data;
    using Dapper;
    using Npgsql;
    using NpgsqlTypes;


    public class JsonParameter :
        SqlMapper.ICustomQueryParameter
    {
        readonly string? _value;

        public JsonParameter(string? value)
        {
            _value = value;
        }

        public void AddParameter(IDbCommand command, string name)
        {
            var parameter = new NpgsqlParameter(name, NpgsqlDbType.Jsonb) { Value = _value != null ? _value : DBNull.Value };

            command.Parameters.Add(parameter);
        }
    }
}
