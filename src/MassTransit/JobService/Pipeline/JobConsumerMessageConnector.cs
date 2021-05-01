namespace MassTransit.JobService.Pipeline
{
    using System;
    using Components;
    using Components.Consumers;
    using Configuration;
    using ConsumeConfigurators;
    using ConsumeConnectors;
    using ConsumerSpecifications;
    using GreenPipes;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.ConsumerFactories;
    using MassTransit.Pipeline.Filters;
    using Metadata;
    using Util;


    public class JobConsumerMessageConnector<TConsumer, TJob> :
        IConsumerMessageConnector<TConsumer>
        where TConsumer : class, IJobConsumer<TJob>
        where TJob : class
    {
        readonly IConsumerConnector _finalizeJobConsumerConnector;
        readonly IConsumerConnector _startJobConsumerConnector;
        readonly IConsumerConnector _submitJobConsumerConnector;
        readonly IConsumerConnector _superviseJobConsumerConnector;

        public JobConsumerMessageConnector()
        {
            _submitJobConsumerConnector = ConsumerConnectorCache<SubmitJobConsumer<TJob>>.Connector;
            _startJobConsumerConnector = ConsumerConnectorCache<StartJobConsumer<TJob>>.Connector;
            _finalizeJobConsumerConnector = ConsumerConnectorCache<FinalizeJobConsumer<TJob>>.Connector;
            _superviseJobConsumerConnector = ConsumerConnectorCache<SuperviseJobConsumer>.Connector;
        }

        public Type MessageType => typeof(TJob);

        public IConsumerMessageSpecification<TConsumer> CreateConsumerMessageSpecification()
        {
            return new JobConsumerMessageSpecification<TConsumer, TJob>();
        }

        public ConnectHandle ConnectConsumer(IConsumePipeConnector consumePipe, IConsumerFactory<TConsumer> consumerFactory,
            IConsumerSpecification<TConsumer> specification)
        {
            var jobServiceOptions = specification.Options<JobServiceOptions>();
            var jobService = jobServiceOptions.JobService;

            if (jobService == null || jobServiceOptions.InstanceEndpointConfigurator == null)
            {
                throw new ConfigurationException(
                    "The job service must be configured prior to configuring a job consumer, using either ConfigureJobServiceEndpoints or ConfigureJobService");
            }

            var options = specification.Options<JobOptions<TJob>>();

            IConsumerMessageSpecification<TConsumer, TJob> messageSpecification = specification.GetMessageSpecification<TJob>();

            var jobSpecification = messageSpecification as JobConsumerMessageSpecification<TConsumer, TJob>;
            if (jobSpecification == null)
                throw new ArgumentException("The consumer specification did not match the message specification type");

            var submitJobHandle = ConnectSubmitJobConsumer(consumePipe, jobSpecification.SubmitJobSpecification, options);

            IPipe<ConsumeContext<TJob>> jobPipe = CreateJobPipe(consumerFactory, specification);
            var startJobHandle = ConnectStartJobConsumer(consumePipe, jobSpecification.StartJobSpecification, options, jobService, jobPipe);

            ConfigureStartJobConsumer(jobServiceOptions.InstanceEndpointConfigurator, options, jobService, jobPipe);

            var finalizeJobHandle = ConnectFinalizeJobConsumer(consumePipe, jobSpecification.FinalizeJobSpecification, options, jobService);

            var superviseJobHandle = ConnectSuperviseJobConsumer(consumePipe, jobSpecification.SuperviseJobSpecification, jobService);

            ConfigureSuperviseJobConsumer(jobServiceOptions.InstanceEndpointConfigurator, jobService);

            return new MultipleConnectHandle(submitJobHandle, startJobHandle, finalizeJobHandle, superviseJobHandle);
        }

        static IPipe<ConsumeContext<TJob>> CreateJobPipe(IConsumerFactory<TConsumer> consumerFactory, IConsumerSpecification<TConsumer> specification)
        {
            IConsumerMessageSpecification<TConsumer, TJob> messageSpecification = specification.GetMessageSpecification<TJob>();

            var options = specification.Options<JobOptions<TJob>>();

            var jobFilter = new JobConsumerMessageFilter<TConsumer, TJob>(options.RetryPolicy);

            IPipe<ConsumerConsumeContext<TConsumer, TJob>> consumerPipe = messageSpecification.Build(jobFilter);

            IPipe<ConsumeContext<TJob>> messagePipe = messageSpecification.BuildMessagePipe(x =>
            {
                x.UseFilter(new ConsumerMessageFilter<TConsumer, TJob>(consumerFactory, consumerPipe));
            });

            return messagePipe;
        }

        ConnectHandle ConnectSubmitJobConsumer(IConsumePipeConnector consumePipe,
            IConsumerSpecification<SubmitJobConsumer<TJob>> specification, JobOptions<TJob> options)
        {
            var consumerFactory = new DelegateConsumerFactory<SubmitJobConsumer<TJob>>(() => new SubmitJobConsumer<TJob>(options));

            return _submitJobConsumerConnector.ConnectConsumer(consumePipe, consumerFactory, specification);
        }

        ConnectHandle ConnectStartJobConsumer(IConsumePipeConnector consumePipe, IConsumerSpecification<StartJobConsumer<TJob>> specification,
            JobOptions<TJob> options, IJobService jobService, IPipe<ConsumeContext<TJob>> pipe)
        {
            var consumerFactory = new DelegateConsumerFactory<StartJobConsumer<TJob>>(() => new StartJobConsumer<TJob>(jobService, options, pipe));

            return _startJobConsumerConnector.ConnectConsumer(consumePipe, consumerFactory, specification);
        }

        ConnectHandle ConnectFinalizeJobConsumer(IConsumePipeConnector consumePipe, IConsumerSpecification<FinalizeJobConsumer<TJob>> specification,
            JobOptions<TJob> options, IJobService jobService)
        {
            var consumerFactory = new DelegateConsumerFactory<FinalizeJobConsumer<TJob>>(() => new FinalizeJobConsumer<TJob>(jobService, options,
                TypeMetadataCache<TConsumer>.ShortName));

            return _finalizeJobConsumerConnector.ConnectConsumer(consumePipe, consumerFactory, specification);
        }

        void ConfigureStartJobConsumer(IReceiveEndpointConfigurator configurator, JobOptions<TJob> options, IJobService jobService,
            IPipe<ConsumeContext<TJob>> pipe)
        {
            var consumerFactory = new DelegateConsumerFactory<StartJobConsumer<TJob>>(() => new StartJobConsumer<TJob>(jobService, options, pipe));

            var consumerConfigurator = new ConsumerConfigurator<StartJobConsumer<TJob>>(consumerFactory, configurator);

            configurator.AddEndpointSpecification(consumerConfigurator);
        }

        void ConfigureSuperviseJobConsumer(IReceiveEndpointConfigurator configurator, IJobService jobService)
        {
            var consumerFactory = new DelegateConsumerFactory<SuperviseJobConsumer>(() => new SuperviseJobConsumer(jobService));

            var consumerConfigurator = new ConsumerConfigurator<SuperviseJobConsumer>(consumerFactory, configurator);

            configurator.AddEndpointSpecification(consumerConfigurator);
        }

        ConnectHandle ConnectSuperviseJobConsumer(IConsumePipeConnector consumePipe, IConsumerSpecification<SuperviseJobConsumer> specification,
            IJobService jobService)
        {
            var consumerFactory = new DelegateConsumerFactory<SuperviseJobConsumer>(() => new SuperviseJobConsumer(jobService));

            return _superviseJobConsumerConnector.ConnectConsumer(consumePipe, consumerFactory, specification);
        }
    }
}
