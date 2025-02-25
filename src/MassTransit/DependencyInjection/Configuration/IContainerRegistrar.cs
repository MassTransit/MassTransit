#nullable enable
namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;


    public interface IContainerRegistrar :
        IContainerSelector
    {
        void RegisterRequestClient<T>(RequestTimeout timeout = default)
            where T : class;

        void RegisterRequestClient<T>(Uri destinationAddress, RequestTimeout timeout = default)
            where T : class;

        void RegisterScopedClientFactory();

        void RegisterEndpointNameFormatter(IEndpointNameFormatter endpointNameFormatter);

        /// <summary>
        /// Gets or adds a registration from the service collection
        /// </summary>
        /// <param name="type"></param>
        /// <param name="missingRegistrationFactory"></param>
        /// <typeparam name="T">The registration type</typeparam>
        /// <returns></returns>
        T GetOrAddRegistration<T>(Type type, Func<Type, T>? missingRegistrationFactory = default)
            where T : class, IRegistration;

        /// <summary>
        /// Returns registrations from the service collection, prior to container construction
        /// </summary>
        /// <typeparam name="T">The registration type</typeparam>
        /// <returns></returns>
        IEnumerable<T> GetRegistrations<T>()
            where T : class, IRegistration;

        /// <summary>
        /// Gets or adds a definition from the service collection
        /// </summary>
        /// <typeparam name="T">The definition type</typeparam>
        /// <typeparam name="TDefinition">The definition implementation</typeparam>
        /// <returns></returns>
        void AddDefinition<T, TDefinition>()
            where T : class, IDefinition
            where TDefinition : class, T;

        void AddEndpointDefinition<T, TDefinition>(IEndpointSettings<IEndpointDefinition<T>>? settings = null)
            where T : class
            where TDefinition : class, IEndpointDefinition<T>;
    }
}
