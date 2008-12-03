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
	using System.Reflection;

	public abstract class ConsumesInboundInterceptorBase :
		InboundInterceptorBase
	{
		protected abstract Type InterfaceType { get; }

		public override IEnumerable<Func<bool>> Subscribe<TComponent>(IInboundContext context)
		{
			foreach (Type messageType in GetInterfaces<TComponent>(context, InterfaceType))
			{
				MethodInfo genericMethod = FindMethod(GetType(), "Connect", new[] {typeof (TComponent), messageType}, new[] {typeof (IInboundContext)});

				if (genericMethod == null)
					throw new PipelineException(string.Format("Unable to subscribe for type: {0} ({1})",
					                                          typeof (TComponent).FullName, messageType.FullName));

				Func<bool> result = (Func<bool>) genericMethod.Invoke(this, new object[] {context});

				context.MessageTypeWasDefined(messageType);

				yield return result;
			}
		}

		public override IEnumerable<Func<bool>> Subscribe<TComponent>(IInboundContext context, TComponent instance)
		{
			foreach (Type messageType in GetInterfaces<TComponent>(context, InterfaceType))
			{
				MethodInfo genericMethod = FindMethod(GetType(), "Connect", new[] {messageType}, new[] {typeof (IInboundContext), typeof (TComponent)});

				if (genericMethod == null)
					throw new PipelineException(string.Format("Unable to subscribe for type: {0} ({1})",
					                                          typeof (TComponent).FullName, messageType.FullName));

				Func<bool> result = (Func<bool>) genericMethod.Invoke(this, new object[] {context, instance});

				context.MessageTypeWasDefined(messageType);

				yield return result;
			}
		}
	}
}