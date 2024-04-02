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
            var address = new SqlHostAddress(hostAddress);

            Host = address.Host;
            InstanceName = address.InstanceName;
        }

        public SqlServerSqlHostSettings(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public SqlServerSqlHostSettings(SqlTransportOptions options)
        {
            var hostSegments = options.Host?.Split('\\');
            if (hostSegments?.Length == 2)
            {
                Host = hostSegments[0].Trim();
                InstanceName = hostSegments[1].Trim();
            }
            else
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

                var split = builder.DataSource.Split(',');
                if (split.Length == 2)
                {
                    Host = split[0].Trim();
                    if (int.TryParse(split[1].Trim(), out var port))
                        Port = port;
                }
                else
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
