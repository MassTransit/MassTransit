namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;


    public interface IOptionsSet
    {
        /// <summary>
        /// Configure the options, adding the option type if it is not present
        /// </summary>
        /// <param name="configure"></param>
        /// <typeparam name="T">The option type</typeparam>
        /// <returns></returns>
        T Options<T>(Action<T>? configure = null)
            where T : IOptions, new();

        /// <summary>
        /// Specify the options, will fault if it already exists
        /// </summary>
        /// <param name="options"></param>
        /// <param name="configure"></param>
        /// <typeparam name="T">The option type</typeparam>
        /// <returns></returns>
        T Options<T>(T options, Action<T>? configure = null)
            where T : IOptions;

        /// <summary>
        /// Return the options, if present
        /// </summary>
        /// <param name="options"></param>
        /// <typeparam name="T">The option type</typeparam>
        bool TryGetOptions<T>(out T options)
            where T : IOptions;

        /// <summary>
        /// Enumerate the options which are assignable to the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> SelectOptions<T>()
            where T : class;
    }
}
