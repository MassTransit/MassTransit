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
	using ScenarioBuilders;
	using Scenarios;

	public static class ScenarioExtensions
	{
		public static HandlerTestFactoryImpl<BusTestScenario, TMessage> InSingleBusScenario<TScenario, TMessage>(
			this HandlerTestFactory<TScenario, TMessage> factory)
			where TScenario : TestScenario
			where TMessage : class
		{
			return new HandlerTestFactoryImpl<BusTestScenario, TMessage>(LoopbackBus);
		}

		public static HandlerTestFactoryImpl<LocalRemoteTestScenario, TMessage> InLocalRemoteBusScenario<TScenario, TMessage>(
			this HandlerTestFactory<TScenario, TMessage> factory)
			where TScenario : TestScenario
			where TMessage : class
		{
			return new HandlerTestFactoryImpl<LocalRemoteTestScenario, TMessage>(LoopbackLocalRemote);
		}

		public static ConsumerTestFactoryImpl<BusTestScenario, TMessage> InSingleBusScenario<TScenario, TMessage>(
			this ConsumerTestFactory<TScenario, TMessage> factory)
			where TScenario : TestScenario
			where TMessage : class
		{
			return new ConsumerTestFactoryImpl<BusTestScenario, TMessage>(LoopbackBus);
		}

		public static ConsumerTestFactoryImpl<LocalRemoteTestScenario, TMessage> InLocalRemoteBusScenario<TScenario, TMessage>
			(
			this ConsumerTestFactory<TScenario, TMessage> factory)
			where TScenario : TestScenario
			where TMessage : class
		{
			return new ConsumerTestFactoryImpl<LocalRemoteTestScenario, TMessage>(LoopbackLocalRemote);
		}

		static LoopbackBusScenarioBuilder LoopbackBus()
		{
			return new LoopbackBusScenarioBuilder();
		}

		static LoopbackLocalRemoteBusScenarioBuilder LoopbackLocalRemote()
		{
			return new LoopbackLocalRemoteBusScenarioBuilder();
		}
	}
}