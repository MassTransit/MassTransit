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
namespace MassTransit.Saga.Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Exceptions;
    using Magnum.Common.Data;
    using MassTransit.Pipeline.Interceptors;

    public class OrchestrateSagaMessageSink<TComponent, TMessage> :
        SagaMessageSinkBase<TComponent, TMessage>
        where TMessage : class, CorrelatedBy<Guid>
        where TComponent : class, Orchestrates<TMessage>, ISaga
    {
        public OrchestrateSagaMessageSink(IInterceptorContext context, IServiceBus bus, ISagaRepository<TComponent> repository)
            : base(context, bus, repository)
        {
        }

        public override IEnumerable<Consumes<TMessage>.All> Enumerate(TMessage message)
        {
            Guid correlationId = message.CorrelationId;

			try
			{
				using (IUnitOfWork work = UnitOfWork.Start())
				using (ITransaction transaction = work.BeginTransaction(IsolationLevel.Serializable))
				{
					TComponent saga = GetSaga(correlationId);

					yield return saga;

					Repository.Save(saga);

					transaction.Commit();
				}
			}
			finally
			{
				UnitOfWork.Finish();
			}
        }

        private TComponent GetSaga(Guid correlationId)
        {
            TComponent saga = Repository.Get(correlationId);
            if (saga == null)
                throw new SagaException("The saga could not be loaded.", typeof (TComponent), typeof (TMessage), correlationId);

            saga.Bus = Bus;

            return saga;
        }
    }
}