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
namespace MassTransit.HttpTransport.Specifications
{
    using EndpointSpecifications;
    using MassTransit.Pipeline;


    public class HttpEndpointConfiguration :
        EndpointConfiguration,
        IHttpEndpointConfiguration
    {
        readonly IHttpTopologyConfiguration _topologyConfiguration;

        public HttpEndpointConfiguration(IHttpTopologyConfiguration topologyConfiguration, IConsumePipe consumePipe = null)
            : base(topologyConfiguration, consumePipe)
        {
            _topologyConfiguration = topologyConfiguration;
        }

        HttpEndpointConfiguration(IHttpEndpointConfiguration parentConfiguration, IHttpTopologyConfiguration topologyConfiguration,
            IConsumePipe consumePipe = null)
            : base(parentConfiguration, topologyConfiguration, consumePipe)
        {
            _topologyConfiguration = topologyConfiguration;
        }

        IHttpTopologyConfiguration IHttpEndpointConfiguration.Topology => _topologyConfiguration;

        public IHttpEndpointConfiguration CreateNewConfiguration(IConsumePipe consumePipe = null)
        {
            var topologyConfiguration = new HttpTopologyConfiguration(_topologyConfiguration);

            return new HttpEndpointConfiguration(this, topologyConfiguration, consumePipe);
        }
    }
}