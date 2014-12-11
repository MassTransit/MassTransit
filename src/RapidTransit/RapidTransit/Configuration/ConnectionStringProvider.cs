namespace RapidTransit.Configuration
{
    using System.Text.RegularExpressions;


    public class ConnectionStringProvider :
        IConnectionStringProvider
    {
        readonly IConfigurationProvider _configurationProvider;

        public ConnectionStringProvider(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public bool TryGetConnectionString(string connectionName, out string connectionString, out string providerName,
            string serverName = null, string databaseName = null)
        {
            if (!_configurationProvider.TryGetConnectionString(connectionName, out connectionString, out providerName))
                return false;

            if (serverName != null)
                connectionString = ReplaceServerName(connectionString, serverName);

            if (databaseName != null)
                connectionString = ReplaceDatabaseName(connectionString, databaseName);

            return true;
        }

        string ReplaceServerName(string value, string serverName)
        {
            if (serverName.IndexOfAny(new[] {'.', ':', '\\'}) >= 0)
            {
                return Regex.Replace(value, @"\s*Server\s*=\s*(?<old>[^;]+)", "Server=" + serverName,
                    RegexOptions.IgnoreCase);
            }
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