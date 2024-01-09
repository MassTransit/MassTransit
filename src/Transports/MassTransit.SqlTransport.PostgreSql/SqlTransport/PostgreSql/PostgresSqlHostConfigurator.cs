namespace MassTransit.SqlTransport.PostgreSql
{
    using System;
    using Configuration;


    public class PostgresSqlHostConfigurator :
        SqlHostConfigurator,
        IPostgresSqlHostConfigurator
    {
        readonly PostgresSqlHostSettings _settings;

        public PostgresSqlHostConfigurator(PostgresSqlHostSettings settings)
            : base(settings)
        {
            _settings = settings;
        }

        public PostgresSqlHostConfigurator(Uri hostAddress)
            : this(new PostgresSqlHostSettings(hostAddress))
        {
        }

        public PostgresSqlHostConfigurator(SqlTransportOptions options)
            : this(new PostgresSqlHostSettings(options))
        {
        }

        public PostgresSqlHostConfigurator(string connectionString)
            : this(new PostgresSqlHostSettings(connectionString))
        {
        }

        public SqlHostSettings Settings => _settings;

        public override string? ConnectionString
        {
            set => _settings.ConnectionString = value;
        }
    }
}
