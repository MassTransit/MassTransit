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
namespace MassTransit.Testing
{
	using Factories;
	using Saga;
	using ScenarioBuilders;

	public static class TestFactory
	{
		public static HandlerTestFactory<BusTestScenario, TMessage> ForHandler<TMessage>()
			where TMessage : class
		{
			var factory = new HandlerTestFactoryImpl<BusTestScenario, TMessage>(() => new LoopbackBusScenarioBuilder());

			return factory;
		}

		public static ConsumerTestFactory<BusTestScenario, TConsumer> ForConsumer<TConsumer>()
			where TConsumer : class, IConsumer
		{
			var factory = new ConsumerTestFactoryImpl<BusTestScenario, TConsumer>(() => new LoopbackBusScenarioBuilder());

			return factory;
		}

		public static SagaTestFactory<BusTestScenario, TSaga> ForSaga<TSaga>()
			where TSaga : class, ISaga
		{
			var factory = new SagaTestFactoryImpl<BusTestScenario, TSaga>(() => new LoopbackBusScenarioBuilder());

			return factory;
		}
	}
}