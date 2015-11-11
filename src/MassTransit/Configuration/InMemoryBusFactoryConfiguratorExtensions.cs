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
namespace MassTransit
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;
    using EndpointConfigurators;
    using NewIdFormatters;
    using Util;


    public static class InMemoryBusFactoryConfiguratorExtensions
    {
        static readonly INewIdFormatter _formatter = new ZBase32Formatter();
        static readonly Regex _regex = new Regex(@"^[A-Za-z0-9\-_\.:]+$");

        static string GetTemporaryQueueName(this HostInfo host, string prefix)
        {
            var sb = new StringBuilder(prefix);

            foreach (char c in host.MachineName)
            {
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
                else if (c == '.' || c == '_' || c == '-' || c == ':')
                    sb.Append(c);
            }
            sb.Append('-');
            foreach (char c in host.ProcessName)
            {
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
                else if (c == '.' || c == '_' || c == '-' || c == ':')
                    sb.Append(c);
            }
            sb.Append('-');
            sb.Append(NewId.Next().ToString(_formatter));

            return sb.ToString();
        }

        public static IManagementEndpointConfigurator ManagementEndpoint(this IInMemoryBusFactoryConfigurator configurator,
            Action<IReceiveEndpointConfigurator> configure = null)
        {
            var queueName = HostMetadataCache.Host.GetTemporaryQueueName("manage-");

            var endpointConfigurator = new InMemoryReceiveEndpointConfigurator(queueName)
            {
                TransportConcurrencyLimit = 1
            };

            configure?.Invoke(endpointConfigurator);

            configurator.AddBusFactorySpecification(endpointConfigurator);

            var managementEndpointConfigurator = new ManagementEndpointConfigurator(endpointConfigurator);

            return managementEndpointConfigurator;
        }
    }
}