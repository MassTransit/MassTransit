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
    using Exceptions;
    using Saga;

    public class SagaDispatcher<TSaga, TMessage> :
        SagaDispatcherBase<TSaga, TMessage>
        where TSaga : class, ISaga, Consumes<TMessage>.All
        where TMessage : class, CorrelatedBy<Guid>
    {
        public SagaDispatcher(IServiceBus bus, IObjectBuilder builder, ISagaRepository<TSaga> repository) :
            base(bus, builder, repository)
        {
        }

        public override void Consume(TMessage message)
        {
            Guid correlationId = message.CorrelationId;

            UsingTransaction(message, m =>
            {
                TSaga saga = _repository.Get(correlationId);
                if (saga == null)
                    throw new SagaException("The saga could not be loaded.", typeof(TSaga), typeof(TMessage), correlationId);

                DispatchToConsumer(saga, message);

                _repository.Save(saga);
            });
        }
    }
}