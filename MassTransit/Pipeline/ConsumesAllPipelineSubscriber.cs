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

	public class ConsumesAllPipelineSubscriber :
		ISubscribeInterceptor
	{
		public IEnumerable<Func<bool>> Subscribe<TComponent>(ISubscribeContext context)
		{
			Type consumerType = typeof (Consumes<>.All);

			Type componentType = typeof (TComponent);

			foreach (Type interfaceType in componentType.GetInterfaces())
			{
				if (!interfaceType.IsGenericType)
					continue;

				Type genericType = interfaceType.GetGenericTypeDefinition();

				if (genericType != consumerType)
					continue;

				Type[] types = interfaceType.GetGenericArguments();

				Type messageType = types[0];

				if (context.HasMessageTypeBeenDefined(messageType))
					continue;

				// TODO if we have a generic type, we need to look for a generic message handler
				if (messageType.IsGenericType)
				{
				}
			}

			yield break;
		}

		public IEnumerable<Func<bool>> Subscribe<TComponent>(ISubscribeContext context, TComponent instance)
		{
			Type consumerType = typeof(Consumes<>.All);

			Type componentType = typeof(TComponent);

			foreach (Type interfaceType in componentType.GetInterfaces())
			{
				if (!interfaceType.IsGenericType)
					continue;

				Type genericType = interfaceType.GetGenericTypeDefinition();

				if (genericType != consumerType)
					continue;

				Type[] types = interfaceType.GetGenericArguments();

				Type messageType = types[0];

				if (context.HasMessageTypeBeenDefined(messageType))
					continue;

				// TODO if we have a generic type, we need to look for a generic message handler
				if (messageType.IsGenericType)
				{
				}

				MethodInfo mi = GetType().GetMethod("Connect", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
				MethodInfo genericMethod = mi.MakeGenericMethod(messageType);

				if (genericMethod != null)
				{
					Func<bool> result = (Func<bool>)genericMethod.Invoke(this, new object[] { context, instance });

					context.MessageTypeWasDefined(messageType);

					yield return result;
				}
			}

			yield break;
		}

		private Func<bool> Connect<TMessage>(ISubscribeContext context, Consumes<TMessage>.All consumer) where TMessage : class
		{
			var sink = new MessageSink<TMessage>(message => consumer);

			return context.Connect(sink);

		}
	}
}