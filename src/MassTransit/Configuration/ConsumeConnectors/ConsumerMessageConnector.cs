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
    using PipeBuilders;
    using PipeConfigurators;
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

            var consumerPipe = BuildConsumerPipe(pipeSpecifications);

            IPipe<ConsumeContext<TMessage>> pipe = BuildMessagePipe(pipeSpecifications, factory, consumerPipe);

            return consumePipe.ConnectConsumePipe(pipe);
        }

        IPipe<ConsumerConsumeContext<TConsumer, TMessage>> BuildConsumerPipe<T>(IPipeSpecification<ConsumerConsumeContext<T>>[] pipeSpecifications)
            where T : class
        {
            var builder = new ConsumerPipeBuilder<T>();
            for (int i = 0; i < pipeSpecifications.Length; i++)
                pipeSpecifications[i].Apply(builder);

            var builders = builder as ConsumerPipeBuilder<TConsumer>;
            if (builders == null)
                throw new InvalidOperationException("Should not be null, ever");

            return Pipe.New<ConsumerConsumeContext<TConsumer, TMessage>>(x =>
            {
                foreach (var filter in builders.Filters)
                    x.UseFilter(filter);

                x.UseFilter(_consumeFilter);
            });
        }

        IPipe<ConsumeContext<TMessage>> BuildMessagePipe<T>(IPipeSpecification<ConsumerConsumeContext<T>>[] pipeSpecifications,
            IConsumerFactory<TConsumer> consumerFactory, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> consumerPipe)
            where T : class
        {
            return Pipe.New<ConsumeContext<TMessage>>(x =>
            {
                var messagePipeBuilder = new MessagePipeBuilder<T>();
                for (int i = 0; i < pipeSpecifications.Length; i++)
                    pipeSpecifications[i].Apply(messagePipeBuilder);

                var pipeBuilder = messagePipeBuilder as MessagePipeBuilder<TConsumer>;
                if (pipeBuilder == null)
                    throw new InvalidOperationException("Should not be null, ever");

                foreach (var filter in pipeBuilder.Filters)
                    x.UseFilter(filter);

                x.UseFilter(new ConsumerMessageFilter<TConsumer, TMessage>(consumerFactory, consumerPipe));
            });
        }


        class MessagePipeBuilder<T> :
            IPipeBuilder<ConsumerConsumeContext<T>>,
            IPipeBuilder<ConsumeContext<TMessage>>
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
        }


        class ConsumerPipeBuilder<T> :
            IPipeBuilder<ConsumerConsumeContext<T>>
            where T : class
        {
            readonly IList<IFilter<ConsumerConsumeContext<T, TMessage>>> _filters;

            public ConsumerPipeBuilder()
            {
                _filters = new List<IFilter<ConsumerConsumeContext<T, TMessage>>>();
            }

            public IEnumerable<IFilter<ConsumerConsumeContext<T, TMessage>>> Filters => _filters;

            public void AddFilter(IFilter<ConsumerConsumeContext<T>> filter)
            {
                _filters.Add(new ConsumerSplitFilter<T, TMessage>(filter));
            }
        }
    }
}