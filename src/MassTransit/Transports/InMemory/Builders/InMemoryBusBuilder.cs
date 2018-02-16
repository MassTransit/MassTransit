// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports.InMemory.Builders
{
    using Configuration;
    using MassTransit.Builders;
    using EndpointSpecifications;
    using Pipeline.Observables;


    public class InMemoryBusBuilder :
        BusBuilder
    {
        readonly ConfigurationReceiveEndpointSpecification _busEndpointSpecification;

        public InMemoryBusBuilder(IInMemoryBusConfiguration configuration, IInMemoryReceiveEndpointConfiguration busEndpointConfiguration,
            BusObservable busObservable)
            : base(configuration, busEndpointConfiguration, busObservable)
        {
            _busEndpointSpecification = new ConfigurationReceiveEndpointSpecification(busEndpointConfiguration);
        }

        protected override IPublishEndpointProvider PublishEndpointProvider => _busEndpointSpecification.PublishEndpointProvider;

        protected override ISendEndpointProvider SendEndpointProvider => _busEndpointSpecification.SendEndpointProvider;

        protected override void PreBuild()
        {
            _busEndpointSpecification.Apply(this);
        }
    }
}