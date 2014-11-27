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
	using System.Linq.Expressions;
	using Magnum.Reflection;
	using Pipeline;

	public class ObservesSagaSubscriptionConnector<TSaga, TMessage> :
		SagaSubscriptionConnector<TSaga, TMessage>
		where TSaga : class, ISaga, Observes<TMessage, TSaga>
		where TMessage : class
	{
		readonly Expression<Func<TSaga, TMessage, bool>> _selector;

		public ObservesSagaSubscriptionConnector(ISagaRepository<TSaga> sagaRepository)
			: base(sagaRepository, new ExistingOrIgnoreSagaPolicy<TSaga, TMessage>(x => false))
		{
			var instance = (Observes<TMessage, TSaga>) FastActivator<TSaga>.Create(Guid.Empty);

			_selector = instance.GetBindExpression();
		}

		protected override ISagaMessageSink<TSaga, TMessage> CreateSink(ISagaRepository<TSaga> repository,
		                                                                ISagaPolicy<TSaga, TMessage> policy)
		{
            throw new NotImplementedException();
            //var sink = new PropertySagaMessageSink<TSaga, TMessage>(repository, policy, _selector);
		
		}
	}
}