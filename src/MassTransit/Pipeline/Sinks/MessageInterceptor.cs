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

	public class MessageInterceptor :
		PipelineSinkBase<object, object>
	{
		readonly IMessageInterceptor _interceptor;

		public MessageInterceptor(IMessageInterceptor interceptor)
			: base(null)
		{
			_interceptor = interceptor;
		}

		public override IEnumerable<Action<object>> Enumerate(object message)
		{
			_interceptor.PreDispatch(message);

			foreach (var consumer in _outputSink.Enumerate(message))
			{
				yield return consumer;
			}

			_interceptor.PostDispatch(message);
		}

		public override bool Inspect(IPipelineInspector inspector)
		{
			return inspector.Inspect(this) && _outputSink.Inspect(inspector);
		}
	}
}