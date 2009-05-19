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
	using System.Linq.Expressions;
	using MassTransit.Pipeline;

	public class CorrelatedSagaMessageSink<TSaga, TMessage> :
		SagaMessageSinkBase<TSaga, TMessage>
		where TMessage : class, CorrelatedBy<Guid>
		where TSaga : ISaga, Consumes<TMessage>.All
	{
		public CorrelatedSagaMessageSink(ISubscriberContext context,
		                                 IServiceBus bus,
		                                 ISagaRepository<TSaga> repository,
		                                 ISagaPolicy<TSaga, TMessage> policy)
			: base(context, bus, repository, policy)
		{
		}

		protected override void ConsumerAction(TSaga saga, TMessage message)
		{
			saga.Bus = Bus;
			saga.Consume(message);
		}

		protected override Expression<Func<TSaga, bool>> GetExpressionForFindingSaga(TMessage item)
		{
			Guid sagaId = item.CorrelationId;

			return x => x.CorrelationId == sagaId;
		}
	}
}