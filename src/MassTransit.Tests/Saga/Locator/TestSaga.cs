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
namespace MassTransit.Tests.Saga.Locator
{
	using System;
	using Magnum.StateMachine;
	using MassTransit.Saga;

	public class TestSaga :
		SagaStateMachine<TestSaga>,
		ISaga
	{
		static TestSaga()
		{
			Define(() =>
				{
					Correlate(Observation).By((saga, message) => saga.Name == message.Name);

					Initially(
						When(Initiate)
							.Then((saga, message) =>
								{
									saga.WasInitiated = true;
									saga.Name = message.Name;
								})
							.TransitionTo(Initiated));

					During(Initiated,
						When(Observation)
							.Then((saga, message) => { saga.WasObserved = true; }),
						When(Complete)
							.Then((saga, message) => { saga.WasCompleted = true; })
							.TransitionTo(Completed));
				});
		}

		protected TestSaga()
		{
		}

		public TestSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public static State Initial { get; set; }
		public static State Completed { get; set; }
		public static State Initiated { get; set; }

		public static Event<InitiateSimpleSaga> Initiate { get; set; }
		public static Event<ObservableSagaMessage> Observation { get; set; }
		public static Event<CompleteSimpleSaga> Complete { get; set; }

		public bool WasInitiated { get; set; }
		public bool WasObserved { get; set; }
		public bool WasCompleted { get; set; }

		public string Name { get; set; }
		public Guid CorrelationId { get; private set; }

		public IServiceBus Bus { get; set; }
	}
}