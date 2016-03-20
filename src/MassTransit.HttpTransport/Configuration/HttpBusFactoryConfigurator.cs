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
    using HttpTransport;
    using MassTransit.Builders;


    public class HttpBusFactoryConfigurator :
        BusFactoryConfigurator,
        IHttpBusFactoryConfigurator,
        IBusFactory
    {
        readonly IList<HttpHost> _hosts;
        readonly IList<IBusFactorySpecification> _transportBuilderConfigurators;
        readonly HttpReceiveSettings _receiveSettings;

        public HttpBusFactoryConfigurator()
        {
            _hosts = new List<HttpHost>();
            _transportBuilderConfigurators = new List<IBusFactorySpecification>();

            _receiveSettings = new HttpReceiveSettings("localhost", 8080, "bus-" + Guid.NewGuid());
        }

        public IBusControl CreateBus()
        {
            var builder = new HttpBusBuilder(_hosts.ToArray(), ConsumePipeFactory, SendPipeFactory, PublishPipeFactory, _receiveSettings);

            foreach (var configurator in _transportBuilderConfigurators)
                configurator.Apply(builder);

            var bus = builder.Build();

            return bus;
        }

        public IHttpHost Host(HttpHostSettings settings)
        {
            var httpHost = new HttpHost(settings);

            _hosts.Add(httpHost);

            return httpHost;
        }

        public void ReceiveEndpoint(string path, Action<IHttpReceiveEndpointConfigurator> configure = null)
        {
            ReceiveEndpoint(_hosts[0], path, configure);
        }

        public void ReceiveEndpoint(IHttpHost host, string path, Action<IHttpReceiveEndpointConfigurator> configure = null)
        {
            if (host == null)
                throw new EndpointNotFoundException("The host address specified was not configured.");

            var ep = new HttpReceiveEndpointConfigurator(host, new HttpReceiveSettings(host.Settings.Host, host.Settings.Port, path));

            if(configure != null)
                configure(ep);

            AddBusFactorySpecification(ep);
        }


        public void AddBusFactorySpecification(IBusFactorySpecification configurator)
        {
            _transportBuilderConfigurators.Add(configurator);
        }

        public void OverrideDefaultBusEndpointPath(string value)
        {
            _receiveSettings.Path = value;
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (var result in base.Validate())
                yield return result;

            if (_hosts.Count == 0)
                yield return this.Failure("Host", "At least one host must be defined");
            if (string.IsNullOrWhiteSpace(_receiveSettings.Path))
                yield return this.Failure("Bus", "The bus path must not be null or empty");

            foreach (var result in _transportBuilderConfigurators.SelectMany(x => x.Validate()))
                yield return result;
        }
    }
}