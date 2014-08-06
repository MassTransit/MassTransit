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


    public class OutboundMessageFilter<TMessage> :
		OutboundPipelineSink<TMessage>
		where TMessage : class
	{
		readonly Func<IBusPublishContext<TMessage>, bool> _accept;
		readonly OutboundPipelineSink<TMessage> _output;

		public OutboundMessageFilter(OutboundPipelineSink<TMessage> output, Func<IBusPublishContext<TMessage>, bool> accept)
		{
			_output = output;
			_accept = accept;
		}

		public IEnumerable<Action<IBusPublishContext<TMessage>>> Enumerate(IBusPublishContext<TMessage> context)
		{
			if (_accept(context))
			{
				return _output.Enumerate(context);
			}

			return Enumerable.Empty<Action<IBusPublishContext<TMessage>>>();
		}

		public bool Inspect(IPipelineInspector inspector)
		{
			return inspector.Inspect(this) && _output.Inspect(inspector);
		}
	}
}