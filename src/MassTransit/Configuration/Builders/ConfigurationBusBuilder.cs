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
namespace MassTransit.Builders
{
    using System;
    using Configuration;
    using EndpointSpecifications;
    using Pipeline.Observables;
    using Util;


    public class ConfigurationBusBuilder :
        IBusBuilder
    {
        readonly IReceiveEndpointConfiguration _busEndpointConfiguration;
        readonly ConfigurationReceiveEndpointSpecification _busEndpointSpecification;
        readonly BusObservable _busObservable;
        readonly IBusConfiguration _configuration;

        public ConfigurationBusBuilder(IBusConfiguration configuration, IReceiveEndpointConfiguration busReceiveEndpointConfiguration,
            BusObservable busObservable)
        {
            _busEndpointSpecification = new ConfigurationReceiveEndpointSpecification(busReceiveEndpointConfiguration);
            _configuration = configuration;
            _busEndpointConfiguration = busReceiveEndpointConfiguration;

            _busObservable = busObservable;
        }

        public IBusControl Build()
        {
            try
            {
                _busEndpointSpecification.Apply(this);

                var bus = new MassTransitBus(_busEndpointConfiguration.InputAddress, _busEndpointConfiguration.ConsumePipe, _busEndpointSpecification.SendEndpointProvider,
                    _busEndpointSpecification.PublishEndpointProvider, _configuration.Hosts, _busObservable);

                TaskUtil.Await(() => _busObservable.PostCreate(bus));

                return bus;
            }
            catch (Exception exception)
            {
                TaskUtil.Await(() => _busObservable.CreateFaulted(exception));

                throw;
            }
        }
    }
}