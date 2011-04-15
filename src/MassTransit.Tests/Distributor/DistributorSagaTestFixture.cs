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
namespace MassTransit.Tests.Distributor
{
	using Configuration;
	using Load.Sagas;
	using MassTransit.Distributor;
	using MassTransit.Saga;
	using MassTransit.Transports;
	using TestFramework;
	using TextFixtures;

	public class DistributorSagaTestFixture<TTransportFactory> :
		SubscriptionServiceTestFixture<TTransportFactory>
		where TTransportFactory : ITransportFactory
	{
		protected ISagaRepository<FirstSaga> FirstSagaRepository { get; private set; }

		protected override void EstablishContext()
		{
			base.EstablishContext();

			FirstSagaRepository = ObjectBuilder.SetupSagaRepository<FirstSaga>();
		}

		protected override void ConfigureLocalBus(IServiceBusConfigurator configurator)
		{
			configurator.UseSagaDistributorFor<FirstSaga>(EndpointResolver);
		}
	}
}