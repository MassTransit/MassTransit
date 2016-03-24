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
namespace MassTransit.HttpTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using BusConfigurators;
    using Configurators;
    using Hosting;
    using MassTransit.Builders;


    public class HttpBusFactoryConfigurator :
        BusFactoryConfigurator,
        IHttpBusFactoryConfigurator,
        IBusFactory
    {
        readonly IList<HttpHost> _hosts;
        readonly IList<IBusFactorySpecification> _transportBuilderConfigurators;

        public HttpBusFactoryConfigurator()
        {
            _hosts = new List<HttpHost>();
            _transportBuilderConfigurators = new List<IBusFactorySpecification>();
        }

        public IBusControl CreateBus()
        {
            var builder = new HttpBusBuilder(_hosts.ToArray(), ConsumePipeFactory, SendPipeFactory, PublishPipeFactory);

            foreach (var configurator in _transportBuilderConfigurators)
                configurator.Apply(builder);

            var bus = builder.Build();

            return bus;
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (var result in base.Validate())
                yield return result;

            if (_hosts.Count == 0)
                yield return this.Failure("Host", "At least one host must be defined");

            foreach (var result in _transportBuilderConfigurators.SelectMany(x => x.Validate()))
                yield return result;
        }

        public IHttpHost Host(HttpHostSettings settings)
        {
            var httpHost = new HttpHost(settings);

            _hosts.Add(httpHost);

            return httpHost;
        }

        public void ReceiveEndpoint(Action<IHttpReceiveEndpointConfigurator> configure = null)
        {
            ReceiveEndpoint(_hosts[0], configure);
        }

        public void ReceiveEndpoint(IHttpHost host, Action<IHttpReceiveEndpointConfigurator> configure = null)
        {
            if (host == null)
                throw new EndpointNotFoundException("The host address specified was not configured.");

            var ep = new HttpReceiveEndpointConfigurator(host);

            if (configure != null)
                configure(ep);

            AddBusFactorySpecification(ep);
        }

        public void AddBusFactorySpecification(IBusFactorySpecification configurator)
        {
            _transportBuilderConfigurators.Add(configurator);
        }
    }
}