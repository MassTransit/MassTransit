namespace MassTransit.Registration
{
    using System;


    /// <summary>
    /// Supports retrieval of services used by the ConfigurationRegistry
    /// </summary>
    public interface IConfigurationServiceProvider :
        IServiceProvider
    {
        /// <summary>
        /// Get the requested service or throw an exception that it was not available
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetRequiredService<T>()
            where T : class;

        /// <summary>
        /// Returns the service, if available, otherwise returns null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetService<T>()
            where T : class;
    }
}
