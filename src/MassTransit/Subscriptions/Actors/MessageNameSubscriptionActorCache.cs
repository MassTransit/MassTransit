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
	using System.Collections.Generic;
	using Stact;
	using Util;

	public class MessageNameSubscriptionActorCache :
		Actor
	{
		readonly ActorFactory<SubscriptionActor> _typeActorFactory;
		readonly IDictionary<string, ActorInstance> _typeActors;

		public MessageNameSubscriptionActorCache(UntypedChannel publishChannel)
		{
			_typeActors = new Dictionary<string, ActorInstance>();
			_typeActorFactory = ActorFactory.Create((f, s, i) => new SubscriptionActor(f, s, i, publishChannel));
		}

		[UsedImplicitly]
		public void Handle(Message<SubscribeTo> message)
		{
			ActorInstance actor;
			if (!_typeActors.TryGetValue(message.Body.MessageName, out actor))
			{
				actor = _typeActorFactory.GetActor();
				actor.Send(new InitializeSubscriptionActor(message.Body.MessageName));
				_typeActors.Add(message.Body.MessageName, actor);
			}

			actor.Send(message);
		}

		[UsedImplicitly]
		public void Handle(Message<UnsubscribeFrom> message)
		{
			ActorInstance actor;
			if (_typeActors.TryGetValue(message.Body.MessageName, out actor))
			{
				actor.Send(message);
			}
		}
	}
}