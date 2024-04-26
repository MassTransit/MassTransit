namespace MassTransit.SqlTransport.PostgreSql
{
    using System;
    using Configuration;
    using Npgsql;


    public class PostgresSqlHostSettings :
        ConfigurationSqlHostSettings
    {
        NpgsqlConnectionStringBuilder? _builder;

        public PostgresSqlHostSettings(Uri hostAddress)
            : base(hostAddress)
        {
        }

        public PostgresSqlHostSettings(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public PostgresSqlHostSettings(SqlTransportOptions options)
        {
            var builder = PostgresSqlTransportConnection.CreateBuilder(options);

            Host = builder.Host;
            if (builder.Port > 0 && builder.Port != NpgsqlConnection.DefaultPort)
                Port = options.Port;

            Database = builder.Database;
            Schema = options.Schema;

            Username = builder.Username;
            Password = builder.Password;

            _builder = builder;
        }

        public string? ConnectionString
        {
            set
            {
                var builder = new NpgsqlConnectionStringBuilder(value);

                Host = builder.Host;
                if (builder.Port > 0 && builder.Port != NpgsqlConnection.DefaultPort)
                    Port = builder.Port;

                Database = builder.Database;

                Username = builder.Username;
                Password = builder.Password;

                _builder = builder;
            }
        }

        public override ConnectionContextFactory CreateConnectionContextFactory(ISqlHostConfiguration hostConfiguration)
        {
            return new PostgresConnectionContextFactory(hostConfiguration);
        }

        public string GetConnectionString()
        {
            var builder = _builder ??= new NpgsqlConnectionStringBuilder
            {
                Host = Host,
                Username = Username,
                Password = Password,
                Database = Database
            };

            if (Port.HasValue)
                builder.Port = Port.Value;

            return builder.ToString();
        }
    }
}
