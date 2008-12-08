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
	using Configuration;
	using Exceptions;
	using Saga;
	using Sinks;

	public class InitiatesInterceptor :
		ConsumesInterceptorBase
	{
		protected override Type InterfaceType
		{
			get { return typeof (InitiatedBy<>); }
		}

		protected virtual Func<bool> Connect<TComponent, TMessage>(IInterceptorContext context)
			where TMessage : class, CorrelatedBy<Guid>
			where TComponent : class, Orchestrates<TMessage>, ISaga
		{
			MessageRouterConfigurator routerConfigurator = MessageRouterConfigurator.For(context.Pipeline);

			var sink = context.Builder.GetInstance<InitiateSagaMessageSink<TComponent, TMessage>>();
			if (sink == null)
				throw new ConfigurationException("Could not build the initiating message sink for " + typeof (TComponent).FullName);

			return routerConfigurator.FindOrCreate<TMessage>().Connect(sink);
		}

		public override IEnumerable<Func<bool>> Subscribe<TComponent>(IInterceptorContext context, TComponent instance)
		{
			yield break;
		}
	}
}