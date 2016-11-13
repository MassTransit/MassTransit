// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Turnout.Configuration
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using GreenPipes;


    public class TurnoutServiceSpecification<TCommand> :
        IBusFactorySpecification,
        ITurnoutServiceConfigurator<TCommand>
        where TCommand : class
    {
        readonly IReceiveEndpointConfigurator _configurator;
        readonly IJobRegistry _jobRegistry;
        readonly Lazy<IJobService> _jobService;

        public TurnoutServiceSpecification(IReceiveEndpointConfigurator configurator)
        {
            _configurator = configurator;

            SuperviseInterval = TimeSpan.FromMinutes(1);
            _jobRegistry = new JobRegistry();
            PartitionCount = 8;

            _jobService = new Lazy<IJobService>(CreateJobService);
        }

        public IJobRegistry JobRegistry => _jobRegistry;

        public Uri ManagementAddress { private get; set; }

        public IJobService Service => _jobService.Value;

        public IEnumerable<ValidationResult> Validate()
        {
            if (JobFactory == null)
                yield return this.Failure("JobFactory", "must be specified");
            if (ManagementAddress == null)
                yield return this.Failure("ControlAddress", "must be a valid address");
            if (SuperviseInterval < TimeSpan.FromSeconds(1))
                yield return this.Failure("SuperviseInterval", "must be >= 1 second");
            if (PartitionCount < 1)
                yield return this.Failure("PartitionCount", "must be > 0");
        }

        public void Apply(IBusBuilder builder)
        {
        }

        public TimeSpan SuperviseInterval { private get; set; }

        public IJobFactory<TCommand> JobFactory { get; set; }

        void IPipeConfigurator<ConsumeContext>.AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _configurator.AddPipeSpecification(specification);
        }

        void IConsumePipeConfigurator.AddPipeSpecification<T1>(IPipeSpecification<ConsumeContext<T1>> specification)
        {
            _configurator.AddPipeSpecification(specification);
        }

        void IReceiveEndpointConfigurator.AddEndpointSpecification(IReceiveEndpointSpecification configurator)
        {
            _configurator.AddEndpointSpecification(configurator);
        }

        Uri IReceiveEndpointConfigurator.InputAddress => _configurator.InputAddress;

        public int PartitionCount { get; set; }

        void ISendPipelineConfigurator.ConfigureSend(Action<ISendPipeConfigurator> callback)
        {
            _configurator.ConfigureSend(callback);
        }

        void IPublishPipelineConfigurator.ConfigurePublish(Action<IPublishPipeConfigurator> callback)
        {
            _configurator.ConfigurePublish(callback);
        }

        IJobService CreateJobService()
        {
            return new JobService(_jobRegistry, _configurator.InputAddress, ManagementAddress, SuperviseInterval);
        }
    }
}