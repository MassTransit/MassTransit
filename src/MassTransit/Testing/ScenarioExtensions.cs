// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Scenarios;


    public static class ScenarioExtensions
    {
        public static HandlerTestFactoryImpl<IBusTestScenario, TMessage> InSingleBusScenario<TScenario, TMessage>(
            this HandlerTestFactory<TScenario, TMessage> factory)
            where TScenario : ITestScenario
            where TMessage : class
        {
            return new HandlerTestFactoryImpl<IBusTestScenario, TMessage>(LoopbackBus);
        }

        public static ConsumerTestFactoryImpl<IBusEndpointTestScenario, TConsumer> InSingleBusScenario<TScenario, TConsumer>(
            this ConsumerTestFactory<TScenario, TConsumer> factory)
            where TScenario : IBusEndpointTestScenario
            where TConsumer : class, IConsumer
        {
            return new ConsumerTestFactoryImpl<IBusEndpointTestScenario, TConsumer>(LoopbackBus);
        }

        public static SagaTestFactoryImpl<IBusTestScenario, TSaga> InSingleBusScenario<TScenario, TSaga>(
            this SagaTestFactory<TScenario, TSaga> factory)
            where TScenario : ITestScenario
            where TSaga : class, ISaga
        {
            return new SagaTestFactoryImpl<IBusTestScenario, TSaga>(LoopbackBus);
        }

        static BusEndpointTestScenarioBuilder LoopbackBus()
        {
            return new BusEndpointTestScenarioBuilder();
        }
    }
}