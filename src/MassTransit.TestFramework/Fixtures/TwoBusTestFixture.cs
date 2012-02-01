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

	/// <summary>
	/// Test fixture that creates two buses, one "remote" and one "local". Of course, both are in-memory;
	/// but this test fixture makes sure that the two buses uses the same loopback router, while still having 
	/// a non-loopback transport factory.
	/// </summary>
	/// <typeparam name="TTransportFactory"></typeparam>
	[TestFixture]
	public class TwoBusTestFixture<TTransportFactory> :
		EndpointTestFixture<TTransportFactory>
		where TTransportFactory : class, ITransportFactory, new()
	{
		/// <summary>
		/// Sets up the remote and local bus and their target coordinators/subscription
		/// routers.
		/// </summary>
		[TestFixtureSetUp]
		public void TwoBusTestFixtureSetup()
		{
			LocalBus = SetupServiceBus(LocalUri, ConfigureLocalBus);
			RemoteBus = SetupServiceBus(RemoteUri, ConfigureRemoteBus);

			_localLoopback.SetTargetCoordinator(_remoteLoopback.Router);
			_remoteLoopback.SetTargetCoordinator(_localLoopback.Router);
		}

		/// <summary>
		/// Makes remote and local bus = null.
		/// </summary>
		[TestFixtureTearDown]
		public void TwoBusTestFixtureTeardown()
		{
			LocalBus = null;
			RemoteBus = null;
		}

		/// <summary>
		/// Gets or sets the local uri, i.e. the bus endpoint for the local bus. Set this property in the c'tor of your
		/// subclassing test fixture.
		/// </summary>
		protected Uri LocalUri { get; set; }

		/// <summary>
		/// Gets or sets the remote uri, i.e. the bus endpoint for the remote bus. Set this property in the c'tor of your
		/// subclassing test fixture.
		/// </summary>
		protected Uri RemoteUri { get; set; }

		/// <summary>
		/// Gets the local bus. Is null when c'tor runs.
		/// </summary>
		protected IServiceBus LocalBus { get; private set; }

		/// <summary>
		/// Gets the remote bus. Is null when c'tor runs.
		/// </summary>
		protected IServiceBus RemoteBus { get; private set; }

		SubscriptionLoopback _localLoopback;
		SubscriptionLoopback _remoteLoopback;

		/// <summary>
		/// You can override to configure the local bus; but if you don't call base method,
		/// you will not get the subscription loopback router.
		/// </summary>
		/// <param name="configurator"></param>
		protected virtual void ConfigureLocalBus(ServiceBusConfigurator configurator)
		{
			configurator.AddSubscriptionObserver((bus, coordinator) =>
				{
					_localLoopback = new SubscriptionLoopback(bus, coordinator);
					return _localLoopback;
				});
		}

		/// <summary>
		/// You can override to configure the remote bus; but if you don't call base method,
		/// you will not get the subscription loopback router.
		/// </summary>
		/// <param name="configurator"></param>
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