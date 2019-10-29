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
namespace MassTransit.AmazonSqsTransport.Topology.Topologies
{
    using System;
    using System.Text;
    using AmazonSqsTransport.Configuration;
    using AmazonSqsTransport.Configuration.Configuration;
    using MassTransit.Topology.Topologies;
    using Metadata;
    using Settings;
    using Transports;


    public class AmazonSqsHostTopology :
        HostTopology,
        IAmazonSqsHostTopology
    {
        readonly Uri _hostAddress;
        readonly IMessageNameFormatter _messageNameFormatter;
        readonly IAmazonSqsTopologyConfiguration _configuration;

        public AmazonSqsHostTopology(IMessageNameFormatter messageNameFormatter, Uri hostAddress, IAmazonSqsTopologyConfiguration configuration)
            : base(configuration)
        {
            _messageNameFormatter = messageNameFormatter;
            _hostAddress = hostAddress;
            _configuration = configuration;
        }

        IAmazonSqsPublishTopology IAmazonSqsHostTopology.PublishTopology => _configuration.Publish;
        IAmazonSqsSendTopology IAmazonSqsHostTopology.SendTopology => _configuration.Send;

        IAmazonSqsMessagePublishTopology<T> IAmazonSqsHostTopology.Publish<T>()
        {
            return _configuration.Publish.GetMessageTopology<T>();
        }

        IAmazonSqsMessageSendTopology<T> IAmazonSqsHostTopology.Send<T>()
        {
            return _configuration.Send.GetMessageTopology<T>();
        }

        public SendSettings GetSendSettings(Uri address)
        {
            var endpointAddress = new AmazonSqsEndpointAddress(_hostAddress, address);

            return _configuration.Send.GetSendSettings(endpointAddress);
        }

        public Uri GetDestinationAddress(string topicName, Action<ITopicConfigurator> configure = null)
        {
            var sendSettings = new TopicPublishSettings(topicName, true, false);

            configure?.Invoke(sendSettings);

            return sendSettings.GetSendAddress(_hostAddress);
        }

        public Uri GetDestinationAddress(Type messageType, Action<ITopicConfigurator> configure = null)
        {
            var isTemporary = TypeMetadataCache.IsTemporaryMessageType(messageType);

            var durable = !isTemporary;
            var autoDelete = isTemporary;

            var name = _messageNameFormatter.GetMessageName(messageType).ToString();

            var settings = new TopicPublishSettings(name, durable, autoDelete);

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
                else if (c == '_' || c == '-')
                    sb.Append(c);

            sb.Append('-');
            foreach (var c in host.ProcessName)
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
                else if (c == '_' || c == '-')
                    sb.Append(c);

            sb.Append('-');
            sb.Append(NewId.Next().ToString(Formatter));

            return sb.ToString();
        }
    }
}
