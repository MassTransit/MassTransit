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
                .AddMassTransit(ConfigureRegistration)
                .BuildServiceProvider(true);
        }

        protected override IMediator Mediator => _provider.GetRequiredService<IMediator>();
    }
}
