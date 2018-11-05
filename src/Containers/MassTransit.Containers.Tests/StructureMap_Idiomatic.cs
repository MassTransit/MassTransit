// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Containers.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Scenarios;
    using Shouldly;
    using StructureMap;
    using StructureMap.Pipeline;
    using TestFramework;
    using Testing;


    [TestFixture]
    public class StructureMap_Idiomatic :
        AsyncTestFixture
    {
        [Test]
        public async Task Should_work_with_the_registry()
        {
            var bus = _container.GetInstance<IBusControl>();

            BusHandle busHandle = await bus.StartAsync();

            await busHandle.Ready;

            ISendEndpoint endpoint = await bus.GetSendEndpoint(new Uri("loopback://localhost/input_queue"));

            const string name = "Joe";

            await endpoint.Send(new SimpleMessageClass(name));

            SimpleConsumer lastConsumer = await SimpleConsumer.LastConsumer;
            lastConsumer.ShouldNotBe(null);

            SimpleMessageInterface last = await lastConsumer.Last;
            last.Name
                .ShouldBe(name);

            bool wasDisposed = await lastConsumer.Dependency.WasDisposed;
            wasDisposed
                .ShouldBe(true); //Dependency was not disposed");

            lastConsumer.Dependency.SomethingDone
                .ShouldBe(true); //Dependency was disposed before consumer executed");
        }

        public StructureMap_Idiomatic()
            : base(new InMemoryTestHarness())
        {
        }

        Container _container;

        [OneTimeSetUp]
        public void Setup()
        {
            _container = new Container(x =>
            {
                x.AddRegistry(new BusRegistry());
                x.AddRegistry(new ConsumerRegistry());
            });
        }


        class ConsumerRegistry :
            Registry
        {
            public ConsumerRegistry()
            {
                ForConcreteType<SimpleConsumer>();
                For<ISimpleConsumerDependency>()
                    .Use<SimpleConsumerDependency>();
                For<AnotherMessageConsumer>()
                    .Use<AnotherMessageConsumerImpl>();
            }
        }


        class BusRegistry :
            Registry
        {
            public BusRegistry()
            {
                For<IBusControl>(new SingletonLifecycle())
                    .Use(context => Bus.Factory.CreateUsingInMemory(x => x.ReceiveEndpoint("input_queue", e => e.Consumer<SimpleConsumer>(context))));
            }
        }


        [OneTimeTearDown]
        public void Teardown()
        {
            _container.Dispose();
        }
    }
}