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
    using System.Globalization;
    using Configuration;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;
    using Settings;


    public class AmazonSqsSendTopology :
        SendTopology,
        IAmazonSqsSendTopologyConfigurator
    {
        public AmazonSqsSendTopology(IEntityNameValidator validator)
        {
            EntityNameValidator = validator;
        }

        public IEntityNameValidator EntityNameValidator { get; }

        IAmazonSqsMessageSendTopologyConfigurator<T> IAmazonSqsSendTopology.GetMessageTopology<T>()
        {
            IMessageSendTopologyConfigurator<T> configurator = base.GetMessageTopology<T>();

            return configurator as IAmazonSqsMessageSendTopologyConfigurator<T>;
        }

        public SendSettings GetSendSettings(AmazonSqsEndpointAddress address)
        {
            return new QueueSendSettings(address.Path, true, address.AutoDelete);
        }

        public ErrorSettings GetErrorSettings(EntitySettings settings)
        {
            return new QueueErrorSettings(settings, BuildEntityName(settings.EntityName, "_error"));
        }

        public DeadLetterSettings GetDeadLetterSettings(EntitySettings settings)
        {
            return new QueueDeadLetterSettings(settings, BuildEntityName(settings.EntityName, "_skipped"));
        }

        protected override IMessageSendTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var messageTopology = new AmazonSqsMessageSendTopology<T>();

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }

        string BuildEntityName(string entityName, string suffix)
        {
            const string fifoSuffix = ".fifo";

            if (!entityName.EndsWith(fifoSuffix, true, CultureInfo.InvariantCulture))
            {
                return entityName + suffix;
            }

            return entityName.Substring(0, entityName.Length - fifoSuffix.Length) + suffix + fifoSuffix;
        }
    }
}
