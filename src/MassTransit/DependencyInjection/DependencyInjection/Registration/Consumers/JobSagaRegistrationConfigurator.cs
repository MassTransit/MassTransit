#nullable enable
namespace MassTransit.DependencyInjection.Registration
{
    using System;
    using Configuration;
    using Microsoft.Extensions.DependencyInjection;


    public class JobSagaRegistrationConfigurator :
        IJobSagaRegistrationConfigurator
    {
        readonly IBusRegistrationConfigurator _configurator;
        ISagaRegistrationConfigurator<JobAttemptSaga> _jobAttemptConfigurator;
        ISagaRegistrationConfigurator<JobSaga> _jobConfigurator;
        ISagaRegistrationConfigurator<JobTypeSaga> _jobTypeConfigurator;

        public JobSagaRegistrationConfigurator(IBusRegistrationConfigurator configurator, Action<JobSagaOptions>? configure)
        {
            _configurator = configurator;

            configurator.AddOptions<JobSagaOptions>()
                .Configure(options => configure?.Invoke(options));

            _jobTypeConfigurator = configurator.AddSagaStateMachine<JobTypeStateMachine, JobTypeSaga, JobTypeSagaDefinition>();
            _jobConfigurator = configurator.AddSagaStateMachine<JobStateMachine, JobSaga, JobSagaDefinition>();
            _jobAttemptConfigurator = configurator.AddSagaStateMachine<JobAttemptStateMachine, JobAttemptSaga, JobAttemptSagaDefinition>();
        }

        public IJobSagaRegistrationConfigurator Endpoints(Action<IEndpointRegistrationConfigurator> configure)
        {
            _jobAttemptConfigurator = _jobAttemptConfigurator.Endpoint(configure);
            _jobConfigurator = _jobConfigurator.Endpoint(configure);
            _jobTypeConfigurator = _jobTypeConfigurator.Endpoint(configure);
            return this;
        }

        public IJobSagaRegistrationConfigurator JobAttemptEndpoint(Action<IEndpointRegistrationConfigurator> configure)
        {
            _jobAttemptConfigurator = _jobAttemptConfigurator.Endpoint(configure);
            return this;
        }

        public IJobSagaRegistrationConfigurator JobEndpoint(Action<IEndpointRegistrationConfigurator> configure)
        {
            _jobConfigurator = _jobConfigurator.Endpoint(configure);
            return this;
        }

        public IJobSagaRegistrationConfigurator JobTypeEndpoint(Action<IEndpointRegistrationConfigurator> configure)
        {
            _jobTypeConfigurator = _jobTypeConfigurator.Endpoint(configure);
            return this;
        }

        public IJobSagaRegistrationConfigurator UseRepositoryRegistrationProvider(ISagaRepositoryRegistrationProvider registrationProvider)
        {
            registrationProvider.Configure(_jobAttemptConfigurator);
            registrationProvider.Configure(_jobConfigurator);
            registrationProvider.Configure(_jobTypeConfigurator);

            return this;
        }
    }
}
