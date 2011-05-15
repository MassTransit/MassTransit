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
namespace MassTransit.Context
{
	using System;
	using System.Collections.Generic;

	public class PublishContext<T> :
		SendContext<T>,
		IBusPublishContext<T>
		where T : class
	{
		readonly HashSet<IEndpoint> _endpoints = new HashSet<IEndpoint>();
		Action<T, IEndpoint> _eachSubscriberAction = Ignore;
		Action<T> _noSubscribersAction = Ignore;

		public PublishContext(T message)
			: base(message)
		{
			_noSubscribersAction = Ignore;
			_eachSubscriberAction = Ignore;
		}

		public PublishContext(T message, ISendContext context)
			: base(message, context)
		{
			_noSubscribersAction = Ignore;
			_eachSubscriberAction = Ignore;
		}

		public override bool TryGetContext<TMessage>(out IBusPublishContext<TMessage> context)
		{
			context = null;

			if (typeof(TMessage).IsAssignableFrom(typeof(T)))
			{
				context = new PublishContext<TMessage>(Message as TMessage, this);
				context.IfNoSubscribers(x => NotifyNoSubscribers(Message));
				context.ForEachSubscriber((x,e) => NotifyForMessageConsumer(Message, e));
			}

			return context != null;
		}

		public void NotifyForMessageConsumer(T message, IEndpoint endpoint)
		{
			_endpoints.Add(endpoint);

			_eachSubscriberAction(message, endpoint);
		}

		public bool WasEndpointAlreadySent(IEndpoint endpoint)
		{
			return _endpoints.Contains(endpoint);
		}

		public void NotifyNoSubscribers(T message)
		{
			_noSubscribersAction(message);
		}

		public void IfNoSubscribers(Action<T> action)
		{
			_noSubscribersAction = action;
		}

		public void ForEachSubscriber(Action<T, IEndpoint> action)
		{
			_eachSubscriberAction = action;
		}

		static void Ignore(T message)
		{
		}

		static void Ignore(T message, IEndpoint endpoint)
		{
		}
	}
}