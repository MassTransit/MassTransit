namespace MassTransit.Containers.Tests.DependencyInjection_Tests
{
    using System;
    using Common_Tests;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;


    [TestFixture]
    public class DependencyInjection_Scope :
        Common_Scope
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_Scope()
        {
            var collection = new ServiceCollection();
            collection.AddMassTransit(x =>
            {
                x.AddBus(provider => BusControl);
            });

            _provider = collection.BuildServiceProvider(true);
        }

        protected override ISendEndpointProvider GetSendEndpointProvider()
        {
            return _provider.GetRequiredService<IBus>();
        }

        protected override IPublishEndpoint GetPublishEndpoint()
        {
            return _provider.GetRequiredService<IBus>();
        }
    }
}
