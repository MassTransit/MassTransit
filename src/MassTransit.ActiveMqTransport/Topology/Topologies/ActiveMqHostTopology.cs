// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.ActiveMqTransport.Topology.Topologies
{
    using System;
    using System.Text;
    using Configuration;
    using MassTransit.Topology.Topologies;
    using Settings;
    using Transports;
    using Util;


    public class ActiveMqHostTopology :
        HostTopology,
        IActiveMqHostTopology
    {
        readonly Uri _hostAddress;
        readonly IMessageNameFormatter _messageNameFormatter;
        readonly IActiveMqTopologyConfiguration _topologyConfiguration;

        public ActiveMqHostTopology(IMessageNameFormatter messageNameFormatter, Uri hostAddress, IActiveMqTopologyConfiguration topologyConfiguration)
            : base(topologyConfiguration)
        {
            _messageNameFormatter = messageNameFormatter;
            _hostAddress = hostAddress;
            _topologyConfiguration = topologyConfiguration;
        }

        IActiveMqPublishTopology IActiveMqHostTopology.PublishTopology => _topologyConfiguration.Publish;
        IActiveMqSendTopology IActiveMqHostTopology.SendTopology => _topologyConfiguration.Send;

        IActiveMqMessagePublishTopology<T> IActiveMqHostTopology.Publish<T>()
        {
            return _topologyConfiguration.Publish.GetMessageTopology<T>();
        }

        IActiveMqMessageSendTopology<T> IActiveMqHostTopology.Send<T>()
        {
            return _topologyConfiguration.Send.GetMessageTopology<T>();
        }

        public SendSettings GetSendSettings(Uri address)
        {
            return _topologyConfiguration.Send.GetSendSettings(address);
        }

        public Uri GetDestinationAddress(string exchangeName, Action<ITopicConfigurator> configure = null)
        {
            var sendSettings = new TopicSendSettings(exchangeName, true, false);

            configure?.Invoke(sendSettings);

            return sendSettings.GetSendAddress(_hostAddress);
        }

        public Uri GetDestinationAddress(Type messageType, Action<ITopicConfigurator> configure = null)
        {
            var isTemporary = TypeMetadataCache.IsTemporaryMessageType(messageType);

            var durable = !isTemporary;
            var autoDelete = isTemporary;

            var name = _messageNameFormatter.GetMessageName(messageType).ToString();

            var settings = new TopicSendSettings(name, durable, autoDelete);

            configure?.Invoke(settings);

            return settings.GetSendAddress(_hostAddress);
        }

        public override string CreateTemporaryQueueName(string prefix)
        {
            var sb = new StringBuilder(prefix);

            var host = HostMetadataCache.Host;

            foreach (var c in host.MachineName)
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
                else if (c == '.' || c == '_' || c == '-' || c == ':')
                    sb.Append(c);

            sb.Append('-');
            foreach (var c in host.ProcessName)
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
                else if (c == '.' || c == '_' || c == '-' || c == ':')
                    sb.Append(c);

            sb.Append('-');
            sb.Append(NewId.Next().ToString(Formatter));

            return sb.ToString();
        }
    }
}