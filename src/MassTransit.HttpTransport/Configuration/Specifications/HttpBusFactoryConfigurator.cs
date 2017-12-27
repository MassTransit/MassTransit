// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Builders;
    using BusConfigurators;
    using GreenPipes;
    using Hosting;
    using MassTransit.Builders;
    using Topology;
    using Transport;
    using Transports;


    public class HttpBusFactoryConfigurator :
        BusFactoryConfigurator<IBusBuilder>,
        IHttpBusFactoryConfigurator,
        IBusFactory
    {
        readonly IHttpEndpointConfiguration _configuration;
        readonly BusHostCollection<HttpHost> _hosts;

        public HttpBusFactoryConfigurator(IHttpEndpointConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration;
            _hosts = new BusHostCollection<HttpHost>();
        }

        public IBusControl CreateBus()
        {
            var builder = new HttpBusBuilder(_hosts, _configuration);

            ApplySpecifications(builder);

            var bus = builder.Build();

            return bus;
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (var result in base.Validate())
                yield return result;

            if (_hosts.Count == 0)
                yield return this.Failure("Host", "At least one host must be defined");
        }

        public IHttpHost Host(HttpHostSettings settings)
        {
            var hostTopology = new HttpHostTopology(_configuration.Topology);
            
            var httpHost = new HttpHost(settings, hostTopology);

            _hosts.Add(httpHost);

            return httpHost;
        }

        public void ReceiveEndpoint(Action<IHttpReceiveEndpointConfigurator> configure = null)
        {
            ReceiveEndpoint(_hosts[0], "", configure);
        }

        public void ReceiveEndpoint(IHttpHost host, string pathMatch, Action<IHttpReceiveEndpointConfigurator> configure = null)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host));

            var endpointSpecification = _configuration.CreateNewConfiguration();

            var specification = new HttpReceiveEndpointSpecification(host, _hosts, pathMatch, endpointSpecification);

            specification.ConnectConsumerConfigurationObserver(this);
            specification.ConnectSagaConfigurationObserver(this);

            configure?.Invoke(specification);

            AddBusFactorySpecification(specification);
        }

        public void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            // TODO: need to come up with a way this makes sense - virtual directory perhaps?
            throw new NotImplementedException();
        }
    }
}