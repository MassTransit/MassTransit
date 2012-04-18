// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Distributor
{
    using System;
    using System.Collections.Generic;
    using Magnum.Extensions;
    using SubscriptionBuilders;
    using Subscriptions;

    public class DistributorBusService :
		IBusService
	{
		readonly IList<SubscriptionBuilder> _builders;
		readonly IList<ISubscriptionReference> _subscriptions;

		bool _disposed;

		public DistributorBusService(IList<SubscriptionBuilder> builders)
		{
			_builders = builders;

			_subscriptions = new List<ISubscriptionReference>();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Start(IServiceBus bus)
		{
			bus.Configure(pipelineConfigurator =>
				{
					foreach (SubscriptionBuilder builder in _builders)
					{
						try
						{
							ISubscriptionReference subscription = builder.Subscribe(pipelineConfigurator);
							_subscriptions.Add(subscription);
						}
						catch (Exception)
						{
							StopAllSubscriptions();
							throw;
						}
					}

					return () => true;
				});
		}

		public void Stop()
		{
			StopAllSubscriptions();
		}

		void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_builders.Clear();
				_subscriptions.Clear();
			}

			_disposed = true;
		}

		void StopAllSubscriptions()
		{
			_subscriptions.Each(x => x.OnStop());
			_subscriptions.Clear();
		}

		~DistributorBusService()
		{
			Dispose(false);
		}
	}
}