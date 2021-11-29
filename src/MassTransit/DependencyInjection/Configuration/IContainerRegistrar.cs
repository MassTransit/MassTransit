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

        /// <summary>
        /// Gets or adds a registration from the service collection
        /// </summary>
        /// <param name="type"></param>
        /// <param name="missingRegistrationFactory"></param>
        /// <typeparam name="T">The registration type</typeparam>
        /// <returns></returns>
        T GetOrAdd<T>(Type type, Func<Type, T> missingRegistrationFactory = default)
            where T : class, IRegistration;

        /// <summary>
        /// Returns registrations from the service collection, prior to container construction
        /// </summary>
        /// <typeparam name="T">The registration type</typeparam>
        /// <returns></returns>
        IEnumerable<T> GetRegistrations<T>()
            where T : class, IRegistration;
    }
}
