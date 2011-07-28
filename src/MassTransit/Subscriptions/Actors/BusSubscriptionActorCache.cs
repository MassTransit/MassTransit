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
	using Magnum.Extensions;
	using Messages;
	using Stact;
	using Util;
	using log4net;

	public class BusSubscriptionActorCache :
		Actor
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (BusSubscriptionActorCache));

		readonly ActorFactory<BusSubscriptionActor> _typeActorFactory;
		readonly IDictionary<string, ActorInstance> _typeActors;

		public BusSubscriptionActorCache(UntypedChannel publishChannel)
		{
			_typeActors = new Dictionary<string, ActorInstance>();
			_typeActorFactory = ActorFactory.Create<BusSubscriptionActor>(x =>
				{
					x.ConstructedBy(i => new BusSubscriptionActor(i, publishChannel));
					x.HandleOnCallingThread();
				});
		}

		[UsedImplicitly]
		public void Handle(Message<Stop> message)
		{
			_typeActors.Values.Each(x => x.ExitOnDispose(30.Seconds()).Dispose());
		}

		[UsedImplicitly]
		public void Handle(Message<SubscribeToMessage> message)
		{
			ActorInstance actor;
			if (!_typeActors.TryGetValue(message.Body.MessageName, out actor))
			{
				actor = _typeActorFactory.GetActor();
				actor.Send(new InitializeBusSubscriptionActor(message.Body.MessageName));
				_typeActors.Add(message.Body.MessageName, actor);
			}

			if (_log.IsDebugEnabled)
				_log.DebugFormat("SubscribeTo: {0}, {1}", message.Body.MessageName, message.Body.SubscriptionId);

			actor.Send(message);
		}

		[UsedImplicitly]
		public void Handle(Message<UnsubscribeFromMessage> message)
		{
			ActorInstance actor;
			if (_typeActors.TryGetValue(message.Body.MessageName, out actor))
			{
				if (_log.IsDebugEnabled)
					_log.DebugFormat("UnsubscribeFrom: {0}, {1}", message.Body.MessageName, message.Body.SubscriptionId);

				actor.Send(message);
			}
			else
			{
				if (_log.IsDebugEnabled)
					_log.DebugFormat("UnsubscribeFrom(unknown): {0}, {1}", message.Body.MessageName, message.Body.SubscriptionId);
			}
		}
	}
}