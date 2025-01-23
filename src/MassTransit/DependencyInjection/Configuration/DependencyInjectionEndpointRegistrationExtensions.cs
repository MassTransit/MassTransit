namespace MassTransit.Configuration
{
    using System;
    using DependencyInjection.Registration;
    using Internals;
    using Microsoft.Extensions.DependencyInjection;


    public static class DependencyInjectionEndpointRegistrationExtensions
    {
        public static IEndpointRegistration RegisterEndpoint<TDefinition, T>(this IServiceCollection collection, IRegistration registration,
            IEndpointSettings<IEndpointDefinition<T>> settings = null)
            where TDefinition : class, IEndpointDefinition<T>
            where T : class
        {
            return RegisterEndpoint<TDefinition, T>(collection, new DependencyInjectionContainerRegistrar(collection), registration, settings);
        }

        public static IEndpointRegistration RegisterEndpoint<TDefinition, T>(this IServiceCollection collection, IContainerRegistrar registrar,
            IRegistration registration, IEndpointSettings<IEndpointDefinition<T>> settings = null)
            where T : class
            where TDefinition : class, IEndpointDefinition<T>
        {
            return new EndpointRegistrar<TDefinition, T>(registration).Register(registrar, settings);
        }

        public static IEndpointRegistration RegisterEndpoint(this IServiceCollection collection, Type endpointDefinitionType)
        {
            return RegisterEndpoint(collection, new DependencyInjectionContainerRegistrar(collection), endpointDefinitionType);
        }

        public static IEndpointRegistration RegisterEndpoint(this IServiceCollection collection, IContainerRegistrar registrar, Type endpointDefinitionType)
        {
            if (!endpointDefinitionType.ClosesType(typeof(IEndpointDefinition<>), out Type[] types))
                throw new ArgumentException($"{TypeCache.GetShortName(endpointDefinitionType)} is not an endpoint definition", nameof(endpointDefinitionType));

            var register = (IEndpointRegistrar)Activator.CreateInstance(typeof(EndpointRegistrar<,>).MakeGenericType(endpointDefinitionType, types[0]));

            return register.Register(registrar);
        }


        interface IEndpointRegistrar
        {
            IEndpointRegistration Register(IContainerRegistrar registrar);
        }


        class EndpointRegistrar<TDefinition, T> :
            IEndpointRegistrar
            where TDefinition : class, IEndpointDefinition<T>
            where T : class
        {
            readonly IRegistration _registration;

            public EndpointRegistrar(IRegistration registration)
            {
                _registration = registration;
            }

            public IEndpointRegistration Register(IContainerRegistrar registrar)
            {
                registrar.AddEndpointDefinition<T, TDefinition>();

                return registrar.GetOrAddRegistration<IEndpointRegistration>(typeof(T), _ => new EndpointRegistration<T>(_registration, registrar));
            }

            public IEndpointRegistration Register(IContainerRegistrar registrar, IEndpointSettings<IEndpointDefinition<T>> settings)
            {
                registrar.AddEndpointDefinition<T, TDefinition>(settings);

                return registrar.GetOrAddRegistration<IEndpointRegistration>(typeof(T), _ => new EndpointRegistration<T>(_registration, registrar));
            }
        }
    }
}
