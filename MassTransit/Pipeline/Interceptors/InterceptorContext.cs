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

	public class InterceptorContext :
		IInterceptorContext
	{
		private readonly ISubscriptionEvent _subscriptionEvent;
		private readonly HashSet<Type> _used = new HashSet<Type>();

		public InterceptorContext(MessagePipeline pipeline, IObjectBuilder builder, ISubscriptionEvent subscriptionEvent)
		{
			_subscriptionEvent = subscriptionEvent;

			Pipeline = pipeline;
			Builder = builder;
		}

		public IObjectBuilder Builder { get; private set; }

		public MessagePipeline Pipeline { get; private set; }

		public bool HasMessageTypeBeenDefined(Type messageType)
		{
			return _used.Contains(messageType);
		}

		public void MessageTypeWasDefined(Type messageType)
		{
			_used.Add(messageType);
		}

		public Func<bool> SubscribedTo(Type messageType)
		{
			return _subscriptionEvent.SubscribedTo(messageType);
		}

		public Func<bool> SubscribedTo(Type messageType, string correlationId)
		{
			return _subscriptionEvent.SubscribedTo(messageType, correlationId);
		}

		public void UnsubscribedFrom(Type messageType)
		{
			_subscriptionEvent.UnsubscribedFrom(messageType);
		}

		public void UnsubscribedFrom(Type messageType, string correlationId)
		{
			_subscriptionEvent.UnsubscribedFrom(messageType, correlationId);
		}
	}
}