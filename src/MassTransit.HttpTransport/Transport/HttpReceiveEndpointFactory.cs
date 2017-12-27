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
namespace MassTransit.HttpTransport.Transport
{
    using System;
    using Builders;
    using Configurators;
    using Specifications;
    using Transports;


    public class HttpReceiveEndpointFactory :
        IHttpReceiveEndpointFactory
    {
        readonly HttpBusBuilder _builder;
        readonly IHttpEndpointConfiguration _configuration;
        readonly IHttpHost _host;
        readonly BusHostCollection<HttpHost> _hosts;

        public HttpReceiveEndpointFactory(HttpBusBuilder builder, IHttpHost host, BusHostCollection<HttpHost> hosts, IHttpEndpointConfiguration configuration)
        {
            _builder = builder;
            _host = host;
            _hosts = hosts;
            _configuration = configuration;
        }

        public void CreateReceiveEndpoint(string pathMatch, Action<IHttpReceiveEndpointConfigurator> configure)
        {
            var endpointConfigurator = new HttpReceiveEndpointSpecification(_host, _hosts, pathMatch, _configuration);

            configure?.Invoke(endpointConfigurator);

            BusConfigurationResult.CompileResults(endpointConfigurator.Validate());

            endpointConfigurator.Apply(_builder);
        }
    }
}