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
    using Configuration;
    using Configurators;


    public class HttpReceiveEndpointFactory :
        IHttpReceiveEndpointFactory
    {
        readonly IHttpBusConfiguration _configuration;
        readonly IHttpHost _host;

        public HttpReceiveEndpointFactory(IHttpBusConfiguration configuration, IHttpHost host)
        {
            _host = host;
            _configuration = configuration;
        }

        public void CreateReceiveEndpoint(string pathMatch, Action<IHttpReceiveEndpointConfigurator> configure)
        {
            if (!_configuration.TryGetHost(_host, out var hostConfiguration))
                throw new ConfigurationException("The host was not properly configured");

            var configuration = hostConfiguration.CreateReceiveEndpointConfiguration(pathMatch);

            configure?.Invoke(configuration.Configurator);

            BusConfigurationResult.CompileResults(configuration.Validate());

            configuration.Build();
        }
    }
}