namespace MassTransit.Containers.Tests.DependencyInjection_Tests
{
    using System;
    using Common_Tests;
    using Mediator;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;


    [TestFixture]
    public class DependencyInjection_Mediator :
        Common_Mediator
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_Mediator()
        {
            _provider = new ServiceCollection()
                .AddMediator(ConfigureRegistration)
                .BuildServiceProvider(true);
        }

        protected override IMediator Mediator => _provider.GetRequiredService<IMediator>();
    }


    [TestFixture]
    public class DependencyInjection_Mediator_Bus :
        Common_Mediator_Bus
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_Mediator_Bus()
        {
            _provider = new ServiceCollection()
                .AddMediator(ConfigureRegistration)
                .AddMassTransit(x => x.AddBus(provider => BusControl))
                .BuildServiceProvider(true);
        }

        protected override IBusRegistrationContext Registration => _provider.GetRequiredService<IBusRegistrationContext>();

        protected override IMediator Mediator => _provider.GetRequiredService<IMediator>();
    }
}
