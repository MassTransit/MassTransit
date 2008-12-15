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
	using System.Collections;
	using System.Collections.Generic;
	using Configuration;
	using Exceptions;
	using Saga;
	using Sinks;

	public class OrchestratesInterceptor :
		ConsumesInterceptorBase<OrchestratesInterceptor>
	{
		protected override Type InterfaceType
		{
			get { return typeof (Orchestrates<>); }
		}

		protected virtual Func<bool> Connect<TComponent, TMessage>(IInterceptorContext context)
			where TMessage : class, CorrelatedBy<Guid>
			where TComponent : class, Orchestrates<TMessage>, ISaga
		{
			MessageRouterConfigurator routerConfigurator = MessageRouterConfigurator.For(context.Pipeline);

			var router = routerConfigurator.FindOrCreate<TMessage>();

			var sink = context.Builder.GetInstance<OrchestrateSagaMessageSink<TComponent, TMessage>>(new Hashtable {{"context", context}});
			if (sink == null)
				throw new ConfigurationException("Could not build the message sink for " + typeof (TComponent).FullName);

			var result = router.Connect(sink);

			Func<bool> remove = context.SubscribedTo(typeof(TMessage));

			return () => result() && (router.SinkCount == 0) && remove();
		}

		public override IEnumerable<Func<bool>> Subscribe<TComponent>(IInterceptorContext context, TComponent instance)
		{
			yield break;
		}
	}
}