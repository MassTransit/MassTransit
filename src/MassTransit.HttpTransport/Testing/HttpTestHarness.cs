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
namespace MassTransit.HttpTransport.Testing
{
    using System;
    using System.Net.Http;
    using Logging;
    using MassTransit.Testing;


    public class HttpTestHarness :
        BusTestHarness
    {
        static readonly ILog _log = Logger.Get<HttpTestHarness>();

        Uri _inputQueueAddress;

        public HttpTestHarness(Uri hostAddress = null, Uri inputQueueAddress = null)
        {
            if (hostAddress == null)
                throw new ArgumentNullException(nameof(hostAddress));

            HostAddress = hostAddress ?? new Uri("http://localhost:8080");

            _inputQueueAddress = inputQueueAddress ?? HostAddress;
        }

        public Uri HostAddress { get; }

        public IHttpHost Host { get; private set; }

        public override Uri InputQueueAddress => _inputQueueAddress;

        public event Action<IHttpBusFactoryConfigurator> OnConfigureBus;
        public event Action<IHttpBusFactoryConfigurator, IHttpHost> OnConfigureBusHost;
        public event Action<IHttpReceiveEndpointConfigurator> OnConfigureRootReceiveEndpoint;

        protected virtual void ConfigureBus(IHttpBusFactoryConfigurator configurator)
        {
            OnConfigureBus?.Invoke(configurator);
        }

        protected virtual void ConfigureBusHost(IHttpBusFactoryConfigurator configurator, IHttpHost host)
        {
            OnConfigureBusHost?.Invoke(configurator, host);
        }

        protected virtual void ConfigureRootReceiveEndpoint(IHttpReceiveEndpointConfigurator configurator)
        {
            OnConfigureRootReceiveEndpoint?.Invoke(configurator);
        }

        protected override IBusControl CreateBus()
        {
            return MassTransit.Bus.Factory.CreateUsingHttp(x =>
            {
                ConfigureBus(x);

                Host = x.Host(HostAddress, h => h.Method = HttpMethod.Post);

                ConfigureBusHost(x, Host);

                x.ReceiveEndpoint(Host, "", e =>
                {
                    ConfigureRootReceiveEndpoint(e);

                    _inputQueueAddress = e.InputAddress;
                });
            });
        }
    }
}