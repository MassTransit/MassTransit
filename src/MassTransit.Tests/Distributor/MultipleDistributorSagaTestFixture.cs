// Copyright 2007-2011 The Apache Software Foundation.
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
namespace MassTransit.Tests.Distributor
{
	using BusConfigurators;
	using Load.Sagas;
	using MassTransit.Saga;
	using MassTransit.Transports;
	using NUnit.Framework;
	using TextFixtures;

    [TestFixture, Ignore]
	public class MultipleDistributorSagaTestFixture<TTransportFactory> :
		SubscriptionServiceTestFixture<TTransportFactory>
		where TTransportFactory : class, ITransportFactory, new()
	{
		protected ISagaRepository<FirstSaga> FirstSagaRepository { get; private set; }

		protected override void EstablishContext()
		{
			base.EstablishContext();

			FirstSagaRepository = SetupSagaRepository<FirstSaga>();
		}

		protected override void ConfigureLocalBus(ServiceBusConfigurator configurator)
		{
		    configurator.Distributor(d => d.Saga(FirstSagaRepository));
		}

		protected override void ConfigureRemoteBus(ServiceBusConfigurator configurator)
		{
            configurator.Distributor(d => d.Saga(FirstSagaRepository));
        }
	}
}