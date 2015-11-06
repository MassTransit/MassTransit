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
namespace MassTransit.Tests.Conventional
{
    using ConsumeConnectors;


    public class CustomConsumeConnectorFactory<TConsumer, TMessage> :
        IMessageConnectorFactory
        where TConsumer : class, IHandler<TMessage>
        where TMessage : class
    {
        readonly CustomMethodConsumerMessageFilter<TConsumer, TMessage> _filter;

        public CustomConsumeConnectorFactory()
        {
            _filter = new CustomMethodConsumerMessageFilter<TConsumer, TMessage>();
        }

        public IConsumerMessageConnector CreateConsumerConnector()
        {
            return new ConsumerMessageConnector<TConsumer, TMessage>(_filter);
        }

        public IInstanceMessageConnector CreateInstanceConnector()
        {
            return new InstanceMessageConnector<TConsumer, TMessage>(_filter);
        }
    }
}