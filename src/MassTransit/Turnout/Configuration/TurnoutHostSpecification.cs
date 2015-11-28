// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Configurators;
    using PipeConfigurators;


    public class TurnoutHostSpecification<T> :
        IBusFactorySpecification,
        ITurnoutHostConfigurator<T>
        where T : class
    {
        readonly IReceiveEndpointConfigurator _configurator;
        readonly Lazy<ITurnoutController> _controller;
        readonly IJobRoster _jobRoster;
        Uri _controlAddress;
        IJobFactory<T> _jobFactory;
        TimeSpan _superviseInterval;

        public TurnoutHostSpecification(IReceiveEndpointConfigurator configurator)
        {
            _configurator = configurator;
            _superviseInterval = TimeSpan.FromMinutes(1);
            _jobRoster = new JobRoster();

            _controller = new Lazy<ITurnoutController>(CreateController);
        }

        public IJobRoster JobRoster => _jobRoster;

        public Uri ControlAddress
        {
            set { _controlAddress = value; }
        }

        public ITurnoutController Controller => _controller.Value;

        public IEnumerable<ValidationResult> Validate()
        {
            if (_jobFactory == null)
                yield return this.Failure("JobFactory", "must be specified");
            if (_controlAddress == null)
                yield return this.Failure("ControlAddress", "must be a valid address");
            if (_superviseInterval < TimeSpan.FromSeconds(1))
                yield return this.Failure("SuperviseInterval", "must be >= 1 second");
        }

        public void Apply(IBusBuilder builder)
        {
        }

        public TimeSpan SuperviseInterval
        {
            get { return _superviseInterval; }
            set { _superviseInterval = value; }
        }

        public IJobFactory<T> JobFactory
        {
            get { return _jobFactory; }
            set { _jobFactory = value; }
        }

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

        void ISendPipelineConfigurator.ConfigureSend(Action<ISendPipeConfigurator> callback)
        {
            _configurator.ConfigureSend(callback);
        }

        ITurnoutController CreateController()
        {
            return new TurnoutController(_jobRoster, _controlAddress, _superviseInterval);
        }
    }
}