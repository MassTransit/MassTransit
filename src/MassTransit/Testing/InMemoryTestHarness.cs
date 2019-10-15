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
namespace MassTransit.Testing
{
    using System;
    using System.Threading.Tasks;
    using Transports.InMemory;


    public class InMemoryTestHarness :
        BusTestHarness
    {
        IInMemoryHost _inMemoryHost;
        readonly string _inputQueueName;

        public InMemoryTestHarness(string virtualHost = null)
        {
            BaseAddress = new Uri("loopback://localhost/");
            if (!string.IsNullOrWhiteSpace(virtualHost))
                BaseAddress = new Uri(BaseAddress, virtualHost.Trim('/') + '/');

            _inputQueueName = "input_queue";
            InputQueueAddress = new Uri(BaseAddress, _inputQueueName);
        }

        public Uri BaseAddress { get; }
        public IInMemoryHost Host => _inMemoryHost;

        public override Uri InputQueueAddress { get; }
        public override string InputQueueName => _inputQueueName;

        public event Action<IInMemoryBusFactoryConfigurator> OnConfigureInMemoryBus;
        public event Action<IInMemoryReceiveEndpointConfigurator> OnConfigureInMemoryReceiveEndpoint;

        protected virtual void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            OnConfigureInMemoryBus?.Invoke(configurator);
        }

        protected virtual void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            OnConfigureInMemoryReceiveEndpoint?.Invoke(configurator);
        }

        public virtual Task<IRequestClient<TRequest>> ConnectRequestClient<TRequest>()
            where TRequest : class
        {
            return ConnectRequestClient<TRequest>(InputQueueAddress);
        }

        public virtual Task<IRequestClient<TRequest>> ConnectRequestClient<TRequest>(Uri destinationAddress)
            where TRequest : class
        {
            return _inMemoryHost.CreateRequestClient<TRequest>(destinationAddress, TestTimeout);
        }

        protected override IBusControl CreateBus()
        {
            return MassTransit.Bus.Factory.CreateUsingInMemory(BaseAddress, x =>
            {
                ConfigureBus(x);

                _inMemoryHost = x.Host(BaseAddress);

                ConfigureInMemoryBus(x);

                x.ReceiveEndpoint(InputQueueName, configurator =>
                {
                    ConfigureReceiveEndpoint(configurator);

                    ConfigureInMemoryReceiveEndpoint(configurator);
                });
            });
        }
    }
}
