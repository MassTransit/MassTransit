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
namespace MassTransit.HttpTransport.Builders
{
    using System;
    using MassTransit.Builders;
    using Specifications;
    using Topology;
    using Transport;
    using Transports;


    public class HttpReceiveEndpointBuilder :
        ReceiveEndpointBuilder,
        IHttpReceiveEndpointBuilder
    {
        readonly IHttpEndpointConfiguration _configuration;
        readonly IHttpHost _host;
        readonly BusHostCollection<HttpHost> _hosts;

        public HttpReceiveEndpointBuilder(IBusBuilder busBuilder, IHttpHost host, BusHostCollection<HttpHost> hosts, IHttpEndpointConfiguration configuration)
            : base(busBuilder, configuration)
        {
            _host = host;
            _configuration = configuration;
            _hosts = hosts;
        }

        public IHttpReceiveEndpointTopology CreateReceiveEndpointTopology(Uri inputAddress)
        {
            return new HttpReceiveEndpointTopology(_configuration, inputAddress, MessageSerializer, _host, _hosts);
        }
    }
}