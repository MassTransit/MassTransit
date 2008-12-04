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
namespace MassTransit.Pipeline.Interceptors.Inbound
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using Configuration;
	using Exceptions;
	using Sinks;

	public class ConsumesForInboundInterceptor :
		InboundInterceptorBase
	{
		private static readonly Type _interfaceType = typeof (Consumes<>.For<>);

		protected virtual Func<bool> Connect<TMessage, TKey>(IInboundContext context, Consumes<TMessage>.For<TKey> consumer)
			where TMessage : class, CorrelatedBy<TKey>
		{
			var correlatedConfigurator = CorrelatedMessageRouterConfigurator.For(context.Pipeline);

			return correlatedConfigurator.FindOrCreate<TMessage, TKey>().Connect(consumer.CorrelationId, new InstanceMessageSink<TMessage>(message => consumer));
		}

		public override IEnumerable<Func<bool>> Subscribe<TComponent>(IInboundContext context)
		{
			yield break;
		}

		public override IEnumerable<Func<bool>> Subscribe<TComponent>(IInboundContext context, TComponent instance)
		{
			foreach (KeyValuePair<Type, Type> match in GetInterfaces<TComponent>(context))
			{
				MethodInfo genericMethod = FindMethod(GetType(), "Connect", new[] {match.Key, match.Value}, new[] {typeof (IInboundContext), typeof (TComponent)});

				if (genericMethod == null)
					throw new PipelineException(string.Format("Unable to subscribe for type: {0} ({1})",
					                                          typeof (TComponent).FullName, match.Key.FullName));

				Func<bool> result = (Func<bool>) genericMethod.Invoke(this, new object[] {context, instance});

				context.MessageTypeWasDefined(match.Key);

				yield return result;
			}
		}

		protected static IEnumerable<KeyValuePair<Type, Type>> GetInterfaces<TComponent>(IInboundContext context)
		{
			Type componentType = typeof (TComponent);

			foreach (Type interfaceType in componentType.GetInterfaces())
			{
				if (!interfaceType.IsGenericType)
					continue;

				Type genericType = interfaceType.GetGenericTypeDefinition();

				if (genericType != _interfaceType)
					continue;

				Type[] types = interfaceType.GetGenericArguments();

				Type messageType = types[0];

				if (context.HasMessageTypeBeenDefined(messageType))
					continue;

				Type keyType = types[1];

				// TODO if we have a generic type, we need to look for a generic message handler
				if (messageType.IsGenericType)
				{
				}

				yield return new KeyValuePair<Type, Type>(messageType, keyType);
			}
		}
	}
}