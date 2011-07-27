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
namespace MassTransit.Testing.TestDecorators
{
	using System;
	using System.Collections.Generic;
	using Pipeline;
	using Saga;

	public class SagaRepositoryTestDecorator<TSaga> :
		ISagaRepository<TSaga>
		where TSaga : class, ISaga
	{
		readonly ReceivedMessageListImpl _received;
		readonly ISagaRepository<TSaga> _sagaRepository;
		SagaListImpl<TSaga> _created;

		public SagaRepositoryTestDecorator(ISagaRepository<TSaga> sagaRepository,
		                                   ReceivedMessageListImpl received, SagaListImpl<TSaga> created)
		{
			_sagaRepository = sagaRepository;
			_received = received;
			_created = created;
		}

		public IEnumerable<Action<IConsumeContext<TMessage>>> GetSaga<TMessage>(IConsumeContext<TMessage> context, Guid sagaId,
		                                                                        InstanceHandlerSelector<TSaga, TMessage>
		                                                                        	selector, ISagaPolicy<TSaga, TMessage> policy)
			where TMessage : class
		{
			ISagaPolicy<TSaga, TMessage> decoratedSagaPolicy = new SagaPolicyTestDecorator<TSaga, TMessage>(policy, sagaId,
				_created);

			IEnumerable<Action<IConsumeContext<TMessage>>> consumers = _sagaRepository.GetSaga(context, sagaId, selector,
				decoratedSagaPolicy);

			foreach (var action in consumers)
			{
				Action<IConsumeContext<TMessage>> consumer = action;

				yield return message =>
					{
						var received = new ReceivedMessageImpl<TMessage>(message);

						try
						{
							consumer(message);
						}
						catch (Exception ex)
						{
							received.SetException(ex);
						}
						finally
						{
							_received.Add(received);
						}
					};
			}
		}

		public IEnumerable<Guid> Find(ISagaFilter<TSaga> filter)
		{
			return _sagaRepository.Find(filter);
		}

		public IEnumerable<TSaga> Where(ISagaFilter<TSaga> filter)
		{
			return _sagaRepository.Where(filter);
		}

		public IEnumerable<TResult> Where<TResult>(ISagaFilter<TSaga> filter, Func<TSaga, TResult> transformer)
		{
			return _sagaRepository.Where(filter, transformer);
		}

		public IEnumerable<TResult> Select<TResult>(Func<TSaga, TResult> transformer)
		{
			return _sagaRepository.Select(transformer);
		}
	}
}