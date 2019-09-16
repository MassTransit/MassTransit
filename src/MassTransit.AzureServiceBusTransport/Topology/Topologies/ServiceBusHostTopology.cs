// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport.Topology.Topologies
{
    using System.Text;
    using AzureServiceBusTransport.Configuration;
    using MassTransit.Topology.Topologies;
    using Metadata;
    using Specifications;
    using Util;


    public class ServiceBusHostTopology :
        HostTopology,
        IServiceBusHostTopology
    {
        readonly IServiceBusTopologyConfiguration _configuration;

        public ServiceBusHostTopology(IServiceBusTopologyConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration;
        }

        IServiceBusPublishTopology IServiceBusHostTopology.PublishTopology => _configuration.Publish;
        IServiceBusSendTopology IServiceBusHostTopology.SendTopology => _configuration.Send;

        public override string CreateTemporaryQueueName(string prefix)
        {
            var sb = new StringBuilder();

            var host = HostMetadataCache.Host;

            foreach (var c in host.MachineName)
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
                else if (c == '_')
                    sb.Append(c);
            sb.Append('_');
            foreach (var c in host.ProcessName)
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
                else if (c == '_')
                    sb.Append(c);
            sb.AppendFormat("_{0}_", prefix);
            sb.Append(NewId.Next().ToString(Formatter));

            return sb.ToString();
        }

        IServiceBusMessagePublishTopology<T> IServiceBusHostTopology.Publish<T>()
        {
            return _configuration.Publish.GetMessageTopology<T>();
        }

        IServiceBusMessageSendTopology<T> IServiceBusHostTopology.Send<T>()
        {
            return _configuration.Send.GetMessageTopology<T>();
        }
    }
}