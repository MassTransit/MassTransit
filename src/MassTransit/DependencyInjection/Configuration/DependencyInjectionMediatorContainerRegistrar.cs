namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DependencyInjection;
    using Mediator;
    using Microsoft.Extensions.DependencyInjection;


    public class DependencyInjectionMediatorContainerRegistrar :
        DependencyInjectionContainerRegistrar
    {
        public DependencyInjectionMediatorContainerRegistrar(IServiceCollection collection)
            : base(collection)
        {
        }

        public override IEnumerable<T> GetRegistrations<T>()
        {
            return Collection.Where(x => x.ServiceType == typeof(Bind<IMediator, T>))
                .Select(x => x.ImplementationInstance).Cast<Bind<IMediator, T>>()
                .Select(x => x.Value);
        }

        public override IEnumerable<T> GetRegistrations<T>(IServiceProvider provider)
        {
            return provider.GetService<IEnumerable<Bind<IMediator, T>>>().Select(x => x.Value) ?? Array.Empty<T>();
        }

        protected override void AddRegistration<T>(T value)
        {
            Collection.Add(ServiceDescriptor.Singleton(Bind<IMediator>.Create(value)));
        }

        protected override IClientFactory GetClientFactory(IServiceProvider provider)
        {
            return provider.GetRequiredService<IMediator>();
        }
    }
}
