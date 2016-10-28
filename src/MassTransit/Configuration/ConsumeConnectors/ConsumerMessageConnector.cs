// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using GreenPipes;
    using Pipeline;
    using Pipeline.Filters;
    using Util;


    public class ConsumerMessageConnector<TConsumer, TMessage> :
        IConsumerMessageConnector
        where TConsumer : class
        where TMessage : class
    {
        readonly IFilter<ConsumerConsumeContext<TConsumer, TMessage>> _consumeFilter;

        public ConsumerMessageConnector(IFilter<ConsumerConsumeContext<TConsumer, TMessage>> consumeFilter)
        {
            _consumeFilter = consumeFilter;
        }

        public Type MessageType => typeof(TMessage);

        ConnectHandle IConsumerConnector.ConnectConsumer<T>(IConsumePipeConnector consumePipe, IConsumerFactory<T> consumerFactory,
            IPipeSpecification<ConsumerConsumeContext<T>>[] pipeSpecifications)
        {
            var factory = consumerFactory as IConsumerFactory<TConsumer>;
            if (factory == null)
                throw new ArgumentException("The consumer factory type does not match: " + TypeMetadataCache<T>.ShortName);

            IPipe<ConsumerConsumeContext<TConsumer, TMessage>> consumerPipe = ConsumerPipeBuilder.BuildConsumerPipe(_consumeFilter, pipeSpecifications);

            IPipe<ConsumeContext<TMessage>> messagePipe = MessagePipeBuilder.BuildMessagePipe<TConsumer, TMessage, T>(pipeSpecifications, new ConsumerMessageFilter<TConsumer, TMessage>(factory, consumerPipe));

            return consumePipe.ConnectConsumePipe(messagePipe);
        }
    }
}