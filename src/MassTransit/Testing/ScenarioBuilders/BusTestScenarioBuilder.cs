// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Testing.ScenarioBuilders
{
    using System;
    using BusConfigurators;
    using Scenarios;


    /// <summary>
    /// Implementation for the test scenario, but abstract for others to customize it. Sets some defaults in the c'tor, which you
    /// can override with the <see cref="ConfigureBus"/> and <see cref="ConfigureReceiveEndpoint"/> methods.
    /// </summary>
    public class BusTestScenarioBuilder :
        IBusTestScenarioBuilder
    {
        readonly InMemoryBusFactoryConfigurator _configurator;
        TimeSpan _timeout;

        /// <summary>
        /// c'tor
        /// </summary>
        public BusTestScenarioBuilder()
        {
            _timeout = TimeSpan.FromSeconds(30);
            _configurator = new InMemoryBusFactoryConfigurator();
        }

        public TimeSpan Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        public void ConfigureBus(Action<IInMemoryBusFactoryConfigurator> configureCallback)
        {
            configureCallback(_configurator);
        }

        public virtual IBusTestScenario Build()
        {
            var scenario = new BusTestScenario(Timeout, _configurator.Build());

            return scenario;
        }

        public void ConfigureReceiveEndpoint(Action<IReceiveEndpointConfigurator> configureCallback)
        {
            _configurator.ReceiveEndpoint("input_queue", configureCallback);
        }
    }
}