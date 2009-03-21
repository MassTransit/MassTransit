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
	using Configuration;
	using Sinks;

	public class ConsumesSelectedInterceptor :
		ConsumesInterceptorBase<ConsumesSelectedInterceptor>
	{
		protected override Type InterfaceType
		{
			get { return typeof (Consumes<>.Selected); }
		}

		protected virtual UnsubscribeAction Connect<TMessage>(IInterceptorContext context, Consumes<TMessage>.Selected consumer) where TMessage : class
		{
			MessageRouterConfigurator routerConfigurator = MessageRouterConfigurator.For(context.Pipeline);

			var router = routerConfigurator.FindOrCreate<TMessage>();

			UnsubscribeAction result = router.Connect(new InstanceMessageSink<TMessage>(message =>
				{
					// rock it
					return consumer.Accept(message) ? (Action<TMessage>) consumer.Consume : null;
				}));

			UnsubscribeAction remove = context.SubscribedTo<TMessage>();

			return () => result() && (router.SinkCount == 0) && remove();
		}

		protected virtual UnsubscribeAction Connect<TComponent, TMessage>(IInterceptorContext context)
			where TMessage : class
			where TComponent : class, Consumes<TMessage>.Selected
		{
			MessageRouterConfigurator routerConfigurator = MessageRouterConfigurator.For(context.Pipeline);

			var router = routerConfigurator.FindOrCreate<TMessage>();

			UnsubscribeAction result = router.Connect(new SelectedComponentMessageSink<TComponent, TMessage>(context));

			UnsubscribeAction remove = context.SubscribedTo<TMessage>();

			return () => result() && (router.SinkCount == 0) && remove();
		}
	}
}