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
	using System.Linq.Expressions;
	using System.Reflection;
	using Configuration;
	using Exceptions;
	using Sinks;
	using Util;

	public class BatchInterceptor :
		PipelineInterceptorBase
	{
		private static readonly Type _batchType = typeof (BatchMessage<,>);
		private static readonly Type _interfaceType = typeof (Consumes<>.All);

		private readonly ReaderWriterLockedDictionary<Type, Func<BatchInterceptor, IInterceptorContext, Func<bool>>> _components;
		private readonly ReaderWriterLockedDictionary<Type, Func<BatchInterceptor, IInterceptorContext, object, Func<bool>>> _instances;

		public BatchInterceptor()
		{
			_instances = new ReaderWriterLockedDictionary<Type, Func<BatchInterceptor, IInterceptorContext, object, Func<bool>>>();
			_components = new ReaderWriterLockedDictionary<Type, Func<BatchInterceptor, IInterceptorContext, Func<bool>>>();
		}

		protected virtual Func<bool> Connect<TMessage, TBatchId>(IInterceptorContext context, Consumes<BatchMessage<TMessage, TBatchId>>.All consumer)
			where TMessage : class, BatchedBy<TBatchId>
		{
			var configurator = BatchMessageRouterConfigurator.For(context.Pipeline);

			var router = configurator.FindOrCreate<TMessage, TBatchId>();

			var result = router.Connect(new InstanceMessageSink<BatchMessage<TMessage, TBatchId>>(message => consumer));

			Func<bool> remove = context.SubscribedTo(typeof (TMessage));

			return () => result() && (router.SinkCount == 0) && remove();
		}

		protected virtual Func<bool> Connect<TComponent, TMessage, TBatchId>(IInterceptorContext context)
			where TMessage : class, BatchedBy<TBatchId>
			where TComponent : class, Consumes<BatchMessage<TMessage, TBatchId>>.All
		{
			var configurator = BatchMessageRouterConfigurator.For(context.Pipeline);

			var router = configurator.FindOrCreate<TMessage, TBatchId>();

			var result = router.Connect(new ComponentMessageSink<TComponent, BatchMessage<TMessage, TBatchId>>(context));

			Func<bool> remove = context.SubscribedTo(typeof (TMessage));

			return () => result() && (router.SinkCount == 0) && remove();
		}


		public override IEnumerable<Func<bool>> Subscribe<TComponent>(IInterceptorContext context)
		{
			Func<BatchInterceptor, IInterceptorContext, Func<bool>> invoker = GetInvoker<TComponent>();
			if (invoker == null)
				yield break;

			yield return invoker(this, context);
		}

		public override IEnumerable<Func<bool>> Subscribe<TComponent>(IInterceptorContext context, TComponent instance)
		{
			Func<BatchInterceptor, IInterceptorContext, object, Func<bool>> invoker = GetInvokerForInstance<TComponent>();
			if (invoker == null)
				yield break;

			yield return invoker(this, context, instance);
		}


		private Func<BatchInterceptor, IInterceptorContext, object, Func<bool>> GetInvokerForInstance<TComponent>()
		{
			Type componentType = typeof (TComponent);

			return _instances.Retrieve(componentType, () =>
				{
					Func<BatchInterceptor, IInterceptorContext, object, Func<bool>> invoker = null;

					// since we don't have it, we're going to build it

					foreach (Type interfaceType in componentType.GetInterfaces())
					{
						Type messageType;
						Type keyType;
						if (!GetMessageType(interfaceType, out messageType, out keyType))
							continue;

						var typeArguments = new[] {messageType, keyType};
						MethodInfo genericMethod = FindMethod(GetType(), "Connect", typeArguments, new[] {typeof (IInterceptorContext), typeof (TComponent)});
						if (genericMethod == null)
							throw new PipelineException(string.Format("Unable to subscribe for type: {0} ({1})",
							                                          typeof (TComponent).FullName, messageType.FullName));

						var interceptorParameter = Expression.Parameter(typeof (BatchInterceptor), "interceptor");
						var contextParameter = Expression.Parameter(typeof (IInterceptorContext), "context");
						var instanceParameter = Expression.Parameter(typeof (object), "instance");

						var instanceCast = Expression.Convert(instanceParameter, typeof (TComponent));

						var call = Expression.Call(interceptorParameter, genericMethod, contextParameter, instanceCast);
						var parameters = new[] {interceptorParameter, contextParameter, instanceParameter};
						var connector = Expression.Lambda<Func<BatchInterceptor, IInterceptorContext, object, Func<bool>>>(call, parameters).Compile();

						Func<BatchInterceptor, IInterceptorContext, object, Func<bool>> method = (interceptor, context, obj) =>
							{
								if (context.HasMessageTypeBeenDefined(messageType))
									return () => false;

								context.MessageTypeWasDefined(messageType);
								context.MessageTypeWasDefined(_batchType.MakeGenericType(messageType, keyType));

								return connector(interceptor, context, obj);
							};

						if (invoker == null)
							invoker = method;
						else
							invoker += method;
					}

					return invoker;
				});
		}

		private Func<BatchInterceptor, IInterceptorContext, Func<bool>> GetInvoker<TComponent>()
		{
			Type componentType = typeof (TComponent);

			return _components.Retrieve(componentType, () =>
				{
					Func<BatchInterceptor, IInterceptorContext, Func<bool>> invoker = null;

					// since we don't have it, we're going to build it

					foreach (Type interfaceType in componentType.GetInterfaces())
					{
						Type messageType;
						Type keyType;
						if (!GetMessageType(interfaceType, out messageType, out keyType))
							continue;

						var typeArguments = new[] {typeof (TComponent), messageType, keyType};
						MethodInfo genericMethod = FindMethod(GetType(), "Connect", typeArguments, new[] {typeof (IInterceptorContext)});
						if (genericMethod == null)
							throw new PipelineException(string.Format("Unable to subscribe for type: {0} ({1})",
							                                          typeof (TComponent).FullName, messageType.FullName));

						var interceptorParameter = Expression.Parameter(typeof (BatchInterceptor), "interceptor");
						var contextParameter = Expression.Parameter(typeof (IInterceptorContext), "context");

						var call = Expression.Call(interceptorParameter, genericMethod, contextParameter);
						var parameters = new[] {interceptorParameter, contextParameter};
						var connector = Expression.Lambda<Func<BatchInterceptor, IInterceptorContext, Func<bool>>>(call, parameters).Compile();

						Func<BatchInterceptor, IInterceptorContext, Func<bool>> method = (interceptor, context) =>
							{
								if (context.HasMessageTypeBeenDefined(messageType))
									return () => false;

								context.MessageTypeWasDefined(messageType);
								context.MessageTypeWasDefined(_batchType.MakeGenericType(messageType, keyType));

								return connector(interceptor, context);
							};

						if (invoker == null)
							invoker = method;
						else
							invoker += method;
					}

					return invoker;
				});
		}

		private static bool GetMessageType(Type interfaceType, out Type messageType, out Type keyType)
		{
			messageType = null;
			keyType = null;

			if (!interfaceType.IsGenericType) return false;

			Type genericType = interfaceType.GetGenericTypeDefinition();
			if (genericType != _interfaceType) return false;

			Type[] types = interfaceType.GetGenericArguments();

			messageType = types[0];
			if (!messageType.IsGenericType || messageType.GetGenericTypeDefinition() != _batchType) return false;

			Type[] genericArguments = messageType.GetGenericArguments();
			messageType = genericArguments[0];
			keyType = genericArguments[1];

			return true;
		}
	}
}