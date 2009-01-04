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
namespace MassTransit.Pipeline.Sinks
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using Exceptions;
	using Interceptors;
	using Magnum.Common.Data;
	using Saga;
	using Saga.Pipeline;
	using Util;

	public class InitiateSagaMessageSink<TComponent, TMessage> : 
		SagaMessageSinkBase<TComponent, TMessage>
		where TMessage : class, CorrelatedBy<Guid>
		where TComponent : class, Orchestrates<TMessage>, ISaga
	{
		public InitiateSagaMessageSink(IInterceptorContext context, IServiceBus bus, ISagaRepository<TComponent> repository) : 
			base(context, bus, repository)
		{
		}

		public override IEnumerable<Consumes<TMessage>.All> Enumerate(TMessage message)
		{
			Guid correlationId = message.CorrelationId;
			if (correlationId == Guid.Empty)
				correlationId = CombGuid.NewCombGuid();

			try
			{
				using (IUnitOfWork work = UnitOfWork.Start())
				using(ITransaction transaction = work.BeginTransaction(IsolationLevel.Serializable))
				{
					TComponent saga = CreateSaga(correlationId);

					work.Flush();

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

		private TComponent CreateSaga(Guid correlationId)
		{
			try
			{
				TComponent saga = Repository.Create(correlationId);
				saga.Bus = Bus;

				return saga;
			}
			catch(Exception ex)
			{
				throw new SagaException("The saga could not be created.", typeof (TComponent), typeof (TMessage), correlationId, ex);
			}
		}
	}
}