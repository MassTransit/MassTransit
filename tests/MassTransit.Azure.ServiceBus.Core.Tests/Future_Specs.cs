namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using DependencyInjection.Registration;
    using MassTransit.TestFramework;
    using MassTransit.TestFramework.ForkJoint.Tests;
    using MassTransit.TestFramework.Futures.Tests;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework.ForkJoint.Consumers;
    using TestFramework.ForkJoint.Contracts;
    using TestFramework.ForkJoint.Futures;


    class AzureServiceBusFutureTestFixtureConfigurator :
        IFutureTestFixtureConfigurator
    {

        public void ConfigureFutureSagaRepository(IBusRegistrationConfigurator configurator)
        {
            configurator.AddSagaRepository<FutureState>()
                .MessageSessionRepository();
        }

        public void ConfigureServices(IServiceCollection collection)
        {
            
        }

        public async Task OneTimeSetup(IServiceProvider provider)
        {
            
        }

        public Task OneTimeTearDown(IServiceProvider provider)
        {
            return Task.CompletedTask;
        }
    }

    public class AzureServiceBusFutureDefinition<TFuture> : DefaultFutureDefinition<TFuture>
        where TFuture : class, SagaStateMachine<FutureState>
    {
        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<FutureState> sagaConfigurator)
        {
            if (endpointConfigurator is IServiceBusEndpointConfigurator configurator)
            {
                configurator.RequiresSession = true;
            }

            base.ConfigureSaga(endpointConfigurator, sagaConfigurator);
        }
    }


    [TestFixture]
    public class AzureServiceBusFryFutureSpecs :
        FryFuture_Specs
    {
        public AzureServiceBusFryFutureSpecs()
            : base(new AzureServiceBusFutureTestFixtureConfigurator())
        {
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<CookFryConsumer, CookFryConsumerDefinition>();
            configurator.AddFuture<FryFuture, AzureServiceBusFutureDefinition<FryFuture>>();

            var serviceUri = AzureServiceBusEndpointUriCreator.Create(Configuration.ServiceNamespace);
            ServiceBusTokenProviderSettings settings = new TestAzureServiceBusAccountSettings();

            configurator.UsingAzureServiceBus((context, cfg) =>
            {
                cfg.Host(serviceUri, h =>
                {
                    h.NamedKey(s =>
                    {
                        s.NamedKeyCredential = settings.NamedKeyCredential;
                    });
                });

                cfg.Send<OrderFry>(s =>
                {
                    s.UseSessionIdFormatter(x => x.Message.OrderLineId.ToString("D"));
                });

                cfg.ConfigureEndpoints(context);
            });
        }
    }

    //have not ported the other future specs    
}
