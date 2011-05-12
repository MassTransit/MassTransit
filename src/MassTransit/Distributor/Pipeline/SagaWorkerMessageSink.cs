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
namespace MassTransit.Distributor.Pipeline
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Context;
	using MassTransit.Pipeline;
	using Messages;
	using Saga;

	public class SagaWorkerMessageSink<TSaga, TMessage> :
		IPipelineSink<IConsumeContext<Distributed<TMessage>>>
		where TSaga : class, ISaga
		where TMessage : class
	{
		readonly ISagaWorker<TSaga> _worker;
		ISagaMessageSink<TSaga, TMessage> _sink;

		public SagaWorkerMessageSink(ISagaWorker<TSaga> worker, ISagaMessageSink<TSaga, TMessage> sink)
		{
			_worker = worker;
			_sink = sink;
		}

		public IEnumerable<Action<IConsumeContext<Distributed<TMessage>>>> Enumerate(
			IConsumeContext<Distributed<TMessage>> context)
		{
			if (!_worker.CanAcceptMessage(context.Message))
				return Enumerable.Empty<Action<IConsumeContext<Distributed<TMessage>>>>();

			var payloadContext = new ConsumeContext<TMessage>(context, context.Message.Payload);
			payloadContext.SetResponseAddress(context.Message.ResponseAddress);

			return _sink.Enumerate(payloadContext).Select(action => (Action<IConsumeContext<Distributed<TMessage>>>) (m =>
				{
					_worker.IncrementInProgress();
					try
					{
						_worker.ConsumingMessage(context.Message);

						action(payloadContext);
					}
					finally
					{
						_worker.DecrementInProgress();
					}
				}));
		}

		public bool Inspect(IPipelineInspector inspector)
		{
			return inspector.Inspect(this, () => _sink.Inspect(inspector));
		}
	}
}