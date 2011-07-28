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
namespace MassTransit.Subscriptions.Actors
{
	using System;
	using System.Collections.Generic;
	using Services.Subscriptions.Messages;
	using Stact;

	public class RemoteEndpointActor :
		Actor
	{
		Uri _controlUri;
		ActorFactory<SubscriptionActor> _factory;
		Fiber _fiber;
		Inbox _inbox;
		Scheduler _scheduler;
		IDictionary<Type, ActorInstance> _subscriptionTypes;

		public RemoteEndpointActor(Fiber fiber, Scheduler scheduler, Inbox inbox, UntypedChannel parent)
		{
			_fiber = fiber;
			_scheduler = scheduler;
			_inbox = inbox;
			_subscriptionTypes = new Dictionary<Type, ActorInstance>();

			_factory = ActorFactory.Create((f, s, i) => new SubscriptionActor(i, _inbox));

			inbox.Receive<Message<InitializeRemoteEndpointActor>>(message =>
				{
					_controlUri = message.Body.ControlUri;

					inbox.Loop(loop => { loop.Receive<Message<RemoveSubscriptionClient>>(m => { }); });
				});
		}
	}
}