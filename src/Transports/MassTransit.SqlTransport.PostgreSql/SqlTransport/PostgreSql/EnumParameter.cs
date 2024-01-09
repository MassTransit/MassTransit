namespace MassTransit.SqlTransport.PostgreSql
{
    using System;
    using System.Data;
    using Dapper;
    using Npgsql;


    public class EnumParameter :
        SqlMapper.ICustomQueryParameter
    {
        readonly string _dataTypeName;
        readonly string? _value;

        public EnumParameter(string? value, string dataTypeName)
        {
            _value = value;
            _dataTypeName = dataTypeName;
        }

        public void AddParameter(IDbCommand command, string name)
        {
            var parameter = new NpgsqlParameter
            {
                ParameterName = name,
                Value = _value != null ? _value : DBNull.Value,
                DataTypeName = _dataTypeName
            };

            command.Parameters.Add(parameter);
        }
    }
}
