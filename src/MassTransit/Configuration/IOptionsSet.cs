namespace MassTransit
{
    using System;
    using Configuration;


    public interface IOptionsSet
    {
        /// <summary>
        /// Configure the options, adding the option type if it is not present
        /// </summary>
        /// <param name="configure"></param>
        /// <typeparam name="T">The option type</typeparam>
        /// <returns></returns>
        T Options<T>(Action<T> configure = null)
            where T : IOptions, new();

        /// <summary>
        /// Return the options, if present
        /// </summary>
        /// <param name="options"></param>
        /// <typeparam name="T">The option type</typeparam>
        bool TryGetOptions<T>(out T options)
            where T : IOptions;
    }
}
