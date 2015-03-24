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
    using Pipeline.Filters;
    using Util;


    public interface SagaMessageConnector :
        SagaConnector
    {
        Type MessageType { get; }
    }


    public abstract class SagaMessageConnector<TSaga, TMessage> :
        SagaMessageConnector
        where TSaga : class, ISaga
        where TMessage : class
    {
        public ConnectHandle Connect<T>(IConsumePipeConnector consumePipe, ISagaRepository<T> sagaRepository,
            params IPipeSpecification<SagaConsumeContext<T>>[] pipeSpecifications)
            where T : class, ISaga
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

            IPipe<SagaConsumeContext<TSaga, TMessage>> messagePipe = Pipe.New<SagaConsumeContext<TSaga, TMessage>>(x =>
            {
                foreach (var filter in builders.Filters)
                    x.Filter(filter);
                x.Filter(GetMessageFilter());
            });

            IPipe<ConsumeContext<TMessage>> pipe = Pipe.New<ConsumeContext<TMessage>>(x =>
            {
                x.Filter(GetLocatorFilter(repository));
                x.Filter(new SagaMessageFilter<TSaga, TMessage>(repository, messagePipe));
            });

            return consumePipe.ConnectConsumePipe(pipe);
        }

        public Type MessageType
        {
            get { return typeof(TMessage); }
        }

        protected abstract IFilter<SagaConsumeContext<TSaga, TMessage>> GetMessageFilter();
        protected abstract IFilter<ConsumeContext<TMessage>> GetLocatorFilter(ISagaRepository<TSaga> repository);


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