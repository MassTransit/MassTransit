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
namespace MassTransit.TestFramework.Fixtures
{
	using System;
	using BusConfigurators;
	using MassTransit.Transports;
	using NUnit.Framework;
	using Subscriptions.Coordinator;

	[TestFixture]
	public class LocalAndRemoteTestFixture<TTransportFactory> :
		EndpointTestFixture<TTransportFactory>
		where TTransportFactory : ITransportFactory, new()
	{
		[TestFixtureSetUp]
		public void LocalAndRemoteTestFixtureSetup()
		{
			LocalBus = SetupServiceBus(LocalUri, ConfigureLocalBus);
			RemoteBus = SetupServiceBus(RemoteUri, ConfigureRemoteBus);

			_localLoopback.SetTargetCoordinator(_remoteLoopback.Router);
			_remoteLoopback.SetTargetCoordinator(_localLoopback.Router);
		}

		[TestFixtureTearDown]
		public void LocalAndRemoteTestFixtureTeardown()
		{
			LocalBus = null;
			RemoteBus = null;
		}

		protected Uri LocalUri { get; set; }
		protected Uri RemoteUri { get; set; }

		protected IServiceBus LocalBus { get; private set; }
		protected IServiceBus RemoteBus { get; private set; }

		SubscriptionLoopback _localLoopback;
		SubscriptionLoopback _remoteLoopback;

		protected virtual void ConfigureLocalBus(ServiceBusConfigurator configurator)
		{
			configurator.AddSubscriptionObserver((bus, coordinator) =>
				{
					_localLoopback = new SubscriptionLoopback(bus, coordinator);
					return _localLoopback;
				});
		}

		protected virtual void ConfigureRemoteBus(ServiceBusConfigurator configurator)
		{
			configurator.AddSubscriptionObserver((bus, coordinator) =>
				{
					_remoteLoopback = new SubscriptionLoopback(bus, coordinator);
					return _remoteLoopback;
				});
		}
	}
}