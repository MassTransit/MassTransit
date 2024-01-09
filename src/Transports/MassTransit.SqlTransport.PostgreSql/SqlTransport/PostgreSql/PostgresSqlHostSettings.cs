namespace MassTransit.SqlTransport.PostgreSql
{
    using System;
    using Configuration;
    using MassTransit.SqlTransport;
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
            Host = options.Host;
            Database = options.Database;
            Username = options.Username;
            Password = options.Password;
            Schema = options.Schema;

            if (options.Port.HasValue)
                Port = options.Port.Value;
        }

        public string? Role { get; set; }

        public string? AdminUsername { get; set; }
        public string? AdminPassword { get; set; }

        public string? ConnectionString
        {
            set
            {
                var builder = new NpgsqlConnectionStringBuilder(value);

                Host = builder.Host;
                if (builder.Port > 0)
                    Port = builder.Port;

                Username = builder.Username;
                Password = builder.Password;

                Database = builder.Database;

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
