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
    using MassTransit.Testing;


    public class HttpTestHarness :
        BusTestHarness
    {
        Uri _inputQueueAddress;

        public HttpTestHarness(Uri hostAddress = null, Uri inputQueueAddress = null)
        {
            HostAddress = hostAddress ?? new Uri("http://localhost:8080");

            _inputQueueAddress = inputQueueAddress ?? HostAddress;

            InputQueueName = _inputQueueAddress.AbsolutePath.Trim('/');
        }

        public Uri HostAddress { get; }

        public IHttpHost Host { get; private set; }

        public override string InputQueueName { get; }
        public override Uri InputQueueAddress => _inputQueueAddress;

        public event Action<IHttpBusFactoryConfigurator> OnConfigureHttpBus;
        public event Action<IHttpBusFactoryConfigurator, IHttpHost> OnConfigureHttpBusHost;
        public event Action<IHttpReceiveEndpointConfigurator> OnConfigureHttpReceiveEndpoint;

        protected virtual void ConfigureHttpBus(IHttpBusFactoryConfigurator configurator)
        {
            OnConfigureHttpBus?.Invoke(configurator);
        }

        protected virtual void ConfigureHttpBusHost(IHttpBusFactoryConfigurator configurator, IHttpHost host)
        {
            OnConfigureHttpBusHost?.Invoke(configurator, host);
        }

        protected virtual void ConfigureHttpReceiveEndpoint(IHttpReceiveEndpointConfigurator configurator)
        {
            OnConfigureHttpReceiveEndpoint?.Invoke(configurator);
        }

        protected override IBusControl CreateBus()
        {
            return MassTransit.Bus.Factory.CreateUsingHttp(x =>
            {
                ConfigureBus(x);

                ConfigureHttpBus(x);

                Host = x.Host(HostAddress, h => h.Method = HttpMethod.Post);

                ConfigureHttpBusHost(x, Host);

                x.ReceiveEndpoint(Host, "", e =>
                {
                    ConfigureReceiveEndpoint(e);

                    ConfigureHttpReceiveEndpoint(e);

                    _inputQueueAddress = e.InputAddress;
                });
            });
        }
    }
}
