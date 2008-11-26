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
namespace MassTransit.Internal
{
    using System;
    using System.Transactions;
    using Saga;

    public abstract class SagaDispatcherBase<TSaga, TMessage> :
        Consumes<TMessage>.Selected
        where TSaga : class, ISaga, Consumes<TMessage>.All
        where TMessage : class, CorrelatedBy<Guid>
    {
    	private readonly IObjectBuilder _builder;
    	private readonly IServiceBus _bus;
    	private readonly ISagaRepository<TSaga> _repository;

    	protected IObjectBuilder Builder
    	{
    		get { return _builder; }
    	}

    	protected IServiceBus Bus
    	{
    		get { return _bus; }
    	}

    	protected ISagaRepository<TSaga> Repository
    	{
    		get { return _repository; }
    	}

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
            saga.Bus = _bus;

            saga.Consume(message);
        }

        protected void UsingTransaction(TMessage message, Action<TMessage> action)
        {
            // if we are already pulling from a transactional queue, use the existing transaction
            if(Transaction.Current != null)
            {
                action(message);
                return;
            }

            using(TransactionScope scope = new TransactionScope())
            {
                action(message);
                scope.Complete();
            }
        }
    }
}