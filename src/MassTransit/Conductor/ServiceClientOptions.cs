namespace MassTransit.Conductor
{
    using System;
    using Configuration;


    public class ServiceClientOptions :
        OptionsSet
    {
        readonly OptionsSet _optionsSet;

        public ServiceClientOptions()
        {
            _optionsSet = new OptionsSet();
        }

        /// <summary>
        /// Configure options on the service instance, which may be used to configure conductor capabilities
        /// </summary>
        /// <param name="configure"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ServiceClientOptions ConfigureOptions<T>(Action<T> configure = null)
            where T : IOptions, new()
        {
            _optionsSet.Options(configure);

            return this;
        }
    }
}
