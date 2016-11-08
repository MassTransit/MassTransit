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
namespace MassTransit.Saga.Connectors
{
    using System;
    using System.Collections.Generic;
    using GreenPipes;
    using MassTransit.Pipeline;
    using Pipeline.Filters;
    using Util;


    public abstract class SagaMessageConnector<TSaga, TMessage> :
        ISagaMessageConnector
        where TSaga : class, ISaga
        where TMessage : class
    {
        ConnectHandle ISagaConnector.ConnectSaga<T>(IConsumePipeConnector consumePipe, ISagaRepository<T> sagaRepository,
            params IPipeSpecification<SagaConsumeContext<T>>[] pipeSpecifications)
        {
            var repository = sagaRepository as ISagaRepository<TSaga>;
            if (repository == null)
                throw new ArgumentException("The saga repository type does not match: " + TypeMetadataCache<T>.ShortName);

            IPipe<SagaConsumeContext<TSaga, TMessage>> sagaPipe = BuildSagaPipe(pipeSpecifications);

            IPipe<ConsumeContext<TMessage>> messagePipe = BuildMessagePipe(pipeSpecifications, repository, sagaPipe);

            return consumePipe.ConnectConsumePipe(messagePipe);
        }

        public Type MessageType => typeof(TMessage);

        IPipe<ConsumeContext<TMessage>> BuildMessagePipe<T>(IPipeSpecification<SagaConsumeContext<T>>[] pipeSpecifications, ISagaRepository<TSaga> repository,
            IPipe<SagaConsumeContext<TSaga, TMessage>> sagaPipe)
            where T : class, ISaga
        {
            return Pipe.New<ConsumeContext<TMessage>>(x =>
            {
                var messagePipeBuilder = new MessagePipeBuilder<T>();
                for (var i1 = 0; i1 < pipeSpecifications.Length; i1++)
                    pipeSpecifications[i1].Apply(messagePipeBuilder);

                var pipeBuilder = messagePipeBuilder as MessagePipeBuilder<TSaga>;
                if (pipeBuilder == null)
                    throw new InvalidOperationException("Should not be null, ever");

                foreach (IFilter<ConsumeContext<TMessage>> filter in pipeBuilder.Filters)
                    x.UseFilter(filter);

                ConfigureMessagePipe(x, repository, sagaPipe);
            });
        }

        IPipe<SagaConsumeContext<TSaga, TMessage>> BuildSagaPipe<T>(IPipeSpecification<SagaConsumeContext<T>>[] pipeSpecifications)
            where T : class, ISaga
        {
            var builder = new SagaPipeBuilder<T>();
            for (var i = 0; i < pipeSpecifications.Length; i++)
                pipeSpecifications[i].Apply(builder);

            var builders = builder as SagaPipeBuilder<TSaga>;
            if (builders == null)
                throw new InvalidOperationException("Should not be null, ever");

            return Pipe.New<SagaConsumeContext<TSaga, TMessage>>(x =>
            {
                foreach (IFilter<SagaConsumeContext<TSaga, TMessage>> filter in builders.Filters)
                    x.UseFilter(filter);

                ConfigureSagaPipe(x);
            });
        }

        /// <summary>
        /// Configure the saga pipe to which the saga instance is sent
        /// </summary>
        /// <param name="configurator"></param>
        protected abstract void ConfigureSagaPipe(IPipeConfigurator<SagaConsumeContext<TSaga, TMessage>> configurator);

        /// <summary>
        /// Configure the message pipe that is prior to the saga repository
        /// </summary>
        /// <param name="configurator">The pipe configurator</param>
        /// <param name="repository"></param>
        /// <param name="sagaPipe"></param>
        protected abstract void ConfigureMessagePipe(IPipeConfigurator<ConsumeContext<TMessage>> configurator, ISagaRepository<TSaga> repository,
            IPipe<SagaConsumeContext<TSaga, TMessage>> sagaPipe);


        class MessagePipeBuilder<T> :
            IPipeBuilder<SagaConsumeContext<T>>,
            IPipeBuilder<ConsumeContext<TMessage>>
            where T : class, ISaga
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

            public void AddFilter(IFilter<SagaConsumeContext<T>> filter)
            {
                // skip filters that are at the saga level, only interested in message-level filters
            }
        }


        class SagaPipeBuilder<T> :
            IPipeBuilder<SagaConsumeContext<T>>,
            IPipeBuilder<SagaConsumeContext<T, TMessage>>
            where T : class, ISaga
        {
            readonly IList<IFilter<SagaConsumeContext<T, TMessage>>> _filters;

            public SagaPipeBuilder()
            {
                _filters = new List<IFilter<SagaConsumeContext<T, TMessage>>>();
            }

            public IEnumerable<IFilter<SagaConsumeContext<T, TMessage>>> Filters => _filters;

            public void AddFilter(IFilter<SagaConsumeContext<T, TMessage>> filter)
            {
                _filters.Add(filter);
            }

            public void AddFilter(IFilter<SagaConsumeContext<T>> filter)
            {
                _filters.Add(new SagaSplitFilter<T, TMessage>(filter));
            }
        }
    }
}