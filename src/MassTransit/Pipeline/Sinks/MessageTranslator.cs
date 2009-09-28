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
namespace MassTransit.Pipeline.Sinks
{
	using System;
	using System.Collections.Generic;

	public class MessageTranslator<TInput, TOutput> :
		PipelineSinkBase<TInput, TOutput>
		where TOutput : class, TInput
		where TInput : class
	{
		public MessageTranslator(IPipelineSink<TOutput> outputSink)
			: base(outputSink)
		{
		}

		public override IEnumerable<Action<TInput>> Enumerate(TInput message)
		{
			// If the message cannot be safely cast to the output type, we have nothing to return
			TOutput output = message as TOutput;
			if (output == null)
				yield break;

			foreach (var consumer in _outputSink.Enumerate(output))
			{
				Action<TOutput> consume = consumer;

				yield return x => consume((TOutput)x);
			}
		}

		public override bool Inspect(IPipelineInspector inspector)
		{
			return inspector.Inspect(this) && _outputSink.Inspect(inspector);
		}
	}
}