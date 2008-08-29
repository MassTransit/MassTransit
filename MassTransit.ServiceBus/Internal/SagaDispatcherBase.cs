// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.ServiceBus.Internal
{
    using System;
    using Saga;

    public abstract class SagaDispatcherBase<TSaga, TMessage> :
        Consumes<TMessage>.Selected
        where TSaga : class, ISaga<TSaga>, Consumes<TMessage>.All
        where TMessage : class, CorrelatedBy<Guid>
    {
        protected IObjectBuilder _builder;
        protected IServiceBus _bus;
        protected ISagaRepository<TSaga> _repository;

        protected SagaDispatcherBase(IServiceBus bus, IObjectBuilder builder, ISagaRepository<TSaga> repository)
        {
            _bus = bus;
            _builder = builder;
            _repository = repository;
        }

        public bool Accept(TMessage message)
        {
            return true;
        }

        public abstract void Consume(TMessage message);

        protected void DispatchToConsumer(TSaga saga, TMessage message)
        {
            saga.Builder = _builder;
            saga.Bus = _bus;
            saga.Save += x => _repository.Save(x);

            saga.Consume(message);
        }
    }
}