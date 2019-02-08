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
namespace MassTransit.Configuration
{
    using Pipeline;
    using Pipeline.Pipes;
    using SendPipeSpecifications;
    using Topology;
    using Topology.Observers;


    public class SendPipeConfiguration :
        ISendPipeConfiguration
    {
        readonly SendPipeSpecification _specification;

        public SendPipeConfiguration(ISendTopology sendTopology)
        {
            _specification = new SendPipeSpecification();
            _specification.ConnectSendPipeSpecificationObserver(new TopologySendPipeSpecificationObserver(sendTopology));
        }

        public SendPipeConfiguration(ISendPipeSpecification parentSpecification)
        {
            _specification = new SendPipeSpecification();
            _specification.ConnectSendPipeSpecificationObserver(new ParentSendPipeSpecificationObserver(parentSpecification));
        }

        public ISendPipeSpecification Specification => _specification;
        public ISendPipeConfigurator Configurator => _specification;

        public ISendPipe CreatePipe()
        {
            return new SendPipe(_specification);
        }
    }
}