// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Pipeline
{
	using System;
	using System.Collections.Generic;

	public class MessagePipeline :
		MessageSinkBase<object, object>
	{
		private MessagePipeline(IMessageSink<object> outputSink) :
			base(outputSink)
		{
		}

		public override IEnumerable<Consumes<object>.All> Enumerate(object message)
		{
			foreach (Consumes<object>.All consumer in _outputSink.ReadLock(x => x.Enumerate(message)))
			{
				yield return consumer;
			}
		}

		public override bool Inspect(IPipelineInspector inspector)
		{
			inspector.Inspect(this);

			return _outputSink.ReadLock(x => x.Inspect(inspector));
		}

		public V Configure<V>(Func<MessagePipeline, V> action)
		{
			V result = default(V);

			_outputSink.WriteLock(x => { result = action(this); });

			return result;
		}

		public void Dispatch(object message)
		{
			Dispatch(message, x => true);
		}

		public void Dispatch(object message, Func<object, bool> accept)
		{
			foreach (Consumes<object>.All consumer in Enumerate(message))
			{
				if (!accept(message))
					break;

				accept = x => true;

				consumer.Consume(message);
			}
		}

		public static MessagePipeline CreateDefaultPipeline()
		{
			MessageRouter<object> router = new MessageRouter<object>();

			MessagePipeline pipeline = new MessagePipeline(router);

			return pipeline;
		}
	}
}