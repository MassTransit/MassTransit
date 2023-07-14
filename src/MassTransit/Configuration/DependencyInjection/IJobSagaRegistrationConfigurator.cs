namespace MassTransit
{
    using System;
    using Configuration;


    public interface IJobSagaRegistrationConfigurator
    {
        /// <summary>
        /// Configure all three saga endpoints (using the same configuration)
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        IJobSagaRegistrationConfigurator Endpoints(Action<IEndpointRegistrationConfigurator> configure);

        /// <summary>
        /// Configure the JobAttemptSaga endpoint
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        IJobSagaRegistrationConfigurator JobAttemptEndpoint(Action<IEndpointRegistrationConfigurator> configure);

        /// <summary>
        /// Configure the JobSaga endpoint
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        IJobSagaRegistrationConfigurator JobEndpoint(Action<IEndpointRegistrationConfigurator> configure);

        /// <summary>
        /// Configure the JobTypeSaga endpoint
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        IJobSagaRegistrationConfigurator JobTypeEndpoint(Action<IEndpointRegistrationConfigurator> configure);

        /// <summary>
        /// Internally used by the saga repositories to register as the saga repository for the job sagas
        /// </summary>
        /// <param name="registrationProvider"></param>
        /// <returns></returns>
        IJobSagaRegistrationConfigurator UseRepositoryRegistrationProvider(ISagaRepositoryRegistrationProvider registrationProvider);
    }
}
