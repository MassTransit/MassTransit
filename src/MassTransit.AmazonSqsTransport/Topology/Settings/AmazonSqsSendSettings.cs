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
namespace MassTransit.AmazonSqsTransport.Topology.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Builders;
    using Configuration.Configurators;


    public class AmazonSqsSendSettings :
        TopicConfigurator,
        SendSettings
    {
        bool _bindToQueue;
        string _queueName;

        public AmazonSqsSendSettings(string topicName, bool durable, bool autoDelete)
            : base(topicName, durable, autoDelete)
        {

        }

        public Uri GetSendAddress(Uri hostAddress)
        {
            var builder = new UriBuilder(hostAddress);

            builder.Path = builder.Path == "/"
                ? $"/{EntityName}"
                : $"/{string.Join("/", builder.Path.Trim('/'), EntityName)}";

            builder.Query += string.Join("&", GetQueryStringOptions());

            return builder.Uri;
        }

        public BrokerTopology GetBrokerTopology()
        {
            var builder = new PublishEndpointBrokerTopologyBuilder();

            builder.Topic = builder.CreateTopic(EntityName, Durable, AutoDelete);

            if (_bindToQueue)
            {
                var queue = builder.CreateQueue(_queueName, Durable, AutoDelete);

                builder.BindQueue(builder.Topic, queue, "");
            }

            return builder.BuildBrokerTopology();
        }

        public void BindToQueue(string queueName)
        {
            _bindToQueue = true;
            _queueName = queueName;
        }

        protected override IEnumerable<string> GetQueryStringOptions()
        {
            foreach (var option in base.GetQueryStringOptions())
                yield return option;

            if (_bindToQueue)
                yield return "bind=true";

            if (!string.IsNullOrWhiteSpace(_queueName))
                yield return "queue=" + WebUtility.UrlEncode(_queueName);
        }

        IEnumerable<string> GetSettingStrings()
        {
            if (Durable)
                yield return "durable";

            if (AutoDelete)
                yield return "auto-delete";
        }

        public override string ToString()
        {
            return string.Join(", ", GetSettingStrings());
        }
    }
}
