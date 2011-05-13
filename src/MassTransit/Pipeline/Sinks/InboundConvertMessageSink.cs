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

	public class InboundConvertMessageSink<TMessage> :
		IPipelineSink<IConsumeContext>
		where TMessage : class
	{
		readonly IPipelineSink<IConsumeContext<TMessage>> _output;

		public InboundConvertMessageSink(IPipelineSink<IConsumeContext<TMessage>> output)
		{
			_output = output;
		}

		public IEnumerable<Action<IConsumeContext>> Enumerate(IConsumeContext context)
		{
			IConsumeContext<TMessage> outputContext;
			if (!context.TryGetContext(out outputContext))
				return Enumerable.Empty<Action<IConsumeContext>>();

			using (ContextStorage.CreateContextScope(outputContext))
			{
				return _output.Enumerate(outputContext).Select(consumer => (Action<IConsumeContext>) (x =>
					{
						try
						{
							consumer(outputContext);
						}
						catch (Exception ex)
						{
							outputContext.GenerateFault(ex);
							throw;
						}
					}));
			}
		}

		public bool Inspect(IPipelineInspector inspector)
		{
			return inspector.Inspect(this) && _output.Inspect(inspector);
		}
	}
}