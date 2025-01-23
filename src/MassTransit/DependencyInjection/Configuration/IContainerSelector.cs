#nullable enable
namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;


    /// <summary>
    /// Used to pull configuration from the container, scoped to the bus, multi-bus, or mediator
    /// </summary>
    public interface IContainerSelector
    {
        /// <summary>
        /// Returns the registration from the service provider, if it exists
        /// </summary>
        /// <param name="provider">The service provider</param>
        /// <param name="type">The registration target type (Consumer, Saga, Activity, etc.)</param>
        /// <param name="value"></param>
        /// <typeparam name="T">The registration type</typeparam>
        /// <returns></returns>
        bool TryGetRegistration<T>(IServiceProvider provider, Type type, [NotNullWhen(true)] out T? value)
            where T : class, IRegistration;

        IEnumerable<T> GetRegistrations<T>(IServiceProvider provider)
            where T : class, IRegistration;

        /// <summary>
        /// Returns the definition from the service provider, if it exists
        /// </summary>
        /// <param name="provider">The service provider</param>
        /// <typeparam name="T">The definition type</typeparam>
        /// <returns>The definition, if found, otherwise null</returns>
        T? GetDefinition<T>(IServiceProvider provider)
            where T : class, IDefinition;

        /// <summary>
        /// Returns the endpoint definition from the service provider, if it exists
        /// </summary>
        /// <param name="provider">The service provider</param>
        /// <typeparam name="T">The definition target type</typeparam>
        /// <returns></returns>
        IEndpointDefinition<T>? GetEndpointDefinition<T>(IServiceProvider provider)
            where T : class;

        IConfigureReceiveEndpoint GetConfigureReceiveEndpoints(IServiceProvider provider);

        /// <summary>
        /// Returns the endpoint name formatter registered for the bus instance
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        IEndpointNameFormatter GetEndpointNameFormatter(IServiceProvider provider);
    }
}
