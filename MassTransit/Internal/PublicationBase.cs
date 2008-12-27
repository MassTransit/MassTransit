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
namespace MassTransit.Internal
{
	using System;
	using System.Collections.Generic;
	using Magnum.Common.ObjectExtensions;
	using Subscriptions;

	public abstract class PublicationBase<TMessage> :
		IPublicationTypeInfo
		where TMessage : class
	{
		protected Type _messageType;
		protected TimeSpan _timeToLive;

		protected PublicationBase()
		{
			_messageType = typeof (TMessage);
			_timeToLive = GetMessageTimeToLive();
		}

		public TimeSpan TimeToLive
		{
			get { return _timeToLive; }
		}

		public abstract IList<Subscription> GetConsumers<T>(IDispatcherContext context, T message) where T : class;

		public abstract void PublishFault<T>(IServiceBus bus, Exception ex, T message) where T : class;

		private TimeSpan GetMessageTimeToLive()
		{
			TimeSpan timeToLive = TimeSpan.MaxValue;

			ExpiresInAttribute attribute = _messageType.GetAttribute<ExpiresInAttribute>();
			if (attribute != null && attribute.TimeToLive < timeToLive)
				timeToLive = attribute.TimeToLive;

			return timeToLive;
		}
	}
}