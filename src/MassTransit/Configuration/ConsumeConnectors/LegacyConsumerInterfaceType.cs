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
namespace MassTransit.ConsumeConnectors
{
    using System;


    /// <summary>
    /// A legacy message-only consumer
    /// </summary>
    public class LegacyConsumerInterfaceType :
        IMessageInterfaceType
    {
        readonly Lazy<IMessageConnectorFactory> _consumeConnectorFactory;

        public LegacyConsumerInterfaceType(Type messageType, Type consumerType)
        {
            MessageType = messageType;

            _consumeConnectorFactory = new Lazy<IMessageConnectorFactory>(() => (IMessageConnectorFactory)
                Activator.CreateInstance(typeof(LegacyConsumeConnectorFactory<,>).MakeGenericType(consumerType,
                    messageType)));
        }

        public Type MessageType { get; }

        IConsumerMessageConnector<T> IMessageInterfaceType.GetConsumerConnector<T>()
        {
            return _consumeConnectorFactory.Value.CreateConsumerConnector<T>();
        }

        IInstanceMessageConnector<T> IMessageInterfaceType.GetInstanceConnector<T>()
        {
            return _consumeConnectorFactory.Value.CreateInstanceConnector<T>();
        }
    }
}