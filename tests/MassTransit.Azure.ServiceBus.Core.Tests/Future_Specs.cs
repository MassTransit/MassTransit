namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Threading.Tasks;
    using DependencyInjection.Registration;
    using global::Azure;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.ForkJoint.Consumers;
    using TestFramework.ForkJoint.Contracts;
    using TestFramework.ForkJoint.Futures;
    using TestFramework.ForkJoint.Tests;


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
        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<FutureState> sagaConfigurator,
            IRegistrationContext context)
        {
            if (endpointConfigurator is IServiceBusEndpointConfigurator configurator)
                configurator.RequiresSession = true;

            base.ConfigureSaga(endpointConfigurator, sagaConfigurator, context);
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

            configurator.UsingAzureServiceBus((context, cfg) =>
            {
                cfg.Host(serviceUri, h =>
                {
                    h.NamedKeyCredential = new AzureNamedKeyCredential(Configuration.KeyName, Configuration.SharedAccessKey);
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
