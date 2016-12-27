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
namespace MassTransit.Testing
{
    using System;
    using Logging;
    using Pipeline.Pipes;
    using Transports.InMemory;


    public class InMemoryTestHarness :
        BusTestHarness
    {
        static readonly ILog _log = Logger.Get<InMemoryTestHarness>();

        InMemoryHost _inMemoryHost;

        public InMemoryTestHarness()
        {
            BaseAddress = new Uri("loopback://localhost/");

            InputQueueName = "input_queue";
            InputQueueAddress = new Uri($"loopback://localhost/{InputQueueName}");
        }

        public string InputQueueName { get; }
        public Uri BaseAddress { get; }
        public IInMemoryHost Host => _inMemoryHost;

        public override Uri InputQueueAddress { get; }

        public IPublishEndpointProvider PublishEndpointProvider => new InMemoryPublishEndpointProvider(Bus, _inMemoryHost, PublishPipe.Empty);

        public IInMemoryTransport GetTransport(string queueName)
        {
            return _inMemoryHost.GetTransport(queueName);
        }

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

        protected override IBusControl CreateBus()
        {
            return MassTransit.Bus.Factory.CreateUsingInMemory(x =>
            {
                _inMemoryHost = new InMemoryHost(Environment.ProcessorCount);

                x.SetHost(_inMemoryHost);

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