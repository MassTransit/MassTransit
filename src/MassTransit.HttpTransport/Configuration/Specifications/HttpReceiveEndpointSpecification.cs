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
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using Builders;
    using Clients;
    using EndpointSpecifications;
    using GreenPipes;
    using MassTransit.Builders;
    using Transport;
    using Transports;
    using Transports.InMemory;


    public class HttpReceiveEndpointSpecification :
        ReceiveEndpointSpecification,
        IHttpReceiveEndpointConfigurator,
        IBusFactorySpecification
    {
        readonly IHttpHost _host;
        readonly string _pathMatch;
        readonly IHttpEndpointConfiguration _configuration;
        IPublishEndpointProvider _publishEndpointProvider;
        ISendEndpointProvider _sendEndpointProvider;
        readonly BusHostCollection<HttpHost> _hosts;

        public HttpReceiveEndpointSpecification(IHttpHost host, BusHostCollection<HttpHost> hosts, string pathMatch, IHttpEndpointConfiguration configuration)
            : base(configuration)
        {
            _host = host;
            _hosts = hosts;
            _pathMatch = pathMatch;
            _configuration = configuration;
        }

        public ISendEndpointProvider SendEndpointProvider => _sendEndpointProvider;
        public IPublishEndpointProvider PublishEndpointProvider => _publishEndpointProvider;

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (var result in base.Validate())
                yield return result.WithParentKey($"{_host.Settings.ToDebugString()}");

            if (_pathMatch.EndsWith("/"))
                yield return this.Failure("PathMatch", "Must not end with a /");
        }

        public void Apply(IBusBuilder builder)
        {
            var receiveEndpointBuilder = new HttpReceiveEndpointBuilder(builder, _host, _hosts, _configuration);

            var receiveEndpointTopology = receiveEndpointBuilder.CreateReceiveEndpointTopology(InputAddress);

            var receivePipe = CreateReceivePipe(receiveEndpointBuilder);

            _sendEndpointProvider = receiveEndpointTopology.SendEndpointProvider;
            _publishEndpointProvider = receiveEndpointTopology.PublishEndpointProvider;

            var receiveSettings = new Settings(_pathMatch);

            var transport = new HttpReceiveTransport(_host, receiveSettings, receiveEndpointTopology);

            var httpHost = _host as HttpHost;
            if (httpHost == null)
                throw new ConfigurationException("Must be a HttpHost");

            httpHost.ReceiveEndpoints.Add(NewId.Next().ToString(), new ReceiveEndpoint(transport, receivePipe));
        }

        protected override Uri GetInputAddress()
        {
            return _host.Settings.GetInputAddress();
        }

        protected override Uri GetErrorAddress()
        {
            var errorQueueName = "bus_error";
            var sendSettings = new HttpSendSettingsImpl(HttpMethod.Get, errorQueueName);

            return _host.Settings.GetSendAddress(sendSettings);
        }

        protected override Uri GetDeadLetterAddress()
        {
            var deadLetterQueueName = "bus_skipped";
            var sendSettings = new HttpSendSettingsImpl(HttpMethod.Delete, deadLetterQueueName);

            return _host.Settings.GetSendAddress(sendSettings);
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