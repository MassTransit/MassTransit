namespace MassTransit.SqlTransport.SqlServer
{
    using System;
    using Configuration;
    using Microsoft.Data.SqlClient;


    public class SqlServerSqlHostSettings :
        ConfigurationSqlHostSettings
    {
        SqlConnectionStringBuilder? _builder;

        public SqlServerSqlHostSettings(Uri hostAddress)
            : base(hostAddress)
        {
        }

        public SqlServerSqlHostSettings(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public SqlServerSqlHostSettings(SqlTransportOptions options)
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
                var builder = new SqlConnectionStringBuilder(value);

                Host = builder.DataSource;

                Username = builder.UserID;
                Password = builder.Password;

                Database = builder.InitialCatalog;

                _builder = builder;
            }
        }

        public override ConnectionContextFactory CreateConnectionContextFactory(ISqlHostConfiguration hostConfiguration)
        {
            return new SqlServerConnectionContextFactory(hostConfiguration);
        }

        public string GetConnectionString()
        {
            var builder = _builder ??= new SqlConnectionStringBuilder
            {
                DataSource = Host,
                UserID = Username,
                Password = Password,
                InitialCatalog = Database,
                TrustServerCertificate = true
            };

            return builder.ToString();
        }
    }
}
