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
namespace MassTransit.SubscriptionConnectors
{
    using Pipeline;


    public class MessageSubscriptionConnectorFactory<TConsumer, TMessage> :
        SubscriptionConnectorFactory
        where TConsumer : class, IMessageConsumer<TMessage>
        where TMessage : class
    {
        readonly LegacyMethodConsumerMessageFilter<TConsumer, TMessage> _filter;

        public MessageSubscriptionConnectorFactory()
        {
            _filter = new LegacyMethodConsumerMessageFilter<TConsumer, TMessage>();
        }

        public ConsumerMessageConnector CreateSubscriptionConnector()
        {
            return new ConsumerMessageConnector<TConsumer, TMessage>(_filter);
        }

        public InstanceMessageConnector CreateInstanceConnector()
        {
            return new InstanceMessageConnector<TConsumer, TMessage>(_filter);
        }
    }
}