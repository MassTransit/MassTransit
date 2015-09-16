// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Host.Configuration
{
    using System.Text.RegularExpressions;
    using Hosting;


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