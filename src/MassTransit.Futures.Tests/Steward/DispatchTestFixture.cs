// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests.Steward
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ConsumeConfigurators;
    using MassTransit.Steward.Contracts;
    using MassTransit.Steward.Core.Agents;
    using MassTransit.Steward.Core.Consumers;
    using NUnit.Framework;
    using Pipeline.ConsumerFactories;
    using TestFramework;


    [TestFixture]
    public abstract class InMemoryDispatchTestFixture :
        InMemoryTestFixture
    {
        protected InMemoryDispatchTestFixture()
        {
            CommandTestContexts = new Dictionary<Type, DispatchTestContext>();
        }

        protected IDictionary<Type, DispatchTestContext> CommandTestContexts { get; private set; }

        protected Task<ISendEndpoint> DispatchEndpoint
        {
            get { return Bus.GetSendEndpoint(GetCommandContext<DispatchMessage>().ExecuteUri); }
        }

        protected void AddCommandContext<TConsumer, T>(Action<IConsumerConfigurator<TConsumer>> configure = null)
            where T : class
            where TConsumer : class, IConsumer<T>, new()
        {
            var context = new DispatchTestContext<TConsumer, T>(BaseAddress, new DefaultConstructorConsumerFactory<TConsumer>(), configure);

            CommandTestContexts.Add(typeof(T), context);
        }

        protected void AddCommandContext<TConsumer, T>(IConsumerFactory<TConsumer> consumerFactory, Action<IConsumerConfigurator<TConsumer>> configure = null)
            where T : class
            where TConsumer : class, IConsumer<T>
        {
            var context = new DispatchTestContext<TConsumer, T>(BaseAddress, consumerFactory, configure);

            CommandTestContexts.Add(typeof(T), context);
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            AddCommandContext<DispatchMessageConsumer, DispatchMessage>(new DelegateConsumerFactory<DispatchMessageConsumer>(() =>
            {
                var agent = new MessageDispatchAgent();

                return new DispatchMessageConsumer(agent);
            }));

            SetupCommands();

            var factoryConfigurator = new BusFactoryConfigurator(configurator);

            foreach (DispatchTestContext dispatchTestContext in CommandTestContexts.Values)
                dispatchTestContext.Configure(factoryConfigurator);
        }


        class BusFactoryConfigurator :
            ActivityTestContextConfigurator
        {
            readonly IInMemoryBusFactoryConfigurator _configurator;

            public BusFactoryConfigurator(IInMemoryBusFactoryConfigurator configurator)
            {
                _configurator = configurator;
            }

            public void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configure)
            {
                _configurator.ReceiveEndpoint(queueName, configure);
            }
        }


        protected DispatchTestContext GetCommandContext<T>()
        {
            return CommandTestContexts[typeof(T)];
        }

        protected abstract void SetupCommands();
    }
}