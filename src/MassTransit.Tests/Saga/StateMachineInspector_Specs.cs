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
namespace MassTransit.Tests.Saga
{
	using System.Collections.Generic;
	using System.Linq;
	using BusConfigurators;
	using BusServiceConfigurators;
	using Locator;
	using Magnum.TestFramework;
	using MassTransit.Distributor;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class StateMachineInspector_Specs
	{
		[Test]
		public void FirstTestName()
		{
			var configurator = MockRepository.GenerateMock<ServiceBusConfigurator>();

			configurator.UseSagaDistributorFor<TestSaga>();

//			IList<object[]> calls =
//				configurator.GetArgumentsForCallsMadeOn(x => x.AddService(BusServiceLayer.Presentation, () => new Distributor<InitiateSimpleSaga>()));
//
//			calls.Count.ShouldEqual(3, "Not enough calls were made to configure the saga");
//
//			calls.Any(x => x[0].GetType().Equals(typeof (DefaultBusServiceConfigurator<Distributor<InitiateSimpleSaga>>)))
//				.ShouldBeTrue("The event was not registered");
//			calls.Any(x => x[0].GetType().Equals(typeof (DefaultBusServiceConfigurator<Distributor<CompleteSimpleSaga>>)))
//				.ShouldBeTrue("The event was not registered");
//			calls.Any(x => x[0].GetType().Equals(typeof (DefaultBusServiceConfigurator<Distributor<ObservableSagaMessage>>)))
//				.ShouldBeTrue("The event was not registered");
		}
	}
}