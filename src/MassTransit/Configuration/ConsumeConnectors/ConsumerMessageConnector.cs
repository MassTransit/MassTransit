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
    using System.Collections.Generic;
    using PipeBuilders;
    using PipeConfigurators;
    using Pipeline;
    using Pipeline.Filters;
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
        readonly IFilter<ConsumerConsumeContext<TConsumer, TMessage>> _consumeFilter;

        public ConsumerMessageConnector(IFilter<ConsumerConsumeContext<TConsumer, TMessage>> consumeFilter)
        {
            _consumeFilter = consumeFilter;
        }

        public Type MessageType
        {
            get { return typeof(TMessage); }
        }

        ConnectHandle ConsumerConnector.Connect<T>(IConsumePipeConnector consumePipe, IConsumerFactory<T> consumerFactory,
            params IPipeSpecification<ConsumerConsumeContext<T>>[] pipeSpecifications)
        {
            var factory = consumerFactory as IConsumerFactory<TConsumer>;
            if (factory == null)
                throw new ArgumentException("The consumer factory type does not match: " + TypeMetadataCache<T>.ShortName);

            var builder = new ConsumerPipeBuilder<T>();
            for (int i = 0; i < pipeSpecifications.Length; i++)
                pipeSpecifications[i].Apply(builder);

            var builders = builder as ConsumerPipeBuilder<TConsumer>;
            if (builders == null)
                throw new InvalidOperationException("Should not be null, ever");

            IPipe<ConsumerConsumeContext<TConsumer, TMessage>> messagePipe = Pipe.New<ConsumerConsumeContext<TConsumer, TMessage>>(x =>
            {
                foreach (var filter in builders.Filters)
                    x.Filter(filter);
                x.Filter(_consumeFilter);
            });

            IPipe<ConsumeContext<TMessage>> pipe =
                Pipe.New<ConsumeContext<TMessage>>(x => x.Filter(new ConsumerMessageFilter<TConsumer, TMessage>(factory, messagePipe)));

            return consumePipe.ConnectConsumePipe(pipe);
        }


        class ConsumerPipeBuilder<T> :
            IPipeBuilder<ConsumerConsumeContext<T>>
            where T : class
        {
            readonly IList<IFilter<ConsumerConsumeContext<T>>> _filters;

            public ConsumerPipeBuilder()
            {
                _filters = new List<IFilter<ConsumerConsumeContext<T>>>();
            }

            public IEnumerable<IFilter<ConsumerConsumeContext<T>>> Filters
            {
                get { return _filters; }
            }

            public void AddFilter(IFilter<ConsumerConsumeContext<T>> filter)
            {
                _filters.Add(filter);
            }
        }
    }
}