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
	using System.Linq;
	using System.Linq.Expressions;
	using MassTransit.Pipeline;

	public abstract class SagaMessageSinkBase<TSaga, TMessage> :
		ISagaMessageSink<TSaga, TMessage>
		where TSaga : class, ISaga
		where TMessage : class
	{
		bool _disposed;

		protected SagaMessageSinkBase(IServiceBus bus,
		                              ISagaRepository<TSaga> repository,
		                              ISagaPolicy<TSaga, TMessage> policy)
		{
			Bus = bus;
			Repository = repository;
			Policy = policy;
		}

		public ISagaPolicy<TSaga, TMessage> Policy { get; private set; }
		public ISagaRepository<TSaga> Repository { get; private set; }
		public IServiceBus Bus { get; private set; }

		protected abstract Expression<Func<TSaga, TMessage, bool>> FilterExpression { get; }

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public IEnumerable<Action<TMessage>> Enumerate(TMessage itemxxx)
		{
			yield return message =>
				{
					Expression<Func<TSaga, bool>> filter = CreateFilterExpressionForMessage(message);

					Repository.Send(filter, Policy, message, saga =>
						{
							saga.Bus = Bus;

							ConsumerAction(saga, message);
						});
				};
		}

		public bool Inspect(IPipelineInspector inspector)
		{
			return inspector.Inspect(this);
		}

		protected abstract void ConsumerAction(TSaga saga, TMessage message);

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
			}

			_disposed = true;
		}

		protected Expression<Func<TSaga, TMessage, bool>> CreateCorrelatedSelector()
		{
			ParameterExpression saga = Expression.Parameter(typeof (TSaga), "saga");
			ParameterExpression message = Expression.Parameter(typeof (TMessage), "message");

			MemberExpression sagaId = Expression.Property(saga,
				typeof (TSaga).GetProperties().Where(x => x.Name == "CorrelationId").First());
			MemberExpression messageId = Expression.Property(message,
				typeof (TMessage).GetProperties().Where(x => x.Name == "CorrelationId").First());

			BinaryExpression comparison = Expression.Equal(sagaId, messageId);

			return Expression.Lambda<Func<TSaga, TMessage, bool>>(comparison, new[] {saga, message});
		}

		Expression<Func<TSaga, bool>> CreateFilterExpressionForMessage(TMessage message)
		{
			return new SagaFilterExpressionConverter<TSaga, TMessage>(message).Convert(FilterExpression);
		}

		~SagaMessageSinkBase()
		{
			Dispose(false);
		}
	}
}