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
	using Logging;
	using Magnum.Extensions;
	using Magnum.StateMachine;
	using Util;

	public class CorrelatedSagaStateMachineMessageSink<TSaga, TMessage> :
		SagaMessageSinkBase<TSaga, TMessage>
		where TMessage : class, CorrelatedBy<Guid>
		where TSaga : SagaStateMachine<TSaga>, ISaga
	{
		static readonly ILog _log =
			Logger.Get(typeof (CorrelatedSagaStateMachineMessageSink<TSaga, TMessage>).ToFriendlyName());

		public CorrelatedSagaStateMachineMessageSink(ISagaRepository<TSaga> repository,
		                                             ISagaPolicy<TSaga, TMessage> policy,
		                                             DataEvent<TSaga, TMessage> dataEvent)
			: base(repository, policy, new CorrelatedSagaLocator<TMessage>(), (s, c) => GetHandlers(s, c, dataEvent))
		{
		}

		static IEnumerable<Action<IConsumeContext<TMessage>>> GetHandlers(TSaga instance, IConsumeContext<TMessage> context,
		                                                                  DataEvent<TSaga, TMessage> dataEvent)
		{
			yield return x =>
				{
					instance.Bus = context.Bus;

					context.BaseContext.NotifyConsume(context, typeof(TSaga).ToShortTypeName(), instance.CorrelationId.ToString());

					using (x.CreateScope())
					{
						if (_log.IsDebugEnabled)
							_log.DebugFormat("RaiseEvent: {0} {1} {2}", typeof (TSaga).Name, dataEvent.Name, instance.CorrelationId);

						instance.RaiseEvent(dataEvent, x.Message);
					}
				};
		}
	}
}