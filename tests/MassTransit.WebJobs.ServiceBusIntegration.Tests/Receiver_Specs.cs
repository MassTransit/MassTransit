
namespace MassTransit.WebJobs.ServiceBusIntegration.Tests
{
    using MassTransit.Testing;
    using MassTransit.TestFramework.Messages;
    using NUnit.Framework;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Registration;
    using MassTransit.ConsumeConfigurators;
    using MassTransit.Configuration;
    using MassTransit.Riders;
    using MassTransit.Azure.ServiceBus.Core.Configuration;
    using MassTransit.Azure.ServiceBus.Core;
    using System.Linq;
    using System.Collections.Generic;

    [TestFixture]
    public class Handling_a_message
    {
        [Test]
        public async Task Should_support_concurrent_invocations()
        {
            const string queueNamePrefix = "test-queue-";

            var topologyConfiguration = new ServiceBusTopologyConfiguration(AzureBusFactory.MessageTopology);
            var busConfiguration = new ServiceBusBusConfiguration(topologyConfiguration);

            var busRegistrationContext = new TestBusRegistrationContext();
            var busInstance = new TestBusInstance
            {
                HostConfiguration = busConfiguration.HostConfiguration
            };
            var messageReceiver = new MessageReceiver(busRegistrationContext, null, busInstance);

            var callsToTest = new List<Func<Task>>
            {
                () => messageReceiver.Handle(queueNamePrefix + "1", new Microsoft.Azure.ServiceBus.Message(), CancellationToken.None),
                () => messageReceiver.HandleConsumer<PingConsumer>(queueNamePrefix + "2", new Microsoft.Azure.ServiceBus.Message(), CancellationToken.None),
            };

            await Task.WhenAll(Enumerable.Range(0, 10).SelectMany(_ =>
                callsToTest.Select(call =>
                    Task.Run(async () =>
                    {
                        try
                        {
                            await call();
                        }
                        catch (InvalidOperationException)
                        {
                            // This occurs because the Microsoft.Azure.ServiceBus.Message (which we can't construct properly)
                            // has DeliveryCount = 0 which it detects as invalid in the getter. If we get this far without
                            // detecting concurrent calls to the Configure* methods then all should be fine.
                        }
                    }))));
        }

        class TestBusRegistrationContext :
            IBusRegistrationContext
        {
            int runningCount = 0;

            void AssertNotCalledConcurrently()
            {
                var newRunningCount = Interlocked.Increment(ref runningCount);
                Assert.AreEqual(1, newRunningCount, "Detected concurrent executions of this non-threadsafe code.");
                Thread.Sleep(100);
                Interlocked.Decrement(ref runningCount);
            }

            public void ConfigureActivity(Type activityType, IReceiveEndpointConfigurator executeEndpointConfigurator, IReceiveEndpointConfigurator compensateEndpointConfigurator)
            {
                AssertNotCalledConcurrently();
            }

            public void ConfigureActivityCompensate(Type activityType, IReceiveEndpointConfigurator compensateEndpointConfigurator)
            {
                AssertNotCalledConcurrently();
            }

            public void ConfigureActivityExecute(Type activityType, IReceiveEndpointConfigurator executeEndpointConfigurator, Uri compensateAddress)
            {
                AssertNotCalledConcurrently();
            }

            public void ConfigureConsumer(Type consumerType, IReceiveEndpointConfigurator configurator)
            {
                AssertNotCalledConcurrently();
            }

            public void ConfigureConsumers(IReceiveEndpointConfigurator configurator)
            {
                AssertNotCalledConcurrently();
            }

            public void ConfigureEndpoints<T>(IReceiveConfigurator<T> configurator, IEndpointNameFormatter endpointNameFormatter) where T : IReceiveEndpointConfigurator
            {
                AssertNotCalledConcurrently();
            }

            public void ConfigureEndpoints<T>(IReceiveConfigurator<T> configurator, IEndpointNameFormatter endpointNameFormatter, Action<IRegistrationFilterConfigurator> configureFilter) where T : IReceiveEndpointConfigurator
            {
                AssertNotCalledConcurrently();
            }

            public void ConfigureExecuteActivity(Type activityType, IReceiveEndpointConfigurator configurator)
            {
                AssertNotCalledConcurrently();
            }

            public void ConfigureSaga(Type sagaType, IReceiveEndpointConfigurator configurator)
            {
                AssertNotCalledConcurrently();
            }

            public void ConfigureSagas(IReceiveEndpointConfigurator configurator)
            {
                AssertNotCalledConcurrently();
            }

            public IConfigureReceiveEndpoint GetConfigureReceiveEndpoints()
            {
                throw new NotImplementedException();
            }

            public T GetRequiredService<T>() where T : class
            {
                throw new NotImplementedException();
            }

            public T GetService<T>() where T : class
            {
                throw new NotImplementedException();
            }

            public object GetService(Type serviceType)
            {
                throw new NotImplementedException();
            }

            public void UseHealthCheck(IBusFactoryConfigurator configurator)
            {
                AssertNotCalledConcurrently();
            }

            void IRegistration.ConfigureConsumer<T>(IReceiveEndpointConfigurator configurator, Action<IConsumerConfigurator<T>> configure)
            {
                AssertNotCalledConcurrently();
            }

            void IRegistration.ConfigureSaga<T>(IReceiveEndpointConfigurator configurator, Action<ISagaConfigurator<T>> configure)
            {
                AssertNotCalledConcurrently();
            }

            public void ConfigureFuture(Type futureType, IReceiveEndpointConfigurator configurator)
            {
                AssertNotCalledConcurrently();
            }

            void IRegistration.ConfigureFuture<T>(IReceiveEndpointConfigurator configurator)
            {
                AssertNotCalledConcurrently();
            }
        }

        class TestBusInstance :
            IBusInstance
        {
            public Type InstanceType => throw new NotImplementedException();

            public IBus Bus => throw new NotImplementedException();

            public IBusControl BusControl => throw new NotImplementedException();

            public IHostConfiguration HostConfiguration { get; set; }

            public void Connect<TRider>(IRiderControl riderControl) where TRider : IRider
            {
                throw new NotImplementedException();
            }

            public HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter, Action<IBusRegistrationContext, IReceiveEndpointConfigurator> configure = null)
            {
                throw new NotImplementedException();
            }

            public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IBusRegistrationContext, IReceiveEndpointConfigurator> configure = null)
            {
                throw new NotImplementedException();
            }

            public TRider GetRider<TRider>() where TRider : IRider
            {
                throw new NotImplementedException();
            }
        }

        class PingConsumer :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                return Task.CompletedTask;
            }
        }
    }
}
