namespace MassTransit.Containers.Tests.DependencyInjection_Tests
{
    using System;
    using Common_Tests;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;


    [TestFixture]
    public class DependencyInjection_RequestClient_Context
        : RequestClient_Context
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_RequestClient_Context()
        {
            var collection = new ServiceCollection();
            collection.AddMassTransit(x =>
            {
                x.AddConsumer<InitialConsumer>();
                x.AddConsumer<SubsequentConsumer>();

                x.AddBus(context => BusControl);

                x.AddRequestClient<InitialRequest>(InputQueueAddress);
                x.AddRequestClient(typeof(SubsequentRequest), SubsequentQueueAddress);
            });

            collection.AddSingleton<IConsumeMessageObserver<InitialRequest>>(context => GetConsumeObserver<InitialRequest>());

            _provider = collection.BuildServiceProvider(true);
        }

        protected override IRequestClient<InitialRequest> RequestClient => _provider.CreateRequestClient<InitialRequest>();

        protected override IBusRegistrationContext Registration => _provider.GetRequiredService<IBusRegistrationContext>();
    }
}
