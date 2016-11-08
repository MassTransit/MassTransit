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
    using System.Collections.Generic;
    using GreenPipes;


    public static class MessagePipeBuilder
    {
        public static IPipe<ConsumeContext<TMessage>> BuildMessagePipe<TConsumer, TMessage, T>(
            IPipeSpecification<ConsumerConsumeContext<T>>[] pipeSpecifications, IFilter<ConsumeContext<TMessage>> messageFilter)
            where TMessage : class where T : class where TConsumer : class
        {
            return Pipe.New<ConsumeContext<TMessage>>(x =>
            {
                var messagePipeBuilder = new MessagePipeBuilder<TConsumer, TMessage, T>();
                for (var i = 0; i < pipeSpecifications.Length; i++)
                    pipeSpecifications[i].Apply(messagePipeBuilder);

                var pipeBuilder = messagePipeBuilder as MessagePipeBuilder<TConsumer, TMessage, TConsumer>;
                if (pipeBuilder == null)
                    throw new InvalidOperationException("Should not be null, ever");

                foreach (IFilter<ConsumeContext<TMessage>> filter in pipeBuilder.Filters)
                    x.UseFilter(filter);

                x.UseFilter(messageFilter);
            });
        }
    }


    public class MessagePipeBuilder<TConsumer, TMessage, T> :
        IPipeBuilder<ConsumerConsumeContext<T>>,
        IPipeBuilder<ConsumeContext<TMessage>>,
        IPipeBuilder<ConsumerConsumeContext<TConsumer, T>>
        where TConsumer : class
        where TMessage : class
        where T : class
    {
        readonly IList<IFilter<ConsumeContext<TMessage>>> _filters;

        public MessagePipeBuilder()
        {
            _filters = new List<IFilter<ConsumeContext<TMessage>>>();
        }

        public IEnumerable<IFilter<ConsumeContext<TMessage>>> Filters => _filters;

        public void AddFilter(IFilter<ConsumeContext<TMessage>> filter)
        {
            _filters.Add(filter);
        }

        public void AddFilter(IFilter<ConsumerConsumeContext<T>> filter)
        {
            // skip filters that are at the consumer level, only interested in message-level filters
        }

        public void AddFilter(IFilter<ConsumerConsumeContext<TConsumer, T>> filter)
        {
            // skip filters that are at the consumer level, only interested in message-level filters
        }
    }
}