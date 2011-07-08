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
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using Configuration;
	using Magnum.Reflection;

	public class StateMachineSagaWorkerConnector<T> :
		IEnumerable<SagaWorkerSubscriptionConnector>
		where T : SagaStateMachine<T>, ISaga
	{
		readonly ISagaPolicyFactory _policyFactory;
		readonly ISagaRepository<T> _sagaRepository;

		public StateMachineSagaWorkerConnector(ISagaRepository<T> sagaRepository)
		{
			_sagaRepository = sagaRepository;
			_policyFactory = new SagaPolicyFactory();
		}

		public IEnumerator<SagaWorkerSubscriptionConnector> GetEnumerator()
		{
			T instance = FastActivator<T>.Create(Guid.Empty);

			var inspector = new SagaStateMachineEventInspector<T>();
			instance.Inspect(inspector);

			return inspector.GetResults()
				.Select(
					x =>
						{
							return (SagaWorkerEventConnectorFactory) FastActivator.Create(typeof (SagaWorkerEventConnectorFactory<,>),
								new[] {typeof (T), x.SagaEvent.MessageType},
								new object[] {_sagaRepository, _policyFactory, x.SagaEvent.Event, x.States});
						})
				.SelectMany(x => x.Create())
				.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}