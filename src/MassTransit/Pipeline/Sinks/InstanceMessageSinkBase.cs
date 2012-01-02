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
	using Magnum.Extensions;

	/// <summary>
	/// Handles 
	/// </summary>
	/// <typeparam name="TMessage"></typeparam>
	public class InstanceMessageSinkBase<TMessage> :
		IPipelineSink<IConsumeContext<TMessage>>
		where TMessage : class
	{
		readonly MultipleHandlerSelector<TMessage> _selector;

		public InstanceMessageSinkBase(MultipleHandlerSelector<TMessage> selector)
		{
			_selector = selector;
		}

		public IEnumerable<Action<IConsumeContext<TMessage>>> Enumerate(IConsumeContext<TMessage> context)
		{
			return Selector(context);
		}

		public bool Inspect(IPipelineInspector inspector)
		{
			inspector.Inspect(this);
			return true;
		}

		IEnumerable<Action<IConsumeContext<TMessage>>> Selector(IConsumeContext<TMessage> messageContext)
		{
			foreach (var result in _selector(messageContext))
			{
				messageContext.BaseContext.NotifyConsume(messageContext, typeof (Action<TMessage>).ToShortTypeName(), null);

				yield return result;
			}
		}
	}
}