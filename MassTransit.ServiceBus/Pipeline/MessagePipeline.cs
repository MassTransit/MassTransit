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

	public class MessagePipeline<TMessage> :
		IMessageSink<TMessage>
		where TMessage : class
	{
		private readonly IMessageSink<TMessage> _outputSink;

		public MessagePipeline(IMessageSink<TMessage> outputSink)
		{
			_outputSink = outputSink;
		}

		public IEnumerable<Consumes<TMessage>.All> Enumerate(TMessage message)
		{
			foreach (Consumes<TMessage>.All consumer in _outputSink.Enumerate(message))
			{
				yield return consumer;
			}
		}

		public void Dispatch(TMessage message)
		{
			Dispatch(message, x => true);
		}

		public void Dispatch(TMessage message, Func<TMessage, bool> accept)
		{
			foreach (Consumes<TMessage>.All consumer in Enumerate(message))
			{
				if (!accept(message))
					break;

				accept = x => true;

				consumer.Consume(message);
			}
		}

		public bool Inspect(IPipelineInspector inspector)
		{
			inspector.Inspect(this);

			return _outputSink.Inspect(inspector);
		}
	}
}