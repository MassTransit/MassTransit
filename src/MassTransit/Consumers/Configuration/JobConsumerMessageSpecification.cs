namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JobService;
    using Util;


    public class JobConsumerMessageSpecification<TConsumer, TJob> :
        IConsumerMessageSpecification<TConsumer, TJob>
        where TJob : class
        where TConsumer : class, IJobConsumer<TJob>
    {
        readonly ConsumerMessageSpecification<TConsumer, TJob> _consumerSpecification;

        public JobConsumerMessageSpecification()
        {
            SubmitJobSpecification = ConsumerConnectorCache<SubmitJobConsumer<TJob>>.Connector.CreateConsumerSpecification<SubmitJobConsumer<TJob>>();
            StartJobSpecification = ConsumerConnectorCache<StartJobConsumer<TJob>>.Connector.CreateConsumerSpecification<StartJobConsumer<TJob>>();
            FinalizeJobSpecification = ConsumerConnectorCache<FinalizeJobConsumer<TJob>>.Connector.CreateConsumerSpecification<FinalizeJobConsumer<TJob>>();
            SuperviseJobSpecification = ConsumerConnectorCache<SuperviseJobConsumer>.Connector.CreateConsumerSpecification<SuperviseJobConsumer>();

            _consumerSpecification = new ConsumerMessageSpecification<TConsumer, TJob>();
        }

        public IConsumerSpecification<SubmitJobConsumer<TJob>> SubmitJobSpecification { get; }
        public IConsumerSpecification<StartJobConsumer<TJob>> StartJobSpecification { get; }
        public IConsumerSpecification<FinalizeJobConsumer<TJob>> FinalizeJobSpecification { get; }
        public IConsumerSpecification<SuperviseJobConsumer> SuperviseJobSpecification { get; }

        public void AddPipeSpecification(IPipeSpecification<ConsumerConsumeContext<TConsumer, TJob>> specification)
        {
            _consumerSpecification.AddPipeSpecification(specification);
        }

        public void Message(Action<IConsumerMessageConfigurator<TJob>> configure)
        {
            _consumerSpecification.Message(configure);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _consumerSpecification.Validate()
                .Concat(SuperviseJobSpecification.Validate())
                .Concat(StartJobSpecification.Validate())
                .Concat(FinalizeJobSpecification.Validate())
                .Concat(SubmitJobSpecification.Validate());
        }

        public Type MessageType => typeof(TJob);

        public bool TryGetMessageSpecification<TC, T>(out IConsumerMessageSpecification<TC, T> specification)
            where T : class
            where TC : class
        {
            specification = this as IConsumerMessageSpecification<TC, T>;
            return specification != null;
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext<TJob>> specification)
        {
            _consumerSpecification.AddPipeSpecification(specification);
        }

        public IPipe<ConsumerConsumeContext<TConsumer, TJob>> Build(IFilter<ConsumerConsumeContext<TConsumer, TJob>> consumeFilter)
        {
            return _consumerSpecification.Build(consumeFilter);
        }

        public IPipe<ConsumeContext<TJob>> BuildMessagePipe(Action<IPipeConfigurator<ConsumeContext<TJob>>> configure)
        {
            return _consumerSpecification.BuildMessagePipe(configure);
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumerConsumeContext<TConsumer>> specification)
        {
            _consumerSpecification.AddPipeSpecification(new ConsumerPipeSpecificationProxy<TConsumer, TJob>(specification));
        }

        public ConnectHandle ConnectConsumerConfigurationObserver(IConsumerConfigurationObserver observer)
        {
            return new MultipleConnectHandle(_consumerSpecification.ConnectConsumerConfigurationObserver(observer),
                SuperviseJobSpecification.ConnectConsumerConfigurationObserver(observer),
                StartJobSpecification.ConnectConsumerConfigurationObserver(observer),
                FinalizeJobSpecification.ConnectConsumerConfigurationObserver(observer),
                SubmitJobSpecification.ConnectConsumerConfigurationObserver(observer));
        }
    }
}
