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
namespace MassTransit.Saga.Pipeline
{
	using System;
	using System.Collections.Generic;
	using MassTransit.Pipeline;

	public abstract class SagaMessageSinkBase<TSaga, TMessage> :
		ISagaMessageSink<TSaga, TMessage>
		where TSaga : class, ISaga
		where TMessage : class
	{
		readonly ISagaLocator<TMessage> _locator;
		readonly InstanceHandlerSelector<TSaga, TMessage> _selector;

		protected SagaMessageSinkBase(ISagaRepository<TSaga> repository, ISagaPolicy<TSaga, TMessage> policy,
		                              ISagaLocator<TMessage> locator, InstanceHandlerSelector<TSaga, TMessage> selector)
		{
			_selector = selector;
			_locator = locator;
			Repository = repository;
			Policy = policy;
		}

		public ISagaPolicy<TSaga, TMessage> Policy { get; private set; }
		public ISagaRepository<TSaga> Repository { get; private set; }

		public IEnumerable<Action<IConsumeContext<TMessage>>> Enumerate(IConsumeContext<TMessage> acceptContext)
		{
			foreach (Guid sagaId in _locator.Find(acceptContext))
			{
				foreach (var action in Repository.GetSaga(acceptContext, sagaId, _selector, Policy))
				{
					yield return action;
				}
			}
		}

		public bool Inspect(IPipelineInspector inspector)
		{
			return inspector.Inspect(this);
		}
	}
}