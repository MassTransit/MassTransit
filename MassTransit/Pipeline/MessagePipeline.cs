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
	using Sinks;

	public class MessagePipeline :
		PipelineSinkBase<object, object>,
		IMessagePipeline
	{
		private readonly IConfigurePipeline _configurator;

		public MessagePipeline(IPipelineSink<object> outputSink, IConfigurePipeline configurator) :
			base(outputSink)
		{
			_configurator = configurator;
		}

		public override IEnumerable<Action<object>> Enumerate(object message)
		{
			foreach (Action<object> consumer in _outputSink.Enumerate(message))
			{
				yield return consumer;
			}
		}

		public override bool Inspect(IPipelineInspector inspector)
		{
			return inspector.Inspect(this, () => _outputSink.Inspect(inspector));
		}

		public void Configure(Action<IConfigurePipeline> action)
		{
			action(_configurator);
		}

		public V Configure<V>(Func<IConfigurePipeline, V> action)
		{
			return action(_configurator);
		}
	}
}