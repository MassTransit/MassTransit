namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// Used to pull registrations from the container, scoped to the bus, multi-bus, or mediator
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
        bool TryGetValue<T>(IServiceProvider provider, Type type, out T value)
            where T : class, IRegistration;

        IEnumerable<T> GetRegistrations<T>(IServiceProvider provider)
            where T : class, IRegistration;
    }
}
