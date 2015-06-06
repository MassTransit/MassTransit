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
namespace MassTransit.Saga.Connectors
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Pipeline;
    using PipeBuilders;
    using PipeConfigurators;
    using Util;


    public abstract class SagaMessageConnector<TSaga, TMessage> :
        ISagaMessageConnector
        where TSaga : class, ISaga
        where TMessage : class
    {
        ConnectHandle ISagaConnector.Connect<T>(IConsumePipeConnector consumePipe, ISagaRepository<T> sagaRepository,
            params IPipeSpecification<SagaConsumeContext<T>>[] pipeSpecifications)
        {
            var repository = sagaRepository as ISagaRepository<TSaga>;
            if (repository == null)
                throw new ArgumentException("The saga repository type does not match: " + TypeMetadataCache<T>.ShortName);

            var builder = new SagaPipeBuilder<T>();
            for (int i = 0; i < pipeSpecifications.Length; i++)
                pipeSpecifications[i].Apply(builder);

            var builders = builder as SagaPipeBuilder<TSaga>;
            if (builders == null)
                throw new InvalidOperationException("Should not be null, ever");

            IPipe<SagaConsumeContext<TSaga, TMessage>> sagaPipe = Pipe.New<SagaConsumeContext<TSaga, TMessage>>(x =>
            {
                foreach (var filter in builders.Filters)
                    x.Filter(filter);

                ConfigureSagaPipe(x);
            });

            IPipe<ConsumeContext<TMessage>> messagePipe = Pipe.New<ConsumeContext<TMessage>>(x =>
            {
                ConfigureMessagePipe(x, repository, sagaPipe);
            });

            return consumePipe.ConnectConsumePipe(messagePipe);
        }

        public Type MessageType
        {
            get { return typeof(TMessage); }
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


        class SagaPipeBuilder<T> :
            IPipeBuilder<SagaConsumeContext<T>>
            where T : class, ISaga
        {
            readonly IList<IFilter<SagaConsumeContext<T>>> _filters;

            public SagaPipeBuilder()
            {
                _filters = new List<IFilter<SagaConsumeContext<T>>>();
            }

            public IEnumerable<IFilter<SagaConsumeContext<T>>> Filters
            {
                get { return _filters; }
            }

            public void AddFilter(IFilter<SagaConsumeContext<T>> filter)
            {
                _filters.Add(filter);
            }
        }
    }
}