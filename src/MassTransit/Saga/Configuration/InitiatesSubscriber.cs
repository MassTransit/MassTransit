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
	using System.Collections;
	using System.Collections.Generic;
	using Exceptions;
	using MassTransit.Pipeline;
	using MassTransit.Pipeline.Configuration;
	using MassTransit.Pipeline.Configuration.Subscribers;
	using Pipeline;

	public class InitiatesSubscriber :
		ConsumesSubscriberBase<InitiatesSubscriber>
	{
		protected override Type InterfaceType
		{
			get { return typeof (InitiatedBy<>); }
		}

		protected virtual UnsubscribeAction Connect<TComponent, TMessage>(ISubscriberContext context)
			where TMessage : class, CorrelatedBy<Guid>
			where TComponent : class, InitiatedBy<TMessage>, ISaga
		{
			MessageRouterConfigurator routerConfigurator = MessageRouterConfigurator.For(context.Pipeline);

			var router = routerConfigurator.FindOrCreate<TMessage>();

			var repository = context.Builder.GetInstance<ISagaRepository<TComponent>>();
			var policy = new InitiatingSagaPolicy<TComponent, TMessage>(x => false);

			var sink = new CorrelatedSagaMessageSink<TComponent, TMessage>(context, context.Data as IServiceBus, repository, policy);
			if (sink == null)
				throw new ConfigurationException("Could not build the initiating message sink for " + typeof (TComponent).FullName);

			var result = router.Connect(sink);

			UnsubscribeAction remove = context.SubscribedTo<TMessage>();

			return () => result() && (router.SinkCount == 0) && remove();
		}

		public override IEnumerable<UnsubscribeAction> Subscribe<TComponent>(ISubscriberContext context, TComponent instance)
		{
			yield break;
		}
	}
}