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
namespace MassTransit
{
    using System;
    using PipeConfigurators;


    /// <summary>
    /// This is simply a delegate to the endpoint configurator for a management endpoint.
    /// A management endpoint is just a type of recieve endpoint that can be used to communicate
    /// with middleware, etc.
    /// </summary>
    public class ManagementEndpointConfigurator :
        IManagementEndpointConfigurator
    {
        readonly IReceiveEndpointConfigurator _configurator;

        public ManagementEndpointConfigurator(IReceiveEndpointConfigurator configurator)
        {
            _configurator = configurator;
        }

        void IPipeConfigurator<ConsumeContext>.AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _configurator.AddPipeSpecification(specification);
        }

        void IConsumePipeConfigurator.AddPipeSpecification<T>(IPipeSpecification<ConsumeContext<T>> specification)
        {
            _configurator.AddPipeSpecification(specification);
        }

        void IReceiveEndpointConfigurator.AddEndpointSpecification(IReceiveEndpointSpecification configurator)
        {
            _configurator.AddEndpointSpecification(configurator);
        }

        Uri IReceiveEndpointConfigurator.InputAddress => _configurator.InputAddress;
    }
}