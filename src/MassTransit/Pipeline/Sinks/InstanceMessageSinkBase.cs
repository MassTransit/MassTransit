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
	using Context;

	public class InstanceMessageSinkBase<TMessage> :
		IPipelineSink<IConsumeContext<TMessage>>
		where TMessage : class
	{
		readonly Func<TMessage, Action<TMessage>> _acceptor;

		public InstanceMessageSinkBase(Func<TMessage, Action<TMessage>> acceptor)
		{
			_acceptor = acceptor;
		}

		public IEnumerable<Action<IConsumeContext<TMessage>>> Enumerate(IConsumeContext<TMessage> context)
		{
			Action<TMessage> consumer;
			using (ContextStorage.CreateContextScope(context))
			{
				consumer = _acceptor(context.Message);
			}

			if (consumer != null)
				yield return x =>
					{
						using (ContextStorage.CreateContextScope(context))
						{
							consumer(context.Message);
						}
					};
		}

		public bool Inspect(IPipelineInspector inspector)
		{
			inspector.Inspect(this);
			return true;
		}
	}
}