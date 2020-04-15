namespace MassTransit.Containers.Tests.DependencyInjection_Tests
{
    using System;
    using Common_Tests;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Scoping;


    [TestFixture]
    public class DependencyInjection_ScopePublish :
        Common_ScopePublish<IServiceScope>
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_ScopePublish()
        {
            var collection = new ServiceCollection();
            collection.AddMassTransit(x =>
            {
                x.AddBus(provider => BusControl);
            });

            _provider = collection.BuildServiceProvider(true);
        }

        protected override IPublishScopeProvider GetPublishScopeProvider()
        {
            return _provider.GetRequiredService<IPublishScopeProvider>();
        }
    }
}
