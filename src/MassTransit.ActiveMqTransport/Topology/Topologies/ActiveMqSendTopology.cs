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
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;
    using Settings;
    using Util;


    public class ActiveMqSendTopology :
        SendTopology,
        IActiveMqSendTopologyConfigurator
    {
        public ActiveMqSendTopology(IEntityNameValidator validator)
        {
            EntityNameValidator = validator;
        }

        public IEntityNameValidator EntityNameValidator { get; }

        IActiveMqMessageSendTopologyConfigurator<T> IActiveMqSendTopology.GetMessageTopology<T>()
        {
            IMessageSendTopologyConfigurator<T> configurator = base.GetMessageTopology<T>();

            return configurator as IActiveMqMessageSendTopologyConfigurator<T>;
        }

        public SendSettings GetSendSettings(Uri address)
        {
            var name = address.AbsolutePath.Substring(1);
            string[] pathSegments = name.Split('/');
            if (pathSegments.Length == 2)
                name = pathSegments[1];

            if (name == "*")
                throw new ArgumentException("Cannot send to a dynamic address");

            EntityNameValidator.ThrowIfInvalidEntityName(name);

            var isTemporary = address.Query.GetValueFromQueryString("temporary", false);

            var durable = address.Query.GetValueFromQueryString("durable", !isTemporary);
            var autoDelete = address.Query.GetValueFromQueryString("autodelete", isTemporary);

            return new QueueSendSettings(name, durable, autoDelete);
        }

        public ErrorSettings GetErrorSettings(EntitySettings settings)
        {
            return new ActiveMqErrorSettings(settings, settings.EntityName + "_error");
        }

        public DeadLetterSettings GetDeadLetterSettings(EntitySettings settings)
        {
            return new ActiveMqDeadLetterSettings(settings, settings.EntityName + "_skipped");
        }

        protected override IMessageSendTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var messageTopology = new ActiveMqMessageSendTopology<T>();

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }
    }
}