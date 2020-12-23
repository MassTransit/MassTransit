namespace MassTransit.JobService.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Components.Consumers;
    using ConsumeConfigurators;
    using ConsumeConnectors;
    using ConsumerSpecifications;
    using GreenPipes;
    using Util;


    public class JobConsumerMessageSpecification<TConsumer, TJob> :
        IConsumerMessageSpecification<TConsumer, TJob>
        where TJob : class
        where TConsumer : class, IJobConsumer<TJob>
    {
        readonly IConsumerSpecification<SubmitJobConsumer<TJob>> _submitJobSpecification;
        readonly IConsumerSpecification<StartJobConsumer<TJob>> _startJobSpecification;
        readonly IConsumerSpecification<SuperviseJobConsumer> _superviseJobSpecification;
        readonly ConsumerMessageSpecification<TConsumer, TJob> _consumerSpecification;

        public JobConsumerMessageSpecification()
        {
            _submitJobSpecification = ConsumerConnectorCache<SubmitJobConsumer<TJob>>.Connector.CreateConsumerSpecification<SubmitJobConsumer<TJob>>();
            _startJobSpecification = ConsumerConnectorCache<StartJobConsumer<TJob>>.Connector.CreateConsumerSpecification<StartJobConsumer<TJob>>();
            _superviseJobSpecification = ConsumerConnectorCache<SuperviseJobConsumer>.Connector.CreateConsumerSpecification<SuperviseJobConsumer>();

            _consumerSpecification = new ConsumerMessageSpecification<TConsumer, TJob>();
        }

        public IConsumerSpecification<SubmitJobConsumer<TJob>> SubmitJobSpecification => _submitJobSpecification;

        public IConsumerSpecification<StartJobConsumer<TJob>> StartJobSpecification => _startJobSpecification;

        public IConsumerSpecification<SuperviseJobConsumer> SuperviseJobSpecification => _superviseJobSpecification;

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
                .Concat(_superviseJobSpecification.Validate())
                .Concat(_startJobSpecification.Validate())
                .Concat(_submitJobSpecification.Validate());
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
                _superviseJobSpecification.ConnectConsumerConfigurationObserver(observer),
                _startJobSpecification.ConnectConsumerConfigurationObserver(observer),
                _submitJobSpecification.ConnectConsumerConfigurationObserver(observer));
        }
    }
}
