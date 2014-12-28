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
	using TestInstanceConfigurators;

	/// <summary>
	/// Factory for testing message handlers, buses and messages - received, skipped, sent and published. The builders used
	/// with the <see cref="ITestConfigurator{TScenario}"/>, defaults to the loopback bus scenario. Use the extension methods in
	/// <see cref="BusTestScenarioExtensions"/> to use alternative scenario builders. A builder is something that ties some component
	/// together.
	/// </summary>
	public static class TestFactory
	{
		/// <summary>
		/// Creates a new <see cref="HandlerTestFactory{TScenario,TMessage}"/> for the passed message (generic parameter).
		/// </summary>
		/// <typeparam name="TMessage">The type of the message to create a test for.</typeparam>
		/// <returns>A 'configurator' - a handler test factory.</returns>
		public static HandlerTestFactory<IBusTestScenario, TMessage> ForHandler<TMessage>()
			where TMessage : class
		{
            var factory = new HandlerTestFactoryImpl<IBusTestScenario, TMessage>(() => new BusTestScenarioBuilder());

			return factory;
		}

        public static IConsumerTestFactory<IBusTestScenario, TConsumer> ForConsumer<TConsumer>()
			where TConsumer : class, IConsumer
		{
            var factory = new ConsumerTestFactoryImpl<IBusTestScenario, TConsumer>(() => new BusTestScenarioBuilder());

			return factory;
		}

		public static SagaTestFactory<IBusTestScenario, TSaga> ForSaga<TSaga>()
			where TSaga : class, ISaga
		{
            var factory = new SagaTestFactoryImpl<IBusTestScenario, TSaga>(() => new BusTestScenarioBuilder());

			return factory;
		}
	}
}