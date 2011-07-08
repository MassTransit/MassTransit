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
namespace MassTransit.Saga.SubscriptionConfigurators
{
	using System.Collections.Generic;
	using Configurators;
	using MassTransit.SubscriptionBuilders;
	using MassTransit.SubscriptionConfigurators;
	using SubscriptionBuilders;

	public class SagaSubscriptionConfiguratorImpl<TSaga> :
		SubscriptionConfiguratorImpl<SagaSubscriptionConfigurator<TSaga>>,
		SagaSubscriptionConfigurator<TSaga>,
		SubscriptionBuilderConfigurator
		where TSaga : class, ISaga
	{
		readonly ISagaRepository<TSaga> _sagaRepository;

		public SagaSubscriptionConfiguratorImpl(ISagaRepository<TSaga> sagaRepository)
		{
			_sagaRepository = sagaRepository;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (_sagaRepository == null)
				yield return this.Failure("The saga repository cannot be null. How else are we going to save stuff? #facetopalm");
		}

		public SubscriptionBuilder Configure()
		{
			return new SagaSubscriptionBuilder<TSaga>(_sagaRepository, ReferenceFactory);
		}
	}
}