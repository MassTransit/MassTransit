namespace MassTransit.SqlTransport.PostgreSql
{
    using System;
    using System.Data;
    using Dapper;


    public class UriTypeHandler : SqlMapper.TypeHandler<Uri>
    {
        public override void SetValue(IDbDataParameter parameter, Uri? value)
        {
            parameter.DbType = DbType.String;
            parameter.Value = value != null ? value.ToString() : DBNull.Value;
        }

        public override Uri Parse(object value)
        {
            if (value is string text && !string.IsNullOrWhiteSpace(text))
                return new Uri(text);

            return null!;
        }
    }
}
