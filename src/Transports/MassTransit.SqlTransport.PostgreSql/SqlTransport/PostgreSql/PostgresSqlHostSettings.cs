namespace MassTransit.SqlTransport.PostgreSql
{
    using System;
    using System.Linq;
    using Configuration;
    using Npgsql;


    public class PostgresSqlHostSettings :
        ConfigurationSqlHostSettings
    {
        readonly NpgsqlDataSource? _dataSource;
        NpgsqlConnectionStringBuilder? _builder;

        public PostgresSqlHostSettings(Uri hostAddress)
            : base(hostAddress)
        {
        }

        public PostgresSqlHostSettings(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public PostgresSqlHostSettings(NpgsqlDataSource dataSource)
        {
            if (dataSource == null)
                throw new ArgumentNullException(nameof(dataSource));

            _dataSource = dataSource;

            IsProvidedDataSource = true;

            ConnectionString = dataSource.ConnectionString;
        }

        public PostgresSqlHostSettings(SqlTransportOptions options)
        {
            var builder = PostgresSqlTransportConnection.CreateBuilder(options);

            ParseHost(builder.Host);
            if (builder.Port > 0 && builder.Port != NpgsqlConnection.DefaultPort)
                Port = options.Port;

            Database = builder.Database;
            Schema = options.Schema;

            Username = builder.Username;
            Password = builder.Password;

            _builder = builder;

            if (options.ConnectionLimit.HasValue)
                ConnectionLimit = options.ConnectionLimit.Value;

            MaintenanceEnabled = !options.DisableMaintenance;
        }

        public string? MultipleHosts { get; set; }

        /// <summary>
        /// If true, the data source was provided by the developer and should not be disposed
        /// </summary>
        public bool IsProvidedDataSource { get; private set; }

        public string? ConnectionString
        {
            set
            {
                var builder = new NpgsqlConnectionStringBuilder(value);

                ParseHost(builder.Host);
                if (builder.Port > 0 && builder.Port != NpgsqlConnection.DefaultPort)
                    Port = builder.Port;

                Database = builder.Database;

                Username = builder.Username;
                Password = builder.Password;

                Schema = builder.SearchPath ?? "transport";

                _builder = builder;
            }
        }

        public NpgsqlDataSource GetDataSource()
        {
            if (_dataSource != null)
                return _dataSource;

            var builder = _builder ??= new NpgsqlConnectionStringBuilder
            {
                Host = MultipleHosts ?? Host,
                Username = Username,
                Password = Password,
                Database = Database
            };

            if (Port.HasValue && Port.Value != NpgsqlConnection.DefaultPort)
                builder.Port = Port.Value;

            return NpgsqlDataSource.Create(builder);
        }

        public override ConnectionContextFactory CreateConnectionContextFactory(ISqlHostConfiguration hostConfiguration)
        {
            return new PostgresConnectionContextFactory(hostConfiguration);
        }

        void ParseHost(string? host)
        {
            var hostSegments = host?.Split(',');
            if (hostSegments?.Length > 1)
            {
                Host = hostSegments[0].Split(':').First().Trim();
                MultipleHosts = host!.Trim();
            }
            else
            {
                var segments = host?.Split(':');
                if (segments?.Length == 1)
                    Host = segments[0].Trim();
                else if (segments?.Length == 2)
                {
                    Host = segments[0].Trim();

                    if (int.TryParse(segments[1], out var port) && port != 0 && port != NpgsqlConnection.DefaultPort)
                        Port = port;
                }
            }
        }
    }
}
