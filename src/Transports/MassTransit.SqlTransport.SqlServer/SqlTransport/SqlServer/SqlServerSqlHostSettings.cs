namespace MassTransit.SqlTransport.SqlServer
{
    using System;
    using System.Net;
    using System.Text;
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
            var builder = SqlServerSqlTransportConnection.CreateBuilder(options);

            ParseDataSource(builder.DataSource);

            Database = builder.InitialCatalog;
            Schema = options.Schema;

            Username = builder.UserID;
            Password = builder.Password;

            _builder = builder;

            if (options.ConnectionLimit.HasValue)
                ConnectionLimit = options.ConnectionLimit.Value;

            MaintenanceEnabled = !options.DisableMaintenance;
        }

        public string? ConnectionString
        {
            set
            {
                var builder = new SqlConnectionStringBuilder(value);

                ParseDataSource(builder.DataSource);

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
                DataSource = FormatDataSource(),
                UserID = Username,
                Password = Password,
                InitialCatalog = Database,
                TrustServerCertificate = true
            };

            return builder.ToString();
        }

        void ParseDataSource(string? source)
        {
            var split = source?.Split(',');
            if (split?.Length == 2)
            {
                ParseHost(split[0].Trim());
                if (int.TryParse(split[1].Trim(), out var port))
                    Port = port;
            }
            else
                ParseHost(source);
        }

        void ParseHost(string? host)
        {
            var hostSegments = host?.Split('\\');
            if (hostSegments?.Length == 2)
            {
                Host = TrimHost(hostSegments[0]);
                InstanceName = hostSegments[1].Trim();
            }
            else
                Host = TrimHost(host);
        }

        string? TrimHost(string? host)
        {
            if (host == null)
                return null;

            if (IPAddress.TryParse(host, out var endpoint))
                return endpoint.ToString();

            var hostSplit = host.Trim().Split(':');
            return hostSplit.Length == 1 ? hostSplit[0] : hostSplit[1];
        }

        string? FormatDataSource()
        {
            if (string.IsNullOrWhiteSpace(Host))
                return null;

            var sb = new StringBuilder();
            sb.Append(Host);
            if (!string.IsNullOrWhiteSpace(InstanceName))
                sb.Append('\\').Append(InstanceName);

            if (Port.HasValue)
                sb.Append(',').Append(Port.Value);

            return sb.ToString();
        }
    }
}
