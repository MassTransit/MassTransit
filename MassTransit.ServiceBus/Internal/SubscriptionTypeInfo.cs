/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.
namespace MassTransit.ServiceBus.Internal
{
	using System;
	using System.Collections.Generic;
	using Exceptions;
	using Subscriptions;

	public interface ICombinedTypeInfo :
		ISubscriptionTypeInfo,
		IPublicationTypeInfo,
		IProducerTypeInfo
	{
	}

	public class SubscriptionTypeInfo :
		ICombinedTypeInfo
	{
		private readonly List<ISubscriptionTypeInfo> _subscriptionTypes = new List<ISubscriptionTypeInfo>();
		private readonly List<IProducerTypeInfo> _producerTypes = new List<IProducerTypeInfo>();
		private IPublicationTypeInfo _publicationType;

		public IList<Subscription> GetConsumers<T>(T message) where T : class
		{
			return _publicationType.GetConsumers(message);
		}

		public void PublishFault<T>(IServiceBus bus, Exception ex, T message) where T : class
		{
			_publicationType.PublishFault(bus, ex, message);
		}

		public void Subscribe<T>(T component) where T : class
		{
			foreach (ISubscriptionTypeInfo subscription in _subscriptionTypes)
			{
				subscription.Subscribe(component);
			}
		}

		public void Unsubscribe<T>(T component) where T : class
		{
			foreach (ISubscriptionTypeInfo subscription in _subscriptionTypes)
			{
				subscription.Unsubscribe(component);
			}
		}

		public void AddComponent()
		{
			foreach (ISubscriptionTypeInfo subscription in _subscriptionTypes)
			{
				subscription.AddComponent();
			}
		}

		public void RemoveComponent()
		{
			foreach (ISubscriptionTypeInfo subscription in _subscriptionTypes)
			{
				subscription.AddComponent();
			}
		}

		public void Add(ISubscriptionTypeInfo subscriptionTypeInfo)
		{
			_subscriptionTypes.Add(subscriptionTypeInfo);
		}

		public void Add(IProducerTypeInfo producerTypeInfo)
		{
			_producerTypes.Add(producerTypeInfo);
			
		}

		public void SetPublicationType(IPublicationTypeInfo publicationTypeInfo)
		{
			if (_publicationType != null)
				throw new ConventionException("The publication type has already been set. Do you have a message with multiple correlation types?");

			_publicationType = publicationTypeInfo;
		}

		public void Dispose()
		{
			foreach (ISubscriptionTypeInfo subscriptionType in _subscriptionTypes)
			{
				subscriptionType.Dispose();
			}
			_subscriptionTypes.Clear();

			foreach (IProducerTypeInfo producerType in _producerTypes)
			{
				producerType.Dispose();
			}
			_producerTypes.Clear();
		}

		public void Attach<T>(T producer) where T : class
		{
			foreach (IProducerTypeInfo producerType in _producerTypes)
			{
				producerType.Attach(producer);
			}
		}
	}
}