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
	using System.Linq.Expressions;
	using Magnum.Extensions;

	public class PropertySagaMessageSink<TSaga, TMessage> :
		SagaMessageSinkBase<TSaga, TMessage>
		where TSaga : class, ISaga, CorrelatedBy<Guid>, Consumes<TMessage>.All
		where TMessage : class
	{
		public PropertySagaMessageSink(ISagaRepository<TSaga> repository,
		                               ISagaPolicy<TSaga, TMessage> policy,
		                               Expression<Func<TSaga, TMessage, bool>> selector)
			: base(repository, policy, new PropertySagaLocator<TSaga, TMessage>(repository, policy, selector), GetHandlers)
		{
			Selector = selector;
		}

		public Expression<Func<TSaga, TMessage, bool>> Selector { get; private set; }

		static IEnumerable<Action<IConsumeContext<TMessage>>> GetHandlers(TSaga instance, IConsumeContext<TMessage> context)
		{
			yield return x =>
				{
					instance.Bus = context.Bus;

					context.BaseContext.NotifyConsume(context, typeof(TSaga).ToShortTypeName());
					
					using (x.CreateScope())
					{
						instance.Consume(x.Message);
					}
				};
		}
	}
}