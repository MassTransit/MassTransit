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
namespace MassTransit.Pipeline.Interceptors
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using Configuration;
	using Exceptions;
	using Sinks;

	public class BatchInterceptor :
		PipelineInterceptorBase
	{
		private static readonly Type _interfaceType = typeof (Consumes<>.All);
		private static readonly Type _batchType = typeof(BatchMessage<,>);

		protected virtual Func<bool> Connect<TMessage, TBatchId>(IInterceptorContext context, Consumes<BatchMessage<TMessage, TBatchId>>.All consumer)
			where TMessage : class, BatchedBy<TBatchId>
		{
			var configurator = BatchMessageRouterConfigurator.For(context.Pipeline);

			return configurator.FindOrCreate<TMessage, TBatchId>().Connect(new InstanceMessageSink<BatchMessage<TMessage, TBatchId>>(message => consumer));
		}

		public override IEnumerable<Func<bool>> Subscribe<TComponent>(IInterceptorContext context)
		{
			yield break;
		}

		public override IEnumerable<Func<bool>> Subscribe<TComponent>(IInterceptorContext context, TComponent instance)
		{
			foreach (KeyValuePair<Type, Type> match in GetInterfaces<TComponent>(context))
			{
				MethodInfo genericMethod = FindMethod(GetType(), "Connect", new[] {match.Key, match.Value}, new[] {typeof (IInterceptorContext), typeof (TComponent)});

				if (genericMethod == null)
					throw new PipelineException(string.Format("Unable to subscribe for type: {0} ({1})",
					                                          typeof (TComponent).FullName, match.Key.FullName));

				Func<bool> result = (Func<bool>) genericMethod.Invoke(this, new object[] {context, instance});

				context.MessageTypeWasDefined(match.Key);
				context.MessageTypeWasDefined(_batchType.MakeGenericType(match.Key, match.Value));

				yield return result;
			}
		}

		protected static IEnumerable<KeyValuePair<Type, Type>> GetInterfaces<TComponent>(IInterceptorContext context)
		{
			Type componentType = typeof (TComponent);

			foreach (Type interfaceType in componentType.GetInterfaces())
			{
				if (!interfaceType.IsGenericType) continue;

				Type genericType = interfaceType.GetGenericTypeDefinition();
				if (genericType != _interfaceType) continue;

				Type[] types = interfaceType.GetGenericArguments();

				Type messageType = types[0];
				if (context.HasMessageTypeBeenDefined(messageType)) continue;

				if (!messageType.IsGenericType || messageType.GetGenericTypeDefinition() != _batchType) continue;

				Type[] genericArguments = messageType.GetGenericArguments();

				yield return new KeyValuePair<Type, Type>(genericArguments[0], genericArguments[1]);
			}
		}
	}
}