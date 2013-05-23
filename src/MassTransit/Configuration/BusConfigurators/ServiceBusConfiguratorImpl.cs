// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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

using MassTransit.Util;

namespace MassTransit.BusConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using Builders;
    using Configuration;
    using Configurators;
    using EndpointConfigurators;
    using Logging;
    using Magnum.Extensions;
    using SubscriptionConfigurators;
    using Transports;

    /// <summary>
    /// <see cref="ServiceBusConfigurator"/>. Core implementation of service bus
    /// configurator.
    /// </summary>
    public class ServiceBusConfiguratorImpl :
        ServiceBusConfigurator
    {
        static readonly ILog _log = Logger.Get(typeof (ServiceBusConfiguratorImpl));

        readonly IList<BusBuilderConfigurator> _configurators;
        readonly EndpointFactoryConfigurator _endpointFactoryConfigurator;
        readonly ServiceBusSettings _settings;

        readonly SubscriptionRouterConfiguratorImpl _subscriptionRouterConfigurator;
        Func<BusSettings, BusBuilder> _builderFactory;

        public ServiceBusConfiguratorImpl(ServiceBusDefaultSettings defaultSettings)
        {
            _settings = new ServiceBusSettings(defaultSettings);

            _builderFactory = DefaultBuilderFactory;
            _configurators = new List<BusBuilderConfigurator>();

            _endpointFactoryConfigurator = new EndpointFactoryConfiguratorImpl(new EndpointFactoryDefaultSettings());

            _subscriptionRouterConfigurator = new SubscriptionRouterConfiguratorImpl(_settings.Network);
            _configurators.Add(_subscriptionRouterConfigurator);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_builderFactory == null)
                yield return this.Failure("BuilderFactory", "The builder factory cannot be null.");

            if (_settings.InputAddress == null)
            {
                string msg =
                    "The 'InputAddress' is null. #sadpanda I was expecting an address to be set like 'msmq://localhost/queue'";
                msg += "or 'rabbitmq://localhost/queue'. The InputAddress is a 'Uri' by the way.";

                yield return this.Failure("InputAddress", msg);
            }

            foreach (ValidationResult result in _endpointFactoryConfigurator.Validate())
                yield return result.WithParentKey("EndpointFactory");

            foreach (ValidationResult result in _configurators.SelectMany(configurator => configurator.Validate()))
                yield return result;

            foreach (ValidationResult result in _subscriptionRouterConfigurator.Validate())
                yield return result;
        }

        public void UseBusBuilder(Func<BusSettings, BusBuilder> builderFactory)
        {
            _builderFactory = builderFactory;
        }

        public void AddSubscriptionRouterConfigurator(SubscriptionRouterBuilderConfigurator configurator)
        {
            _subscriptionRouterConfigurator.AddConfigurator(configurator);
        }

        public void AddBusConfigurator(BusBuilderConfigurator configurator)
        {
            _configurators.Add(configurator);
        }

        public void ReceiveFrom(Uri uri)
        {
            _settings.InputAddress = uri;
        }

        public void SetNetwork(string network)
        {
            _settings.Network = network.IsEmpty() ? null : network;
        }

        public void DisablePerformanceCounters()
        {
            _settings.EnablePerformanceCounters = false;
        }

        public void BeforeConsumingMessage(Action beforeConsume)
        {
            if (_settings.BeforeConsume == null)
                _settings.BeforeConsume = beforeConsume;
            else
                _settings.BeforeConsume += beforeConsume;
        }

        public void AfterConsumingMessage(Action afterConsume)
        {
            if (_settings.AfterConsume == null)
                _settings.AfterConsume = afterConsume;
            else
                _settings.AfterConsume += afterConsume;
        }

        public void UseEndpointFactoryBuilder(
            Func<IEndpointFactoryDefaultSettings, EndpointFactoryBuilder> endpointFactoryBuilderFactory)
        {
            _endpointFactoryConfigurator.UseEndpointFactoryBuilder(endpointFactoryBuilderFactory);
        }

        public void AddEndpointFactoryConfigurator(EndpointFactoryBuilderConfigurator configurator)
        {
            _endpointFactoryConfigurator.AddEndpointFactoryConfigurator(configurator);
        }


        public IEndpointFactoryDefaultSettings Defaults
        {
            get { return _endpointFactoryConfigurator.Defaults; }
        }

        public IEndpointFactory CreateEndpointFactory()
        {
            return _endpointFactoryConfigurator.CreateEndpointFactory();
        }

        public IServiceBus CreateServiceBus()
        {
            LogAssemblyVersionInformation();

            IEndpointCache endpointCache = CreateEndpointCache();
            _settings.EndpointCache = endpointCache;

            BusBuilder builder = _builderFactory(_settings);

            _subscriptionRouterConfigurator.SetNetwork(_settings.Network);

            // run through all configurators that have been set and let
            // them do their magic
            foreach (BusBuilderConfigurator configurator in _configurators)
            {
                builder = configurator.Configure(builder);
            }

            IServiceBus bus = builder.Build();

            return bus;
        }

        static void LogAssemblyVersionInformation()
        {
            if (_log.IsInfoEnabled)
            {
                var assembly = typeof(ServiceBusFactory).Assembly;

                var assemblyVersion = assembly.GetName().Version;
                FileVersionInfo assemblyFileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

                string assemblyFileVersion = assemblyFileVersionInfo.FileVersion;

                _log.InfoFormat("MassTransit v{0}/v{1}, .NET Framework v{2}",
                    assemblyFileVersion,
                    assemblyVersion,
                    Environment.Version);
            }
        }

        /// <summary>
        /// This lets you change the bus settings without
        /// having to implement a <see cref="BusBuilderConfigurator"/>
        /// first. Use with caution.
        /// </summary>
        /// <param name="callback">The callback that changes the settings.</param>
        public void ChangeSettings([NotNull] Action<ServiceBusSettings> callback)
        {
            if (callback == null) throw new ArgumentNullException("callback");
            callback(_settings);
        }

        IEndpointCache CreateEndpointCache()
        {
            if (_settings.EndpointCache != null)
                return _settings.EndpointCache;

            IEndpointFactory endpointFactory = CreateEndpointFactory();

            IEndpointCache endpointCache = new EndpointCache(endpointFactory);

            return endpointCache;
        }

        static BusBuilder DefaultBuilderFactory(BusSettings settings)
        {
            return new ServiceBusBuilderImpl(settings);
        }
    }
}