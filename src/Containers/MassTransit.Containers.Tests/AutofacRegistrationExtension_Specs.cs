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
namespace MassTransit.Containers.Tests
{
    using System.Reflection;
    using System.Threading.Tasks;
    using Autofac;
    using NUnit.Framework;
    using Saga;
    using Scenarios;
    using TestFramework.Messages;


    [TestFixture]
    public class AutofacContainer_RegistrationExtension
    {
        [Test]
        public void Registration_extension_method_for_consumers()
        {
            var builder = new ContainerBuilder();

            builder.RegisterConsumers(typeof(AutofacContainer_RegistrationExtension).GetTypeInfo().Assembly);

            var container = builder.Build();

            Assert.That(container.IsRegistered<TestConsumer>(), Is.True);
        }

        [Test]
        public async Task Throw_them_under_the_bus()
        {
            var builder = new ContainerBuilder();

            builder.RegisterConsumers(typeof(AutofacContainer_RegistrationExtension).GetTypeInfo().Assembly);

            builder.RegisterInMemorySagaRepository();
            builder.RegisterType<InMemorySagaRepository<SimpleSaga>>()
                .As<ISagaRepository<SimpleSaga>>()
                .SingleInstance();

            var container = builder.Build();

            var busControl = Bus.Factory.CreateUsingInMemory(x =>
            {
                x.ReceiveEndpoint("input_queue", e => e.LoadFrom(container));
            });

            var busHandle = await busControl.StartAsync();

            await busHandle.Ready;

            await busHandle.StopAsync();
        }
    }


    public class TestConsumer :
        IConsumer<PingMessage>
    {
        public async Task Consume(ConsumeContext<PingMessage> context)
        {
        }
    }
}
