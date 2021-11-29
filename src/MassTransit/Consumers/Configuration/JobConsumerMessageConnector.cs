namespace MassTransit.Configuration
{
    using System;
    using Consumer;
    using JobService;
    using Middleware;
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

            var jobTypeId = jobService.GetJobTypeId<TJob>();

            IConsumerMessageSpecification<TConsumer, TJob> messageSpecification = specification.GetMessageSpecification<TJob>();

            var jobSpecification = messageSpecification as JobConsumerMessageSpecification<TConsumer, TJob>;
            if (jobSpecification == null)
                throw new ArgumentException("The consumer specification did not match the message specification type");

            var submitJobHandle = ConnectSubmitJobConsumer(consumePipe, jobSpecification.SubmitJobSpecification, options, jobTypeId);

            IPipe<ConsumeContext<TJob>> jobPipe = CreateJobPipe(consumerFactory, specification);
            var startJobHandle = ConnectStartJobConsumer(consumePipe, jobSpecification.StartJobSpecification, options, jobTypeId, jobService, jobPipe);

            ConfigureInstanceStartJobConsumer(jobServiceOptions.InstanceEndpointConfigurator, options, jobTypeId, jobService, jobPipe);

            var finalizeJobHandle = ConnectFinalizeJobConsumer(consumePipe, jobSpecification.FinalizeJobSpecification, options, jobTypeId, jobService);

            return new MultipleConnectHandle(submitJobHandle, startJobHandle, finalizeJobHandle);
        }

        static IPipe<ConsumeContext<TJob>> CreateJobPipe(IConsumerFactory<TConsumer> consumerFactory, IConsumerSpecification<TConsumer> specification)
        {
            IConsumerMessageSpecification<TConsumer, TJob> messageSpecification = specification.GetMessageSpecification<TJob>();

            var options = specification.Options<JobOptions<TJob>>();

            var jobFilter = new JobConsumerMessageFilter<TConsumer, TJob>(options.RetryPolicy);

            IPipe<ConsumerConsumeContext<TConsumer, TJob>> consumerPipe = messageSpecification.Build(jobFilter);

            IPipe<ConsumeContext<TJob>> messagePipe = messageSpecification.BuildMessagePipe(x =>
            {
                specification.ConfigureMessagePipe(x);

                x.UseFilter(new ConsumerMessageFilter<TConsumer, TJob>(consumerFactory, consumerPipe));
            });

            return messagePipe;
        }

        ConnectHandle ConnectSubmitJobConsumer(IConsumePipeConnector consumePipe,
            IConsumerSpecification<SubmitJobConsumer<TJob>> specification, JobOptions<TJob> options, Guid jobTypeId)
        {
            var consumerFactory = new DelegateConsumerFactory<SubmitJobConsumer<TJob>>(() => new SubmitJobConsumer<TJob>(options, jobTypeId));

            return _submitJobConsumerConnector.ConnectConsumer(consumePipe, consumerFactory, specification);
        }

        ConnectHandle ConnectStartJobConsumer(IConsumePipeConnector consumePipe, IConsumerSpecification<StartJobConsumer<TJob>> specification,
            JobOptions<TJob> options, Guid jobTypeId, IJobService jobService, IPipe<ConsumeContext<TJob>> pipe)
        {
            var consumerFactory = new DelegateConsumerFactory<StartJobConsumer<TJob>>(() => new StartJobConsumer<TJob>(jobService, options, jobTypeId, pipe));

            return _startJobConsumerConnector.ConnectConsumer(consumePipe, consumerFactory, specification);
        }

        ConnectHandle ConnectFinalizeJobConsumer(IConsumePipeConnector consumePipe, IConsumerSpecification<FinalizeJobConsumer<TJob>> specification,
            JobOptions<TJob> options, Guid jobTypeId, IJobService jobService)
        {
            var consumerFactory = new DelegateConsumerFactory<FinalizeJobConsumer<TJob>>(() => new FinalizeJobConsumer<TJob>(jobService, options, jobTypeId,
                TypeCache<TConsumer>.ShortName));

            return _finalizeJobConsumerConnector.ConnectConsumer(consumePipe, consumerFactory, specification);
        }

        static void ConfigureInstanceStartJobConsumer(IReceiveEndpointConfigurator configurator, JobOptions<TJob> options, Guid jobTypeId,
            IJobService jobService,
            IPipe<ConsumeContext<TJob>> pipe)
        {
            var consumerFactory = new DelegateConsumerFactory<StartJobConsumer<TJob>>(() => new StartJobConsumer<TJob>(jobService, options, jobTypeId, pipe));

            var consumerConfigurator = new ConsumerConfigurator<StartJobConsumer<TJob>>(consumerFactory, configurator);

            configurator.AddEndpointSpecification(consumerConfigurator);
        }
    }
}
