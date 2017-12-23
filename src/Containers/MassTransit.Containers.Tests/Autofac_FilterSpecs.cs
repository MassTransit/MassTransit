// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Autofac;
    using AutofacIntegration;
    using Configuration;
    using ConsumeConfigurators;
    using GreenPipes;
    using NUnit.Framework;
    using TestFramework;
    using Testing;
    using Util;


    [TestFixture]
    public class Using_the_configuration_observers :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_fail_on_bad_validation()
        {
            Task<B> result = null;
            Request<A> request = await _harness.Bus.Request(_harness.InputQueueSendEndpoint, (new A {PostalCode = "74011"}), r =>
            {
                r.Timeout = _harness.TestTimeout;
                result = r.Handle<B>(context =>
                {
                });
            });

            Assert.That(async () => await result, Throws.TypeOf<RequestFaultException>());
        }

        [Test]
        public async Task Should_fail_on_bad_validation_at_the_request_task()
        {
            Task<B> result = null;
            Request<A> request = await _harness.Bus.Request(_harness.InputQueueSendEndpoint, (new A {PostalCode = "74011"}), r =>
            {
                r.Timeout = _harness.TestTimeout;
                result = r.Handle<B>(context =>
                {
                });
            });

            Assert.That(async () => await request.Task, Throws.TypeOf<RequestFaultException>());
        }

        [Test]
        public async Task Should_pass_a_good_validation()
        {
            Task<B> result = null;
            await _harness.Bus.Request(_harness.InputQueueSendEndpoint, (new A {PostalCode = "90210"}), r =>
            {
                r.Timeout = _harness.TestTimeout;
                result = r.Handle<B>(context =>
                {
                });
            });

            var b = await result;

            Assert.That(b.Success, Is.True);
        }

        InMemoryTestHarness _harness;
        ConsumerTestHarness<MyConsumer> _consumer;
        ConsumerConfigurationObserver _observer;
        IContainer _container;

        [OneTimeSetUp]
        public async Task A_consumer_is_being_tested()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<MyConsumer>();
            builder.RegisterGeneric(typeof(ValidatorFilter<>));

            builder.RegisterType<AValidator>()
                .As<IValidator<A>>();

            builder.RegisterType<ConsumerConfigurationObserver>()
                .SingleInstance();

            _container = builder.Build();

            _observer = _container.Resolve<ConsumerConfigurationObserver>();

            _harness = new InMemoryTestHarness();
            _harness.OnConfigureBus += ConfigureBus;
            _consumer = _harness.Consumer(new AutofacConsumerFactory<MyConsumer>(_container, "message"));

            await _harness.Start();
        }

        void ConfigureBus(IBusFactoryConfigurator configurator)
        {
            configurator.ConnectConsumerConfigurationObserver(_observer);
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _harness.Stop();
        }


        interface IValidator<in T>
            where T : class
        {
            void Validate(T message);
        }


        class AValidator :
            IValidator<A>
        {
            public void Validate(A message)
            {
                if (string.IsNullOrWhiteSpace(message.PostalCode))
                    throw new ArgumentException("The postal code was not specified");
                if (message.PostalCode != "90210")
                    throw new ArgumentException("Only Beverly Hills will do!");
            }
        }


        class ValidatorFilter<T> :
            IFilter<ConsumeContext<T>>
            where T : class
        {
            readonly IValidator<T> _validator;

            public ValidatorFilter(IValidator<T> validator)
            {
                _validator = validator;
            }

            public Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
            {
                _validator.Validate(context.Message);

                return next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
                context.CreateFilterScope("validator");
            }
        }


        class MyConsumer :
            IConsumer<A>,
            IConsumer<C>
        {
            public async Task Consume(ConsumeContext<A> context)
            {
                await context.RespondAsync(new B {Success = true});
            }

            public Task Consume(ConsumeContext<C> context)
            {
                return TaskUtil.Completed;
            }
        }


        class ConsumerConfigurationObserver :
            IConsumerConfigurationObserver
        {
            readonly HashSet<Type> _consumerTypes;
            readonly ILifetimeScope _lifetimeScope;
            readonly HashSet<Tuple<Type, Type>> _messageTypes;

            public ConsumerConfigurationObserver(ILifetimeScope lifetimeScope)
            {
                _lifetimeScope = lifetimeScope;
                _consumerTypes = new HashSet<Type>();
                _messageTypes = new HashSet<Tuple<Type, Type>>();
            }

            public HashSet<Type> ConsumerTypes => _consumerTypes;

            public HashSet<Tuple<Type, Type>> MessageTypes => _messageTypes;

            void IConsumerConfigurationObserver.ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
            {
                _consumerTypes.Add(typeof(TConsumer));
            }

            void IConsumerConfigurationObserver.ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
            {
                _messageTypes.Add(Tuple.Create(typeof(TConsumer), typeof(TMessage)));

                if (_lifetimeScope.IsRegistered<IValidator<TMessage>>())
                {
                    ValidatorFilter<TMessage> filter;
                    if (_lifetimeScope.TryResolve(out filter))
                    {
                        configurator.Message(m => m.UseFilter(filter));
                    }
                }
            }
        }


        public class A
        {
            public string PostalCode { get; set; }
        }


        public class B
        {
            public bool Success { get; set; }
        }


        public class C
        {
            public string Value { get; set; }
        }
    }
}