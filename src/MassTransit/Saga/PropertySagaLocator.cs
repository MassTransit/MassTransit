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
namespace MassTransit.Saga
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using Pipeline;

	public class PropertySagaLocator<TSaga, TMessage> :
		ISagaLocator<TMessage>
		where TSaga : class, ISaga
		where TMessage : class
	{
		readonly Expression<Func<TSaga, TMessage, bool>> _filterExpression;
		readonly ISagaRepository<TSaga> _repository;
		readonly ISagaPolicy<TSaga, TMessage> _policy;

		public PropertySagaLocator(ISagaRepository<TSaga> repository, ISagaPolicy<TSaga,TMessage> policy, Expression<Func<TSaga, TMessage, bool>> filterExpression)
		{
			_repository = repository;
			_policy = policy;
			_filterExpression = filterExpression;
		}

		public IEnumerable<Guid> Find(IConsumeContext<TMessage> context)
		{
			Expression<Func<TSaga, bool>> filter =
				new SagaFilterExpressionConverter<TSaga, TMessage>(context.Message).Convert(_filterExpression);

			var sagaFilter = new SagaFilter<TSaga>(filter);

			int count = 0;
			foreach(var sagaId in  _repository.Where(sagaFilter, x => x.CorrelationId))
			{
				yield return sagaId;
				count++;
			}

			if(count == 0)
			{
				if(_policy.CanCreateInstance(context))
				{
					yield return _policy.GetNewSagaId(context);
				}
			}
		}
	}
}