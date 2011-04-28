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
namespace MassTransit.Subscriptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
	/// Manages the subscription and unsubscription of message consumers to the
	/// service bus as part of the bus lifecycle.
	/// 
	/// As a bus service, once the bus is started and operational, this service 
	/// will get started. At this point, any registered consumers and sagas will
	/// be subscribed to the bus. 
	/// 
	/// When stop is called, those subscriptions will be removed unless the
	/// registration information indicated that the subscription is meant to be
	/// persistent, and not removed on service shutdown.
	/// 
	/// TODO: DRU/SUB
	/// </summary>
	public class SubscriptionBusService :
		IBusService
	{
	    IList<SubscriptionRegistration> _unsubscribes;

		public void Dispose()
		{
		}

		public void Start(IServiceBus bus)
		{
		    foreach (var unsubscribe in _unsubscribes)
		    {
		        var ua = bus.Subscribe(unsubscribe.ConsumingType);
		        unsubscribe.Unsubscribe = ua;
		    }

		    bus.SubscribeConsumer<object>(o =>
		    {
		        if (o == null)
		            return null;

		        var sr = new SubscriptionRegistration();

                return msg =>
                {
                    sr.ConsumerFactory(sr.ConsumingType);
                };
		    });
		}

		public void Stop()
		{
		    _unsubscribes
                .Where(u=>u.AutoUnsubscribe)
                .Each(u => u.Unsubscribe());
		}
	}

    public class SubscriptionRegistration
    {
        public Type ConsumingType { get; private set; }
        public Func<Type, object> ConsumerFactory { get; private set; }
        public bool AutoUnsubscribe { get; private set; }
        public UnsubscribeAction Unsubscribe { get; set; }
    }
}