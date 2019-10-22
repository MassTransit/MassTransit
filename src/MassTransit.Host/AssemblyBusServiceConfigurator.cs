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
namespace MassTransit.Host
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autofac;
    using Context;
    using Hosting;
    using Internals.Extensions;
    using Metadata;
    using Util;


    /// <summary>
    /// Used to configure a bus for an assembly-based registration using the hosting extensions.
    /// </summary>
    public class AssemblyBusServiceConfigurator :
        IBusServiceConfigurator
    {
        readonly IEndpointSpecification[] _endpointSpecifications;
        readonly IServiceSpecification _serviceSpecification;
        readonly ISettingsProvider _settingsProvider;
        readonly ILifetimeScope _lifetimeScope;

        public AssemblyBusServiceConfigurator(IEnumerable<IEndpointSpecification> endpointSpecifications, IServiceSpecification serviceSpecification,
            ISettingsProvider settingsProvider, ILifetimeScope lifetimeScope)
        {
            _serviceSpecification = serviceSpecification;
            _settingsProvider = settingsProvider;
            _lifetimeScope = lifetimeScope;
            _endpointSpecifications = endpointSpecifications.ToArray();
        }

        void IBusServiceConfigurator.Configure(IServiceConfigurator configurator)
        {
            LogContext.Info?.Log("Configuring Service: {ServiceName}", TypeMetadataCache.GetShortName(_serviceSpecification.GetType()));

            _serviceSpecification.Configure(configurator);

            foreach (var specification in _endpointSpecifications)
            {
                string queueName;
                int consumerLimit;
                GetEndpointSettings(specification, out queueName, out consumerLimit);

                LogContext.Info?.Log("Configuring Endpoint: {EndpointName} (queue-name: {Queue}, consumer-limit: {ConsumerLimit})",
                    TypeMetadataCache.GetShortName(_serviceSpecification.GetType()), queueName, consumerLimit);

                configurator.ReceiveEndpoint(queueName, consumerLimit, x =>
                {
                    specification.Configure(x);

                    LogContext.Info?.Log("Configured Endpoint: {EndpointName} (address: {InputAddress})",
                        TypeMetadataCache.GetShortName(_serviceSpecification.GetType()), x.InputAddress);
                });
            }

            ConfigureBusObservers(configurator);
        }

        void ConfigureBusObservers(IServiceConfigurator configurator)
        {
            var observers = _lifetimeScope.ResolveOptional<IEnumerable<IBusObserver>>();
            if (observers != null)
            {
                foreach (var observer in observers)
                {
                    configurator.BusObserver(observer);
                }
            }
        }

        void GetEndpointSettings(IEndpointSpecification specification, out string queueName, out int consumerLimit)
        {
            var prefix = GetSettingsPrefix(specification.GetType());

            queueName = specification.QueueName;
            consumerLimit = specification.ConsumerLimit;

            EndpointSettings endpointSettings;
            if (_settingsProvider.TryGetSettings(prefix, out endpointSettings))
            {
                if (!string.IsNullOrWhiteSpace(endpointSettings.QueueName))
                    queueName = endpointSettings.QueueName;

                if (endpointSettings.ConsumerLimit.HasValue)
                    consumerLimit = endpointSettings.ConsumerLimit.Value;
            }
        }

        string GetSettingsPrefix(Type specificationType)
        {
            string name = specificationType.Name;
            if (name.EndsWith("Specification", StringComparison.InvariantCultureIgnoreCase))
                name = name.Substring(0, name.Length - "Specification".Length);
            if (name.EndsWith("Endpoint", StringComparison.InvariantCultureIgnoreCase))
                name = name.Substring(0, name.Length - "Endpoint".Length);
            return name;
        }
    }
}
