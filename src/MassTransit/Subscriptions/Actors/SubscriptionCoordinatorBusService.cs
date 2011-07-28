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
	using Magnum;
	using Magnum.Extensions;
	using Stact;
	using log4net;

	public class SubscriptionCoordinatorBusService :
		IBusService
	{
		IServiceBus _bus;
		ActorInstance _producer;
		SubscriptionEventChannelPublisher _publisher;
		UnsubscribeAction _unregisterAction;
		ActorInstance _cache;
		static readonly ILog _log = LogManager.GetLogger(typeof (SubscriptionCoordinatorBusService));
		
		public void Dispose()
		{
			_log.Debug("Exiting Cache");

			_cache.Stop();
			_cache.ExitOnDispose(30.Seconds()).Dispose();
			_cache = null;

			_producer.ExitOnDispose(30.Seconds()).Dispose();
			_producer = null;
		}

		public void Start(IServiceBus bus)
		{
			var stub = new ChannelAdapter();

			ActorFactory<EndpointSubscriptionMessageProducer> factory = ActorFactory.Create(
				() => new EndpointSubscriptionMessageProducer(CombGuid.Generate(), bus.Endpoint.Address.Uri, stub));

			_producer = factory.GetActor();

			var connector = ActorFactory.Create(() => new OutboundPipelineSubscriptionConnector(bus)).GetActor();

			var broadcast = new BroadcastChannel(new UntypedChannel[]{connector, _producer});

			var cacheFactory = ActorFactory.Create(() => new MessageNameSubscriptionActorCache(broadcast));

			_cache = cacheFactory.GetActor();

			_publisher = new SubscriptionEventChannelPublisher(_cache);

			_bus = bus;
			_unregisterAction = _bus.Configure(x =>
				{
					UnregisterAction unregisterAction = x.Register(_publisher);

					return () => unregisterAction();
				});
		}

		public void Stop()
		{
			_unregisterAction();
		}
	}
}