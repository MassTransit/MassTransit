// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Threading.Tasks;
    using Autofac;
    using NUnit.Framework;
    using TestFramework;


    public class Service_registration_in_consumer_scope :
        InMemoryTestFixture
    {
        readonly IContainer _container;
        readonly TaskCompletionSource<Service> _source;

        public Service_registration_in_consumer_scope()
        {
            _source = GetTask<Service>();
            var builder = new ContainerBuilder();
            builder.RegisterType<BaseService>().As<Service>();
            builder.RegisterInstance(_source).AsSelf();
            builder.RegisterType<Consumer>().AsSelf();
            _container = builder.Build();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            configurator.Consumer<Consumer>(_container, "message", (b, context) =>
            {
                b.RegisterType<ConsumerService>().As<Service>();
            });
        }

        [Test]
        public async Task Should_Return_From_Scope()
        {
            await Bus.Publish(new Message());
            var result = await _source.Task;

            Assert.That(result, Is.TypeOf<ConsumerService>());
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }


        interface Service
        {
        }


        class BaseService : Service
        {
        }


        class ConsumerService : Service
        {
        }


        class Message
        {
        }


        class Consumer : IConsumer<Message>
        {
            readonly TaskCompletionSource<Service> _source;
            readonly Service _service;

            public Consumer(TaskCompletionSource<Service> source, Service service)
            {
                _source = source;
                _service = service;
            }

            public Task Consume(ConsumeContext<Message> context)
            {
                _source.TrySetResult(_service);
                return Task.CompletedTask;
            }
        }
    }


    public class Consumer_scope_context_payload :
        InMemoryTestFixture
    {
        readonly IContainer _container;
        readonly TaskCompletionSource<ILifetimeScope> _source;
        readonly string _tag;

        public Consumer_scope_context_payload()
        {
            _source = GetTask<ILifetimeScope>();
            _tag = "message";
            var builder = new ContainerBuilder();
            builder.RegisterInstance(_source).AsSelf();
            builder.RegisterType<Consumer>().AsSelf();
            _container = builder.Build();
        }

        [Test]
        public async Task Should_contains_lifetime_scope()
        {
            await Bus.Publish(new Message());
            var result = await _source.Task;

            Assert.NotNull(result);
            Assert.That(result.Tag, Is.EqualTo(_tag));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            configurator.Consumer<Consumer>(_container, _tag);
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }


        class Message
        {
        }


        class Consumer : IConsumer<Message>
        {
            readonly TaskCompletionSource<ILifetimeScope> _source;

            public Consumer(TaskCompletionSource<ILifetimeScope> source)
            {
                _source = source;
            }

            public Task Consume(ConsumeContext<Message> context)
            {
                _source.TrySetResult(context.TryGetPayload(out ILifetimeScope scope) ? scope : null);
                return Task.CompletedTask;
            }
        }
    }
}
