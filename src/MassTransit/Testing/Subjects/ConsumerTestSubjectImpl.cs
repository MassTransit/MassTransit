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
	using TestDecorators;

	public class ConsumerTestSubjectImpl<T> :
		ConsumerTestSubject<T>
		where T : class
	{
		readonly ReceivedMessageListImpl _received;
		IServiceBus _bus;
		IConsumerFactory<T> _consumerFactory;
		bool _disposed;
		UnsubscribeAction _unsubscribe;

		public ConsumerTestSubjectImpl(IConsumerFactory<T> consumerFactory)
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
			GC.SuppressFinalize(this);
		}

		public void Prepare(IServiceBus bus)
		{
			_bus = bus;

			var decoratedConsumerFactory = new ConsumerFactoryTestDecorator<T>(_consumerFactory, bus, _received);

			_unsubscribe = bus.SubscribeConsumer(decoratedConsumerFactory);
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

		~ConsumerTestSubjectImpl()
		{
			Dispose(false);
		}
	}
}