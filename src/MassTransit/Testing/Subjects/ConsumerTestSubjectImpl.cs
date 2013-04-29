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
	using Scenarios;
	using TestDecorators;

	public class ConsumerTestSubjectImpl<TScenario, TSubject> :
		ConsumerTestSubject<TSubject>
		where TSubject : class, IConsumer
	    where TScenario : TestScenario
	{
		readonly IConsumerFactory<TSubject> _consumerFactory;
		readonly ReceivedMessageListImpl _received;
		bool _disposed;
		UnsubscribeAction _unsubscribe;

		public ConsumerTestSubjectImpl(IConsumerFactory<TSubject> consumerFactory)
		{
			_consumerFactory = consumerFactory;

			_received = new ReceivedMessageListImpl();
		}

		public ReceivedMessageList Received
		{
			get { return _received; }
		}

		public void Dispose()
		{
			Dispose(true);
		}

		public void Prepare(TScenario scenario)
		{
			var decoratedConsumerFactory = new ConsumerFactoryTestDecorator<TSubject>(_consumerFactory, _received);

			_unsubscribe = scenario.InputBus.SubscribeConsumer(decoratedConsumerFactory);
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
	}
}