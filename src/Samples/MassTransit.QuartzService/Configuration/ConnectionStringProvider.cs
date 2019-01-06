namespace MassTransit.QuartzService.Configuration
{
    using System.Text.RegularExpressions;


    public class ConnectionStringProvider :
        IConnectionStringProvider
    {
        IConfigurationProvider _configurationProvider;

        public ConnectionStringProvider(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public string GetConnectionString(string connectionName, string serverName = null, string databaseName = null)
        {
            string connectionString = _configurationProvider.GetConnectionString(connectionName);
            if (connectionString == null)
                return null;

            if (serverName != null)
                connectionString = ReplaceServerName(connectionString, serverName);

            if (databaseName != null)
                connectionString = ReplaceDatabaseName(connectionString, databaseName);

            return connectionString;
        }

        string ReplaceServerName(string value, string serverName)
        {
            if (serverName.IndexOfAny(new[] {'.', ':', '\\'}) >= 0)
                return Regex.Replace(value, @"\s*Server\s*=\s*(?<old>[^;]+)", "Server=" + serverName,
                    RegexOptions.IgnoreCase);
            return Regex.Replace(value, @"\s*Server\s*=\s*(?<old>[^;\.:]+)", "Server=" + serverName,
                RegexOptions.IgnoreCase);
        }

        string ReplaceDatabaseName(string value, string databaseName)
        {
            return Regex.Replace(value, @"\s*Database\s*=\s*(?<old>[^;]+)", "Database=" + databaseName,
                RegexOptions.IgnoreCase);
        }
    }
}