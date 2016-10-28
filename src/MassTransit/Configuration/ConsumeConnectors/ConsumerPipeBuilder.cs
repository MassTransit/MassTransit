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
    using Pipeline.Filters;


    public static class ConsumerPipeBuilder
    {
        public static IPipe<ConsumerConsumeContext<TConsumer, TMessage>> BuildConsumerPipe<TConsumer, TMessage, T>(
            IFilter<ConsumerConsumeContext<TConsumer, TMessage>> consumeFilter, IPipeSpecification<ConsumerConsumeContext<T>>[] pipeSpecifications)
            where TConsumer : class
            where TMessage : class
            where T : class
        {
            var builder = new ConsumerPipeBuilder<TConsumer, TMessage, T>();
            for (var i = 0; i < pipeSpecifications.Length; i++)
                pipeSpecifications[i].Apply(builder);

            var builders = builder as ConsumerPipeBuilder<TConsumer, TMessage, TConsumer>;
            if (builders == null)
                throw new InvalidOperationException("Should not be null, ever");

            return Pipe.New<ConsumerConsumeContext<TConsumer, TMessage>>(x =>
            {
                foreach (IFilter<ConsumerConsumeContext<TConsumer, TMessage>> filter in builders.Filters)
                    x.UseFilter(filter);

                x.UseFilter(consumeFilter);
            });
        }
    }


    public class ConsumerPipeBuilder<TConsumer, TMessage, T> :
        IPipeBuilder<ConsumerConsumeContext<T>>,
        IPipeBuilder<ConsumerConsumeContext<T, TMessage>>
        where TConsumer : class
        where TMessage : class
        where T : class
    {
        readonly IList<IFilter<ConsumerConsumeContext<T, TMessage>>> _filters;

        public ConsumerPipeBuilder()
        {
            _filters = new List<IFilter<ConsumerConsumeContext<T, TMessage>>>();
        }

        public IEnumerable<IFilter<ConsumerConsumeContext<T, TMessage>>> Filters => _filters;

        public void AddFilter(IFilter<ConsumerConsumeContext<T, TMessage>> filter)
        {
            _filters.Add(filter);
        }

        public void AddFilter(IFilter<ConsumerConsumeContext<T>> filter)
        {
            _filters.Add(new ConsumerSplitFilter<T, TMessage>(filter));
        }

        public static IPipe<ConsumerConsumeContext<TConsumer, TMessage>> BuildConsumerPipe(IFilter<ConsumerConsumeContext<TConsumer, TMessage>> consumeFilter,
            IPipeSpecification<ConsumerConsumeContext<T>>[] pipeSpecifications)
        {
            var builder = new ConsumerPipeBuilder<TConsumer, TMessage, T>();
            for (var i = 0; i < pipeSpecifications.Length; i++)
                pipeSpecifications[i].Apply(builder);

            var builders = builder as ConsumerPipeBuilder<TConsumer, TMessage, TConsumer>;
            if (builders == null)
                throw new InvalidOperationException("Should not be null, ever");

            return Pipe.New<ConsumerConsumeContext<TConsumer, TMessage>>(x =>
            {
                foreach (IFilter<ConsumerConsumeContext<TConsumer, TMessage>> filter in builders.Filters)
                    x.UseFilter<ConsumerConsumeContext<TConsumer, TMessage>>(filter);

                x.UseFilter<ConsumerConsumeContext<TConsumer, TMessage>>(consumeFilter);
            });
        }
    }
}