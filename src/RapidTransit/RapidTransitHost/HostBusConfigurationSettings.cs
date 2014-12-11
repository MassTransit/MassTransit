// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace RapidTransit
{
    using MassTransit.Util;
    using RapidTransit.Configuration;
    using Topshelf.Runtime;


    /// <summary>
    /// Registered in the container to implement the management bus settings
    /// </summary>
    public class HostBusConfigurationSettings :
        HostBusSettings
    {
        const string QueueNameKey = "HostBusQueueName";

        readonly string _baseQueueName;

        public HostBusConfigurationSettings(IConfigurationProvider configurationProvider, HostSettings hostSettings)
        {
            _baseQueueName = GetBaseQueueName(configurationProvider, hostSettings);
        }

        string HostBusSettings.QueueName
        {
            get { return string.Format("{0}_{1}", _baseQueueName, HostMetadataCache.Host.MachineName.ToLowerInvariant()); }
        }

        static string GetBaseQueueName(IConfigurationProvider configurationProvider, HostSettings hostSettings)
        {
            string baseQueueName;
            if (configurationProvider.TryGetSetting(QueueNameKey, out baseQueueName))
                return baseQueueName;

            return hostSettings.ServiceName.Replace(" ", "_");
        }
    }
}