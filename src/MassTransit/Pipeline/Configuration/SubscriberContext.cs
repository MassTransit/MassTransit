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
namespace MassTransit.Pipeline.Configuration
{
	using System;
	using System.Collections.Generic;
	using Internal;

	public class SubscriberContext :
		ISubscriberContext
	{
		private readonly ISubscriptionEvent _subscriptionEvent;
		private readonly HashSet<Type> _usedMessageTypes = new HashSet<Type>();

		public SubscriberContext(IMessagePipeline pipeline, IObjectBuilder builder, ISubscriptionEvent subscriptionEvent, object data)
		{
			_subscriptionEvent = subscriptionEvent;

			Pipeline = pipeline;
			Builder = builder;
			Data = data;
		}

		public IObjectBuilder Builder { get; private set; }

		public IMessagePipeline Pipeline { get; private set; }

		public object Data { get; private set; }

		public bool HasMessageTypeBeenDefined(Type messageType)
		{
			return _usedMessageTypes.Contains(messageType);
		}

		public void MessageTypeWasDefined(Type messageType)
		{
			_usedMessageTypes.Add(messageType);
		}

		public UnsubscribeAction SubscribedTo<T>() 
			where T : class
		{
			return _subscriptionEvent.SubscribedTo<T>();
		}

		public UnsubscribeAction SubscribedTo<T,K>(K correlationId)
			where T : class, CorrelatedBy<K>
		{
			return _subscriptionEvent.SubscribedTo<T,K>(correlationId);
		}
	}
}