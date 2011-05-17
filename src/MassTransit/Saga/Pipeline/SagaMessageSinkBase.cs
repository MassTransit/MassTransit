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
	using Context;
	using MassTransit.Pipeline;

	public abstract class SagaMessageSinkBase<TSaga, TMessage> :
		ISagaMessageSink<TSaga, TMessage>
		where TSaga : class, ISaga
		where TMessage : class
	{
		protected SagaMessageSinkBase(ISagaRepository<TSaga> repository, ISagaPolicy<TSaga, TMessage> policy)
		{
			Repository = repository;
			Policy = policy;
		}

		protected abstract Expression<Func<TSaga, TMessage, bool>> FilterExpression { get; }

		public ISagaPolicy<TSaga, TMessage> Policy { get; private set; }
		public ISagaRepository<TSaga> Repository { get; private set; }

		public IEnumerable<Action<IConsumeContext<TMessage>>> Enumerate(IConsumeContext<TMessage> acceptContext)
		{
			yield return context =>
				{
					Expression<Func<TSaga, bool>> filter = CreateFilterExpressionForMessage(context.Message);

					Repository.Send(filter, Policy, context.Message, saga =>
						{
							saga.Bus = context.Bus;

							using (ContextStorage.CreateContextScope(context))
							{
								ConsumerAction(saga, context.Message);
							}
						});
				};
		}

		public bool Inspect(IPipelineInspector inspector)
		{
			return inspector.Inspect(this);
		}

		protected abstract void ConsumerAction(TSaga saga, TMessage message);

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
	}
}