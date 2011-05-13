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
namespace MassTransit.Pipeline.Sinks
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Context;
	using Magnum.Concurrency;

	/// <summary>
	/// Splits a message path based on the correlation information in the message
	/// </summary>
	/// <typeparam name="TMessage">The type of the message to be routed</typeparam>
	/// <typeparam name="TKey">They key type for the message</typeparam>
	/// <typeparam name="T"></typeparam>
	public class CorrelatedMessageRouter<T, TMessage, TKey> :
		IPipelineSink<T>
		where TMessage : class, CorrelatedBy<TKey>
		where T : class, IMessageContext<TMessage>
	{
		readonly Atomic<Dictionary<TKey, CorrelatedMessageSinkRouter<T, TMessage, TKey>>> _output;

		public CorrelatedMessageRouter()
		{
			_output = Atomic.Create(new Dictionary<TKey, CorrelatedMessageSinkRouter<T, TMessage,TKey>>());
		}

		public int SinkCount(TKey key)
		{
			CorrelatedMessageSinkRouter<T,TMessage,TKey> router;
			if (!_output.Value.TryGetValue(key, out router))
				return 0;

			return router.SinkCount;
		}

		public IEnumerable<Action<T>> Enumerate(T context)
		{
			TKey key = context.Message.CorrelationId;

			CorrelatedMessageSinkRouter<T,TMessage, TKey> output;
			if (!_output.Value.TryGetValue(key, out output))
				return Enumerable.Empty<Action<T>>();

			return output.Enumerate(context);
		}

		public bool Inspect(IPipelineInspector inspector)
		{
			return inspector.Inspect(this, () => _output.Value.Values.All(x => x.Inspect(inspector)));
		}

		public UnsubscribeAction Connect(TKey correlationId, IPipelineSink<T> sink)
		{
			_output.Set(sinks =>
				{
					CorrelatedMessageSinkRouter<T, TMessage, TKey> keySink;
					if (sinks.TryGetValue(correlationId, out keySink) == false)
					{
						keySink = new CorrelatedMessageSinkRouter<T,TMessage, TKey>(correlationId);
						keySink.Connect(sink);

						return new Dictionary<TKey, CorrelatedMessageSinkRouter<T, TMessage, TKey>>(sinks) { { correlationId, keySink } };
					}

					var result = new Dictionary<TKey, CorrelatedMessageSinkRouter<T, TMessage, TKey>>(sinks);

					keySink = new CorrelatedMessageSinkRouter<T, TMessage, TKey>(correlationId, keySink.Sinks);
					keySink.Connect(sink);
					result[correlationId] = keySink;

					return result;
				});

			return () => Disconnect(correlationId, sink);
		}

		bool Disconnect(TKey correlationId, IPipelineSink<T> sink)
		{
			return _output.Set(sinks =>
				{
					CorrelatedMessageSinkRouter<T, TMessage, TKey> keySink;
					if (sinks.TryGetValue(correlationId, out keySink) == false)
						return sinks;

					var result = new Dictionary<TKey, CorrelatedMessageSinkRouter<T, TMessage, TKey>>(sinks);

					List<IPipelineSink<T>> outputSinks = keySink.Sinks.Where(x => x != sink).ToList();
					if (outputSinks.Count == 0)
						result.Remove(correlationId);
					else
						result[correlationId] = new CorrelatedMessageSinkRouter<T, TMessage, TKey>(correlationId, outputSinks);

					return result;
				}) != null;
		}
	}
}