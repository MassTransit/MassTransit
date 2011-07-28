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
	using Magnum;
	using Magnum.Extensions;
	using Messages;
	using Pipeline;
	using Services.Subscriptions.Messages;
	using Stact;
	using log4net;
	using AddSubscription = Services.Subscriptions.Messages.AddSubscription;
	using RemoveSubscription = Services.Subscriptions.Messages.RemoveSubscription;

	public class BusSubscriptionEventListener :
		ISubscriptionEvent,
		IDisposable
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (BusSubscriptionEventListener));
		ActorInstance _actorCache;

		bool _disposed;

		public BusSubscriptionEventListener(UntypedChannel output)
		{
			_actorCache = ActorFactory.Create<BusSubscriptionActorCache>(x =>
				{
					x.ConstructedBy(() => new BusSubscriptionActorCache(output));
					x.HandleOnCallingThread();
				}).GetActor();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public UnsubscribeAction SubscribedTo<TMessage>()
			where TMessage : class
		{
			return Subscribe<TMessage>(null);
		}

		public UnsubscribeAction SubscribedTo<TMessage, TKey>(TKey correlationId)
			where TMessage : class, CorrelatedBy<TKey>
		{
			return Subscribe<TMessage>(string.Format("{0}", correlationId));
		}

		UnsubscribeAction Subscribe<TMessage>(string correlationId)
			where TMessage : class
		{
			if (IgnoreMessageType<TMessage>())
				return () => true;

			Guid subscriptionId = CombGuid.Generate();
			string messageName = typeof (TMessage).ToMessageName();

			var subscribeTo = new SubscribeToMessage
				{
					SubscriptionId = subscriptionId,
					MessageName = messageName,
					CorrelationId = correlationId,
				};

			if (_log.IsDebugEnabled)
				_log.DebugFormat("SubscribeTo: {0}, {1}", subscribeTo.MessageName, subscribeTo.SubscriptionId);

			_actorCache.Send(subscribeTo);

			return () => Unsubscribe(subscriptionId, messageName, correlationId);
		}

		bool Unsubscribe(Guid subscriptionId, string messageName, string correlationId)
		{
			var unsubscribeFrom = new UnsubscribeFromMessage
				{
					SubscriptionId = subscriptionId,
					MessageName = messageName,
					CorrelationId = correlationId,
				};

			if (_log.IsDebugEnabled)
				_log.DebugFormat("UnsubscribeFrom: {0}, {1}", unsubscribeFrom.MessageName, unsubscribeFrom.SubscriptionId);

			_actorCache.Send(unsubscribeFrom);

			return true;
		}

		bool IgnoreMessageType<TMessage>()
		{
			if (typeof (TMessage) == typeof (AddSubscription))
				return true;
			if (typeof (TMessage) == typeof (RemoveSubscription))
				return true;
			if (typeof (TMessage) == typeof (AddSubscriptionClient))
				return true;
			if (typeof (TMessage) == typeof (RemoveSubscriptionClient))
				return true;
			if (typeof (TMessage) == typeof (SubscriptionRefresh))
				return true;

			return false;
		}

		void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_actorCache.ExitOnDispose(30.Seconds()).Dispose();
				_actorCache = null;
			}

			_disposed = true;
		}

		~BusSubscriptionEventListener()
		{
			Dispose(false);
		}
	}
}