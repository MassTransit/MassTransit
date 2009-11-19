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
namespace MassTransit.Tests.Saga
{
	using Configuration;
	using Locator;
	using MassTransit.Distributor;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class StateMachineInspector_Specs
	{
		[Test]
		public void FirstTestName()
		{
			IServiceBusConfigurator configurator = MockRepository.GenerateMock<IServiceBusConfigurator>();
			configurator.Expect(x => x.AddService<Distributor<InitiateSimpleSaga>>(null)).IgnoreArguments();
			configurator.Expect(x => x.AddService<Distributor<CompleteSimpleSaga>>(null)).IgnoreArguments();
			configurator.Expect(x => x.AddService<Distributor<ObservableSagaMessage>>(null)).IgnoreArguments();

			IEndpointFactory endpointFactory = MockRepository.GenerateMock<IEndpointFactory>();
			;
			configurator.UseDistributorForSaga<TestSaga>(endpointFactory);

			configurator.VerifyAllExpectations();
		}
	}
}