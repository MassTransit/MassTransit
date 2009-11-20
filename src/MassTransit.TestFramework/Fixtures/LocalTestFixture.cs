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
namespace MassTransit.TestFramework.Fixtures
{
	using System;
	using Configuration;
	using NUnit.Framework;

	[TestFixture]
	public abstract class LocalTestFixture<TEndpoint> :
		EndpointTestFixture<TEndpoint>
		where TEndpoint : IEndpoint
	{
		[TestFixtureSetUp]
		public void LocalTestFixtureSetup()
		{
			LocalBus = SetupServiceBus(LocalUri);
		}

		[TestFixtureTearDown]
		public void LocalTestFixtureTeardown()
		{
			LocalBus.Dispose();
			LocalBus = null;
		}

		protected Uri LocalUri { get; set; }

		protected virtual IServiceBus SetupServiceBus(Uri uri)
		{
			IServiceBus bus = ServiceBusConfigurator.New(x =>
				{
					x.ReceiveFrom(uri);

					ConfigureLocalBus(x);
				});

			Buses.Add(bus);

			return bus;
		}

		protected void ConfigureLocalBus(IServiceBusConfigurator configurator)
		{
		}

		protected IServiceBus LocalBus { get; private set; }
	}
}