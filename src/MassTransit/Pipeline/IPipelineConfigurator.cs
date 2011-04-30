// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
	using Configuration.Subscribers;

	public interface IPipelineConfigurator :
		ISubscriptionEvent
	{
		IMessagePipeline Pipeline { get; }

		IServiceBus Bus { get; }

		UnsubscribeAction Subscribe<TComponent>()
			where TComponent : class;

		UnsubscribeAction Subscribe<TMessage>(Action<TMessage> handler, Predicate<TMessage> acceptor)
			where TMessage : class;

		UnsubscribeAction Subscribe<TMessage>(Func<TMessage, Action<TMessage>> getHandler)
			where TMessage : class;

		UnsubscribeAction Subscribe<TComponent>(TComponent instance)
			where TComponent : class;

		UnregisterAction Register(IPipelineSubscriber subscriber);
		UnregisterAction Register(ISubscriptionEvent subscriptionEventHandler);
	}
}