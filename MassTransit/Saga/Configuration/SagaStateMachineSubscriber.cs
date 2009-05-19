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
namespace MassTransit.Saga.Configuration
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Reflection;
	using Exceptions;
	using Magnum;
	using MassTransit.Pipeline;
	using MassTransit.Pipeline.Configuration.Subscribers;
	using Util;

	public class SagaStateMachineSubscriber :
		PipelineSubscriberBase
	{
		public override IEnumerable<UnsubscribeAction> Subscribe<TComponent>(ISubscriberContext context)
		{
			Type componentType = typeof (TComponent);

			Type baseType = componentType.BaseType;
			if (!baseType.IsGenericType)
				yield break;

			if (baseType.GetGenericTypeDefinition() != typeof (SagaStateMachine<>))
				yield break;

			MethodInfo genericMethod = FindConnectMethod<TComponent>();

			var connector = BuildConnector(genericMethod);

			foreach (var result in connector(this, context))
			{
				yield return result;
			}
		}

		public override IEnumerable<UnsubscribeAction> Subscribe<TComponent>(ISubscriberContext context, TComponent instance)
		{
			yield break;
		}

		protected virtual IEnumerable<UnsubscribeAction> Connect<TComponent>(ISubscriberContext context)
			where TComponent : SagaStateMachine<TComponent>, ISaga
		{
			var component = (TComponent) Activator.CreateInstance(typeof (TComponent), CombGuid.Generate());

			var subscriber = new SagaStateMachineSubscriptionInspector(context);

			component.Inspect(subscriber);

			return subscriber.Results;
		}

		private MethodInfo FindConnectMethod<TComponent>()
		{
			var genericMethod = ReflectiveMethodInvoker.FindMethod(GetType(),
				"Connect",
				new[] { typeof(TComponent) },
				new[] { typeof(ISubscriberContext) });

			if (genericMethod == null)
				throw new ConfigurationException(string.Format("Unable to subscribe for type: '{0}'",
					typeof(TComponent).FullName));

			return genericMethod;
		}

		private Func<SagaStateMachineSubscriber, ISubscriberContext, IEnumerable<UnsubscribeAction>> BuildConnector(MethodInfo genericMethod)
		{
			var interceptorParameter = Expression.Parameter(typeof(SagaStateMachineSubscriber), "interceptor");
			var contextParameter = Expression.Parameter(typeof(ISubscriberContext), "context");

			var call = Expression.Call(interceptorParameter, genericMethod, contextParameter);
			var connector = Expression.Lambda<Func<SagaStateMachineSubscriber, ISubscriberContext, IEnumerable<UnsubscribeAction>>>(call, new[] { interceptorParameter, contextParameter }).Compile();

			return connector;
		}
	}
}