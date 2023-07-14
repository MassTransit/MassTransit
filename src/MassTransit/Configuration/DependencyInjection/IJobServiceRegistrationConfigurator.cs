namespace MassTransit
{
    using System;


    public interface IJobServiceRegistrationConfigurator
    {
        /// <summary>
        /// Configure the job service options
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        IJobServiceRegistrationConfigurator Options(Action<JobConsumerOptions> configure);

        /// <summary>
        /// Configure the instance endpoint settings
        /// </summary>
        /// <param name="configure"></param>
        void Endpoint(Action<IEndpointRegistrationConfigurator> configure);
    }
}
