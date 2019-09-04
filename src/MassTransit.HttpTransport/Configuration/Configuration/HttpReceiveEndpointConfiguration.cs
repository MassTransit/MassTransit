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
namespace MassTransit.HttpTransport.Configuration
{
    using System;
    using Builders;
    using GreenPipes;
    using GreenPipes.Builders;
    using GreenPipes.Configurators;
    using MassTransit.Configuration;
    using Transport;


    public class HttpReceiveEndpointConfiguration :
        ReceiveEndpointConfiguration,
        IHttpReceiveEndpointConfiguration,
        IHttpReceiveEndpointConfigurator
    {
        readonly IHttpEndpointConfiguration _endpointConfiguration;
        readonly IHttpHostConfiguration _hostConfiguration;
        readonly IBuildPipeConfigurator<HttpHostContext> _httpHostPipeConfigurator;
        readonly string _pathMatch;

        public HttpReceiveEndpointConfiguration(IHttpHostConfiguration hostConfiguration, string pathMatch, IHttpEndpointConfiguration endpointConfiguration)
            : base(hostConfiguration, endpointConfiguration)
        {
            _hostConfiguration = hostConfiguration;
            _pathMatch = pathMatch;
            _endpointConfiguration = endpointConfiguration;

            HostAddress = hostConfiguration.HostAddress;
            InputAddress = new Uri(hostConfiguration.HostAddress, pathMatch);

            _httpHostPipeConfigurator = new PipeConfigurator<HttpHostContext>();
        }

        IHttpReceiveEndpointConfigurator IHttpReceiveEndpointConfiguration.Configurator => this;

        IHttpTopologyConfiguration IHttpEndpointConfiguration.Topology => _endpointConfiguration.Topology;

        public IHttpBusConfiguration BusConfiguration => _hostConfiguration.BusConfiguration;

        public override Uri HostAddress { get; }

        public override Uri InputAddress { get; }

        public override IReceiveEndpoint Build()
        {
            var builder = new HttpReceiveEndpointBuilder(this);

            ApplySpecifications(builder);

            var receiveEndpointContext = builder.CreateReceiveEndpointContext();

            var receiveSettings = new Settings(_pathMatch);

            var httpConsumerFilter = new HttpConsumerFilter(_hostConfiguration.Settings, receiveSettings, receiveEndpointContext);

            _httpHostPipeConfigurator.UseFilter(httpConsumerFilter);

            var transport = new HttpReceiveTransport(_hostConfiguration.Host, receiveEndpointContext, _httpHostPipeConfigurator.Build());

            transport.Add(httpConsumerFilter);

            return CreateReceiveEndpoint(string.IsNullOrWhiteSpace(_pathMatch) ? NewId.Next().ToString() : _pathMatch, transport, receiveEndpointContext);
        }


        class Settings :
            ReceiveSettings
        {
            public Settings(string pathMatch)
            {
                PathMatch = pathMatch;
            }

            public string PathMatch { get; }
        }
    }
}