// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.EndpointSpecifications
{
    using System.Collections.Generic;
    using System.Linq;
    using ConsumePipeSpecifications;
    using GreenPipes;
    using Pipeline;
    using Pipeline.Pipes;
    using PublishPipeSpecifications;
    using SendPipeSpecifications;
    using Topology.Configuration;
    using Topology.Observers;


    public abstract class EndpointConfiguration<TConfiguration, TConsumeTopology, TSendTopology, TPublishTopology> :
        IEndpointConfiguration<TConfiguration, TConsumeTopology, TSendTopology, TPublishTopology>
        where TConsumeTopology : IConsumeTopologyConfigurator
        where TSendTopology : ISendTopologyConfigurator
        where TPublishTopology : IPublishTopologyConfigurator
        where TConfiguration : IEndpointConfiguration<TConfiguration, TConsumeTopology, TSendTopology, TPublishTopology>
    {
        readonly IConsumePipe _consumePipe;
        readonly ConsumePipeSpecification _consumePipeSpecification;
        readonly PublishPipeSpecification _publishPipeSpecification;
        readonly ConnectHandle _publishToSendTopologyHandle;
        readonly SendPipeSpecification _sendPipeSpecification;

        protected EndpointConfiguration(TConsumeTopology consumeTopology, TSendTopology sendTopology, TPublishTopology publishTopology)
        {
            _consumePipeSpecification = new ConsumePipeSpecification();

            ConsumeTopology = consumeTopology;

            SendTopology = sendTopology;

            _sendPipeSpecification = new SendPipeSpecification();
            _sendPipeSpecification.Connect(new TopologySendPipeSpecificationObserver(sendTopology));

            PublishTopology = publishTopology;

            var observer = new PublishToSendTopologyConfigurationObserver(sendTopology);
            _publishToSendTopologyHandle = publishTopology.Connect(observer);

            _publishPipeSpecification = new PublishPipeSpecification();
            _publishPipeSpecification.Connect(new TopologyPublishPipeSpecificationObserver(publishTopology));
        }

        protected EndpointConfiguration(TConsumeTopology consumeTopology, TSendTopology sendTopology,
            TPublishTopology publishTopology,
            TConfiguration parentConfiguration, IConsumePipe consumePipe = null)
            : this(consumeTopology, sendTopology, publishTopology)
        {
            _consumePipe = consumePipe;

            _consumePipeSpecification.Connect(new ParentConsumePipeSpecificationObserver(parentConfiguration.ConsumePipeSpecification));

            _sendPipeSpecification.Connect(new ParentSendPipeSpecificationObserver(parentConfiguration.SendPipeSpecification));

            _publishPipeSpecification.Connect(new ParentPublishPipeSpecificationObserver(parentConfiguration.PublishPipeSpecification));
        }

        public TPublishTopology PublishTopology { get; }
        public TConsumeTopology ConsumeTopology { get; }
        public TSendTopology SendTopology { get; }

        public IConsumePipeSpecification ConsumePipeSpecification => _consumePipeSpecification;
        public ISendPipeSpecification SendPipeSpecification => _sendPipeSpecification;
        public IPublishPipeSpecification PublishPipeSpecification => _publishPipeSpecification;

        public IConsumePipeConfigurator ConsumePipeConfigurator => _consumePipeSpecification;
        public ISendPipeConfigurator SendPipeConfigurator => _sendPipeSpecification;
        public IPublishPipeConfigurator PublishPipeConfigurator => _publishPipeSpecification;

        public IConsumePipe CreateConsumePipe()
        {
            return _consumePipe ?? _consumePipeSpecification.BuildConsumePipe();
        }

        public ISendPipe CreateSendPipe()
        {
            return new SendPipe(_sendPipeSpecification);
        }

        public IPublishPipe CreatePublishPipe()
        {
            return new PublishPipe(_publishPipeSpecification);
        }

        public abstract TConfiguration CreateConfiguration(IConsumePipe consumePipe = null);

        public void SeparatePublishFromSendTopology()
        {
            _publishToSendTopologyHandle.Disconnect();
        }

        public virtual IEnumerable<ValidationResult> Validate()
        {
            return _consumePipeSpecification.Validate()
                .Concat(_sendPipeSpecification.Validate())
                .Concat(_publishPipeSpecification.Validate());
        }
    }
}