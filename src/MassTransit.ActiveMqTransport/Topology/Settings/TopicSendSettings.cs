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
namespace MassTransit.ActiveMqTransport.Topology.Settings
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using Configurators;


    public class TopicSendSettings :
        TopicConfigurator,
        SendSettings
    {
        public TopicSendSettings(string exchangeName, bool durable, bool autoDelete)
            : base(exchangeName, durable, autoDelete)
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

            return builder.BuildBrokerTopology();
        }

        protected override IEnumerable<string> GetQueryStringOptions()
        {
            foreach (var option in base.GetQueryStringOptions())
                yield return option;
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