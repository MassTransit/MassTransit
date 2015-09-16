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
    using Hosting;
    using Internals.Extensions;
    using Logging;


    /// <summary>
    /// Used to configure a bus for an assembly-based registration using the hosting extensions.
    /// </summary>
    public class AssemblyBusServiceConfigurator :
        IBusServiceConfigurator
    {
        readonly IEndpointSpecification[] _endpointSpecifications;
        readonly ILog _log = Logger.Get<AssemblyBusServiceConfigurator>();
        readonly IServiceSpecification _serviceSpecification;
        readonly ISettingsProvider _settingsProvider;

        public AssemblyBusServiceConfigurator(IEnumerable<IEndpointSpecification> endpointSpecifications, IServiceSpecification serviceSpecification,
            ISettingsProvider settingsProvider)
        {
            _serviceSpecification = serviceSpecification;
            _settingsProvider = settingsProvider;
            _endpointSpecifications = endpointSpecifications.ToArray();
        }

        void IBusServiceConfigurator.Configure(IServiceConfigurator configurator)
        {
            _log.Info($"Configuring Service: {_serviceSpecification.GetType().GetTypeName()}");

            _serviceSpecification.Configure(configurator);

            foreach (var specification in _endpointSpecifications)
            {
                string queueName;
                int consumerLimit;
                GetEndpointSettings(specification, out queueName, out consumerLimit);

                _log.Info($"Configuring Endpoint: {specification.GetType().GetTypeName()} (queue-name: {queueName}, consumer-limit: {consumerLimit})");

                configurator.ReceiveEndpoint(queueName, consumerLimit, x =>
                {
                    specification.Configure(x);
                });
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