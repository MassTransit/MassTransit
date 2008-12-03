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
			foreach (Type messageType in GetInterfaces<TComponent>(context, typeof (Consumes<>.All)))
			{
				MethodInfo genericMethod = FindMethod(GetType(), "Connect", new Type[] { typeof(TComponent), messageType }, new Type[] { typeof(ISubscribeContext)});

				if (genericMethod == null)
					throw new PipelineException(string.Format("Unable to subscribe for type: {0} ({1})",
					                                          typeof (TComponent).FullName, messageType.FullName));

				Func<bool> result = (Func<bool>) genericMethod.Invoke(this, new object[] {context});

				context.MessageTypeWasDefined(messageType);

				yield return result;
			}
		}

		public IEnumerable<Func<bool>> Subscribe<TComponent>(ISubscribeContext context, TComponent instance)
		{
			foreach (Type messageType in GetInterfaces<TComponent>(context, typeof (Consumes<>.All)))
			{
				MethodInfo genericMethod = FindMethod(GetType(), "Connect", new Type[] {messageType}, new Type[] {typeof (ISubscribeContext), typeof (TComponent)});

				if (genericMethod == null)
					throw new PipelineException(string.Format("Unable to subscribe for type: {0} ({1})",
					                                          typeof (TComponent).FullName, messageType.FullName));

				Func<bool> result = (Func<bool>) genericMethod.Invoke(this, new object[] {context, instance});

				context.MessageTypeWasDefined(messageType);

				yield return result;
			}
		}

		private static IEnumerable<Type> GetInterfaces<TComponent>(ISubscribeContext context, Type consumerType)
		{
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

				yield return messageType;
			}
		}

		private static MethodInfo FindMethod(Type type,
		                               string methodName,
		                               Type[] typeArguments,
		                               Type[] parameterTypes)
		{
			MethodInfo methodInfo = null;

			if (null == parameterTypes)
			{
				methodInfo = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
				methodInfo = methodInfo.MakeGenericMethod(typeArguments);
			}
			else
			{
				// Method is probably overloaded. As far as I know there's no other way 
				// to get the MethodInfo instance, we have to
				// search for it in all the type methods
				MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
				foreach (MethodInfo method in methods)
				{
					if (method.Name == methodName)
					{
						// create the generic method
						if (method.GetGenericArguments().Length == typeArguments.Length)
						{
							MethodInfo genericMethod = method.MakeGenericMethod(typeArguments);
							ParameterInfo[] parameters = genericMethod.GetParameters();

							// compare the method parameters
							if (parameters.Length == parameterTypes.Length)
							{
								for (int i = 0; i < parameters.Length; i++)
								{
									if (parameters[i].ParameterType != parameterTypes[i])
									{
										continue; // this is not the method we're looking for
									}
								}

								// if we're here, we got the right method
								methodInfo = genericMethod;
								break;
							}
						}
					}
				}

				if (null == methodInfo)
				{
					throw new InvalidOperationException("Method not found");
				}

			}

			return methodInfo;
		}

		private Func<bool> Connect<TMessage>(ISubscribeContext context, Consumes<TMessage>.All consumer) where TMessage : class
		{
			var sink = new MessageSink<TMessage>(message => consumer);

			return context.Connect(sink);
		}

		private Func<bool> Connect<TComponent, TMessage>(ISubscribeContext context)
			where TMessage : class
			where TComponent : class, Consumes<TMessage>.All
		{
			var sink = new ComponentMessageSink<TComponent, TMessage>(context);

			return context.Connect(sink);
		}
	}
}