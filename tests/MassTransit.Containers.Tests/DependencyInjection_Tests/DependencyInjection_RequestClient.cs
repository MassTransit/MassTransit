namespace MassTransit.Containers.Tests.DependencyInjection_Tests
{
    using System;
    using Common_Tests;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;


    [TestFixture]
    public class DependencyInjection_RequestClient_Context
        : Common_RequestClient_Context
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_RequestClient_Context()
        {
            var collection = new ServiceCollection();
            collection.AddMassTransit(ConfigureRegistration);

            collection.AddSingleton<IConsumeMessageObserver<InitialRequest>>(context => GetConsumeObserver<InitialRequest>());

            _provider = collection.BuildServiceProvider(true);
        }

        protected override IRequestClient<InitialRequest> RequestClient => _provider.CreateRequestClient<InitialRequest>();

        protected override IBusRegistrationContext Registration => _provider.GetRequiredService<IBusRegistrationContext>();
    }


    [TestFixture]
    public class DependencyInjection_RequestClient_Generic
        : Common_RequestClient_Generic
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_RequestClient_Generic()
        {
            var collection = new ServiceCollection();
            collection.AddMassTransit(ConfigureRegistration);
            collection.AddGenericRequestClient();

            _provider = collection.BuildServiceProvider(true);
        }

        protected override IRequestClient<InitialRequest> RequestClient => _provider.CreateRequestClient<InitialRequest>();

        protected override IBusRegistrationContext Registration => _provider.GetRequiredService<IBusRegistrationContext>();
    }


    [TestFixture]
    public class DependencyInjection_RequestClient_Outbox
        : Common_RequestClient_Outbox
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_RequestClient_Outbox()
        {
            var collection = new ServiceCollection();
            collection.AddMassTransit(ConfigureRegistration);

            collection.AddSingleton<IConsumeMessageObserver<InitialRequest>>(context => GetConsumeObserver<InitialRequest>());

            _provider = collection.BuildServiceProvider(true);
        }

        protected override IRequestClient<InitialRequest> RequestClient => _provider.CreateRequestClient<InitialRequest>();

        protected override IBusRegistrationContext Registration => _provider.GetRequiredService<IBusRegistrationContext>();
    }


    [TestFixture]
    public class DependencyInjection_RequestClient_Outbox_Courier
        : Common_RequestClient_Outbox_Courier
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_RequestClient_Outbox_Courier()
        {
            var collection = new ServiceCollection();
            collection.AddMassTransit(ConfigureRegistration);

            _provider = collection.BuildServiceProvider(true);
        }

        protected override IBusRegistrationContext Registration => _provider.GetRequiredService<IBusRegistrationContext>();
    }
}
