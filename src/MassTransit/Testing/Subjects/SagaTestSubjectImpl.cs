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
namespace MassTransit.Testing.Subjects
{
	using System;
	using Saga;
	using Scenarios;
	using TestDecorators;

	public class SagaTestSubjectImpl<TScenario, TSaga> :
		SagaTestSubject<TSaga>
		where TSaga : class, ISaga
		where TScenario : TestScenario
	{
		readonly ReceivedMessageListImpl _received;
		readonly ISagaRepository<TSaga> _sagaRepository;
		bool _disposed;
		UnsubscribeAction _unsubscribe;
		SagaListImpl<TSaga> _created;

		public SagaTestSubjectImpl(ISagaRepository<TSaga> sagaRepository)
		{
			_sagaRepository = sagaRepository;

			_received = new ReceivedMessageListImpl();
			_created = new SagaListImpl<TSaga>();
		}

		public ReceivedMessageList Received
		{
			get { return _received; }
		}

		public SagaList<TSaga> Created
		{
			get { return _created; }
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Prepare(TScenario scenario)
		{
			var decoratedSagaRepository = new SagaRepositoryTestDecorator<TSaga>(_sagaRepository, _received, _created);

			_unsubscribe = scenario.InputBus.SubscribeSaga(decoratedSagaRepository);
		}

		void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				if (_unsubscribe != null)
				{
					_unsubscribe();
					_unsubscribe = null;
				}

				_received.Dispose();
			}

			_disposed = true;
		}

		~SagaTestSubjectImpl()
		{
			Dispose(false);
		}
	}
}