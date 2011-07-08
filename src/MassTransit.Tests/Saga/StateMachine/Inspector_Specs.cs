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
namespace MassTransit.Tests.Saga.StateMachine
{
	using System.Linq;
	using Locator;
	using Magnum.TestFramework;
	using NUnit.Framework;

	[TestFixture]
	public class Inspecting_a_state_machine_driven_saga : 
		StateMachineSubscriberTestBase
	{
		[Test]
		public void Should_return_all_of_the_events()
		{
			Results.Length.ShouldEqual(3);
		}

		[Test]
		public void Should_contain_the_initiate_event()
		{
			Results.Where(x => x.SagaEvent.Event == TestSaga.Initiate).Single();
		}

		[Test]
		public void Should_contain_the_complete_event()
		{
			Results.Where(x => x.SagaEvent.Event == TestSaga.Complete).Single();
		}

		[Test]
		public void Should_contain_the_observation_event()
		{
			Results.Where(x => x.SagaEvent.Event == TestSaga.Observation).Single();
		}

		[Test]
		public void Should_include_the_initial_state_for_the_initiate_event()
		{
			var initiate = Results.Where(x => x.SagaEvent.Event == TestSaga.Initiate).Single();

			initiate.States.Contains(TestSaga.Initial).ShouldBeTrue();
		}
	}
}