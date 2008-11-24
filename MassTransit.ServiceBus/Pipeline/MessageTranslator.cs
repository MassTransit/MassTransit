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
namespace MassTransit.ServiceBus.Pipeline
{
	using System.Collections.Generic;

	public class MessageTranslator<TInput, TOutput> :
		IMessageSink<TInput>
		where TOutput : class, TInput
		where TInput : class
	{
		private readonly IMessageSink<TOutput> _outputSink;

		public MessageTranslator(IMessageSink<TOutput> outputSink)
		{
			_outputSink = outputSink;
		}

		public IEnumerable<Consumes<TInput>.All> Enumerate(TInput message)
		{
			TOutput output = TranslateTo<TOutput>.From(message);

			foreach (Consumes<TOutput>.All consumer in _outputSink.Enumerate(output))
			{
				yield return Consumes<TInput>.WidenTo<TOutput>.For(consumer);
			}
		}

		public bool Inspect(IPipelineInspector inspector)
		{
			inspector.Inspect(this);

			return _outputSink.Inspect(inspector);
		}
	}
}