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
	using System.Collections;
	using System.Linq;
	using Locator;
	using Magnum.Reflection;
	using MassTransit.Pipeline;
	using MassTransit.Saga;
	using MassTransit.Saga.Configuration;
	using MassTransit.Saga.Pipeline;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class Subscribing_a_saga_state_machine :
		StateMachineSubscriberTestBase
	{
		[Test, Explicit]
		public void Should_handle_data_events_on_the_saga_state_machine()
		{
			EventInspectorResult<TestSaga> initiate = Results.Where(x => x.SagaEvent.Event == TestSaga.Initiate).Single();

			var factory = MockRepository.GenerateMock<ISagaPolicyFactory>();

			var context = MockRepository.GenerateMock<ISubscriberContext>();
			//context.Stub(x => x.Pipeline).Return(new MessagePipeline())

			var builder = MockRepository.GenerateMock<IObjectBuilder>();
			context.Stub(x => x.Builder).Return(builder);

			builder.Stub(x => x.GetInstance<CorrelatedSagaStateMachineMessageSink<TestSaga, InitiateSimpleSaga>>(new Hashtable()))
				.Return(MockRepository.GenerateMock<CorrelatedSagaStateMachineMessageSink<TestSaga, InitiateSimpleSaga>>());

			var policy = MockRepository.GenerateMock<ISagaPolicy<TestSaga, InitiateSimpleSaga>>();
			factory.Stub(x => x.GetPolicy<TestSaga, InitiateSimpleSaga>(initiate.States)).Return(policy);

			var subscriber = new SagaEventSubscriber<TestSaga>(context, factory);

			subscriber.Call("Connect", new[] {initiate.SagaEvent.MessageType}, initiate.SagaEvent.Event, initiate.States);
		}
	}
}