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
namespace MassTransit.ConsumeConnectors
{
    using System;
    using Pipeline.Filters;


    public class ConsumeMessageConnectorFactory<TConsumer, TMessage> :
        IMessageConnectorFactory
        where TConsumer : class, IConsumer<TMessage>
        where TMessage : class
    {
        readonly ConsumerMessageConnector<TConsumer, TMessage> _consumerConnector;
        readonly InstanceMessageConnector<TConsumer, TMessage> _instanceConnector;

        public ConsumeMessageConnectorFactory()
        {
            var filter = new MethodConsumerMessageFilter<TConsumer, TMessage>();

            _consumerConnector = new ConsumerMessageConnector<TConsumer, TMessage>(filter);
            _instanceConnector = new InstanceMessageConnector<TConsumer, TMessage>(filter);
        }

        public IConsumerMessageConnector<T> CreateConsumerConnector<T>()
            where T : class
        {
            var result = _consumerConnector as IConsumerMessageConnector<T>;
            if(result == null)
                throw new ArgumentException("The consumer type did not match the connector type");

            return result;
        }

        IInstanceMessageConnector<T> IMessageConnectorFactory.CreateInstanceConnector<T>()
        {
            var result = _instanceConnector as IInstanceMessageConnector<T>;
            if (result == null)
                throw new ArgumentException("The consumer type did not match the connector type");

            return result;
        }
    }
}