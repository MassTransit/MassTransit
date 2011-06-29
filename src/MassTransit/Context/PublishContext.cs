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
	using System.Diagnostics;


	public class PublishContext<T> :
		SendContext<T>,
		IBusPublishContext<T>
		where T : class
	{
		readonly HashSet<IEndpointAddress> _endpoints = new HashSet<IEndpointAddress>();
		Action<IEndpointAddress> _eachSubscriberAction = Ignore;
		Action _noSubscribersAction = Ignore;
		Func<IEndpointAddress, bool> _wasEndpointAlreadySent;
		Stopwatch _timer;

		PublishContext(T message)
			: base(message)
		{
			_wasEndpointAlreadySent = _endpoints.Contains;
			_timer = Stopwatch.StartNew();
		}

		PublishContext(T message, ISendContext context)
			: base(message, context)
		{
			_wasEndpointAlreadySent = _endpoints.Contains;
			_timer = Stopwatch.StartNew();
		}

		public TimeSpan Duration
		{
			get { return _timer.Elapsed; }
		}

		public static PublishContext<T> FromMessage(T message)
		{
			return new PublishContext<T>(message);
		}

		public static PublishContext<T> FromMessage<TMessage>(TMessage message, ISendContext context)
			where TMessage : class
		{
			if (typeof (TMessage).IsAssignableFrom(typeof (T)))
			{
				return new PublishContext<T>(message as T, context);
			}

			return null;
		}

		public override bool TryGetContext<TMessage>(out IBusPublishContext<TMessage> context)
		{
			context = null;

			if (typeof(TMessage).IsAssignableFrom(typeof(T)))
			{
				var busPublishContext = new PublishContext<TMessage>(Message as TMessage, this);
				busPublishContext._wasEndpointAlreadySent = _wasEndpointAlreadySent;
				busPublishContext.IfNoSubscribers(_noSubscribersAction);
				busPublishContext.ForEachSubscriber(_eachSubscriberAction);

				context = busPublishContext;
			}

			return context != null;
		}

		public override void NotifySend(IEndpointAddress address)
		{
			base.NotifySend(address);

			_endpoints.Add(address);

			_eachSubscriberAction(address);
		}

		public bool WasEndpointAlreadySent(IEndpointAddress address)
		{
			return _wasEndpointAlreadySent(address);
		}

		public void NotifyNoSubscribers()
		{
			_noSubscribersAction();
		}

		public void IfNoSubscribers(Action action)
		{
			_noSubscribersAction = action;
		}

		public void ForEachSubscriber(Action<IEndpointAddress> action)
		{
			_eachSubscriberAction = action;
		}

		static void Ignore()
		{
		}

		static void Ignore(IEndpointAddress endpoint)
		{
		}

		public void Complete()
		{
			_timer.Stop();
		}
	}
}