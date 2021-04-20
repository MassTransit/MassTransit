namespace MassTransit.Containers.Tests.DependencyInjection_Tests
{
    using System;
    using Common_Tests;
    using Mediator;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Registration;


    [TestFixture]
    public class DependencyInjection_Mediator :
        Common_Mediator
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_Mediator()
        {
            _provider = new ServiceCollection()
                .AddMediator(ConfigureRegistration)
                .AddMassTransit(x => x.AddBus(provider => BusControl))
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


    [TestFixture]
    public class DependencyInjection_Mediator_Request_Filter :
        Common_Mediator_Request_Filter
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_Mediator_Request_Filter()
        {
            _provider = new ServiceCollection()
                .AddMediator(ConfigureRegistration)
                .BuildServiceProvider(true);
        }

        protected override IRequestClient<T> GetRequestClient<T>()
            where T : class
        {
            var scope = _provider.CreateScope();
            return scope.ServiceProvider.GetRequiredService<IRequestClient<T>>();
        }

        protected override void ConfigureFilters(IMediatorRegistrationContext context, IMediatorConfigurator configurator)
        {
            DependencyInjectionFilterExtensions.UseSendFilter(configurator, typeof(PongFilter<>), context);
        }
    }


    [TestFixture]
    public class DependencyInjection_Mediator_FilterScope :
        Common_Mediator_FilterScope
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_Mediator_FilterScope()
        {
            _provider = new ServiceCollection()
                .AddSingleton(EasyASource)
                .AddSingleton(EasyBSource)
                .AddSingleton(ScopedContextSource)
                .AddSingleton(AsyncTestHarness)
                .AddScoped<ScopedContext>()
                .AddScoped(typeof(ScopedConsumeFilter<>))
                .AddScoped(typeof(ScopedSendFilter<>))
                .AddMediator(ConfigureRegistration)
                .BuildServiceProvider(true);
        }

        protected override IMediator Mediator => _provider.GetRequiredService<IMediator>();

        protected override void ConfigureFilters(IMediatorRegistrationContext context, IMediatorConfigurator configurator)
        {
            DependencyInjectionFilterExtensions.UseConsumeFilter(configurator, typeof(ScopedConsumeFilter<>), context);
            DependencyInjectionFilterExtensions.UseSendFilter(configurator, typeof(ScopedSendFilter<>), context);
        }
    }
}
