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
    using System;
    using Pipeline;
    using Pipeline.Sinks;
    using Util;


    public interface ConsumerMessageConnector :
        ConsumerConnector
    {
        Type MessageType { get; }
    }


    public class ConsumerMessageConnector<TConsumer, TMessage> :
        ConsumerMessageConnector
        where TConsumer : class
        where TMessage : class
    {
        readonly IConsumerMessageAdapter<TConsumer, TMessage> _adapter;

        public ConsumerMessageConnector(IConsumerMessageAdapter<TConsumer, TMessage> adapter)
        {
            _adapter = adapter;
        }

        public Type MessageType
        {
            get { return typeof(TMessage); }
        }

        public ConnectHandle Connect<T>(IInboundMessagePipe pipe, IAsyncConsumerFactory<T> consumerFactory,
            IMessageRetryPolicy retryPolicy)
            where T : class
        {
            var factory = consumerFactory as IAsyncConsumerFactory<TConsumer>;
            if (factory == null)
            {
                throw new ArgumentException("The consumer factory type does not match: "
                                            + TypeMetadataCache<T>.ShortName);
            }

            var messagePipe = new ConsumerMessagePipe<TConsumer, TMessage>(factory, _adapter, retryPolicy);

            return pipe.Connect(messagePipe);
        }
    }
}