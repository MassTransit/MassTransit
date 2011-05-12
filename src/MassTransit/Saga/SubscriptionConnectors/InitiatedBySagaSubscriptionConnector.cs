// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Saga.SubscriptionConnectors
{
	using System;
	using Exceptions;
	using Pipeline;

	public class InitiatedBySagaSubscriptionConnector<TSaga, TMessage> :
		SagaSubscriptionConnector<TSaga, TMessage>
		where TSaga : class, ISaga, InitiatedBy<TMessage>
		where TMessage : class, CorrelatedBy<Guid>
	{
		public InitiatedBySagaSubscriptionConnector(ISagaRepository<TSaga> sagaRepository)
			: base(sagaRepository, new InitiatingSagaPolicy<TSaga, TMessage>(x => false))
		{
		}

		protected override ISagaMessageSink<TSaga, TMessage> CreateSink(ISagaRepository<TSaga> repository,
		                                                                ISagaPolicy<TSaga, TMessage> policy)
		{
			var sink = new CorrelatedSagaMessageSink<TSaga, TMessage>(repository, policy);
			if (sink == null)
				throw new ConfigurationException("Could not build the initiating message sink for " + typeof (TSaga).FullName);

			return sink;
		}
	}
}