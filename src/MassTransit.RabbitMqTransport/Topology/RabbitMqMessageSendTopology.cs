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
namespace MassTransit.RabbitMqTransport.Topology
{
    using System;
    using MassTransit.Topology;
    using Util;


    public class RabbitMqMessageSendTopology<TMessage> :
        MessageSendTopology<TMessage>,
        IRabbitMqMessageSendTopologyConfigurator<TMessage>,
        IRabbitMqMessageSendTopologyConfigurator
        where TMessage : class
    {
        IRabbitMqMessageSendTopologyConfigurator<T> IRabbitMqMessageSendTopologyConfigurator.GetMessageTopology<T>()
        {
            var result = this as IRabbitMqMessageSendTopologyConfigurator<T>;
            if (result == null)
                throw new ArgumentException($"The expected message type was invalid: {TypeMetadataCache<T>.ShortName}");

            return result;
        }
    }
}