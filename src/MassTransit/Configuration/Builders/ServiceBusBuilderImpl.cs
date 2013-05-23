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
namespace MassTransit.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using BusServiceConfigurators;
    using Configuration;
    using Exceptions;
    using Logging;
    using Magnum;
    using Magnum.Extensions;
    using Pipeline.Configuration;
    using Util;

    public class ServiceBusBuilderImpl :
        ServiceBusBuilder
    {
        static readonly ILog _log = Logger.Get<ServiceBusBuilderImpl>();
        readonly IList<BusServiceConfigurator> _busServiceConfigurators;
        readonly IList<Action<ServiceBus>> _postCreateActions;
        readonly BusSettings _settings;

        public ServiceBusBuilderImpl(BusSettings settings)
        {
            Guard.AgainstNull(settings, "settings");

            Guard.AgainstNull(settings.EndpointCache, "endpointCache");

            _settings = settings;

            _postCreateActions = new List<Action<ServiceBus>>();
            _busServiceConfigurators = new List<BusServiceConfigurator>();
        }

        public BusSettings Settings
        {
            get { return _settings; }
        }

        public IControlBus Build()
        {
            ServiceBus bus = CreateServiceBus(_settings.EndpointCache);

            try
            {
                ConfigureBusSettings(bus);

                RunPostCreateActions(bus);

                ConfigureMessageInterceptors(bus);

                RunBusServiceConfigurators(bus);

                if (_settings.AutoStart)
                {
                    bus.Start();
                }

                return bus;
            }
            catch
            {
                try
                {
                    bus.Dispose();
                }
                catch (Exception ex)
                {
                    _log.Error("Exception disposing of failed bus instance", ex);
                }

                throw;
            }
        }

        
        public void UseControlBus(IControlBus controlBus)
        {
            _postCreateActions.Add(bus => bus.ControlBus = controlBus);
        }

        public void AddPostCreateAction(Action<ServiceBus> postCreateAction)
        {
            _postCreateActions.Add(postCreateAction);
        }

        public void AddBusServiceConfigurator(BusServiceConfigurator configurator)
        {
            _busServiceConfigurators.Add(configurator);
        }

        public void Match<T>([NotNull] Action<T> callback)
            where T : class, BusBuilder
        {
            Guard.AgainstNull(callback);

            if (typeof (T).IsAssignableFrom(GetType()))
                callback(this as T);
        }

        void RunBusServiceConfigurators(ServiceBus bus)
        {
            foreach (BusServiceConfigurator busServiceConfigurator in _busServiceConfigurators)
            {
                try
                {
                    IBusService busService = busServiceConfigurator.Create(bus);

                    bus.AddService(busServiceConfigurator.Layer, busService);
                }
                catch (Exception ex)
                {
                    throw new ConfigurationException("Failed to create the bus service: " +
                                                     busServiceConfigurator.ServiceType.ToShortTypeName(), ex);
                }
            }
        }

        void RunPostCreateActions(ServiceBus bus)
        {
            foreach (var postCreateAction in _postCreateActions)
            {
                try
                {
                    postCreateAction(bus);
                }
                catch (Exception ex)
                {
                    throw new ConfigurationException("An exception was thrown while running post-creation actions", ex);
                }
            }
        }

        ServiceBus CreateServiceBus(IEndpointCache endpointCache)
        {
            IEndpoint endpoint = endpointCache.GetEndpoint(_settings.InputAddress);

            return new ServiceBus(endpoint, endpointCache, _settings.EnablePerformanceCounters);
        }

        void ConfigureBusSettings(ServiceBus bus)
        {
            if (_settings.ConcurrentConsumerLimit > 0)
                bus.MaximumConsumerThreads = _settings.ConcurrentConsumerLimit;

            if (_settings.ConcurrentReceiverLimit > 0)
                bus.ConcurrentReceiveThreads = _settings.ConcurrentReceiverLimit;

            bus.ReceiveTimeout = _settings.ReceiveTimeout;

            bus.ShutdownTimeout = _settings.ShutdownTimeout;
            ConfigureThreadPool(bus.MaximumConsumerThreads);
        }

        static void ConfigureThreadPool(int consumerThreads)
        {
            var requiredThreads = CalculateRequiredThreads(consumerThreads);

            ConfigureMinThreads(requiredThreads);

            ConfigureMaxThreads(requiredThreads);
        }

        static int CalculateRequiredThreads(int consumerThreads)
        {
            int workerThreads;
            int completionPortThreads;
            ThreadPool.GetMinThreads(out workerThreads, out completionPortThreads);
            int availableWorkerThreads;
            int availableCompletionPortThreads;
            ThreadPool.GetAvailableThreads(out availableWorkerThreads, out availableCompletionPortThreads);
            var requiredThreads = consumerThreads + (workerThreads - availableWorkerThreads);
            return requiredThreads;
        }

        static void ConfigureMinThreads(int requiredThreads)
        {
            int workerThreads;
            int completionPortThreads;
            ThreadPool.GetMinThreads(out workerThreads, out completionPortThreads);
            workerThreads = Math.Max(workerThreads, requiredThreads);
            ThreadPool.SetMinThreads(workerThreads, completionPortThreads);
        }

        static void ConfigureMaxThreads(int requiredThreads)
        {
            int workerThreads;
            int completionPortThreads;
            ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
            workerThreads = Math.Max(workerThreads, requiredThreads);
            ThreadPool.SetMaxThreads(workerThreads, completionPortThreads);
        }

        void ConfigureMessageInterceptors(IServiceBus bus)
        {
            if (_settings.BeforeConsume != null || _settings.AfterConsume != null)
            {
                var configurator = new InboundMessageInterceptorConfigurator(bus.InboundPipeline);

                var interceptor = new DelegateMessageInterceptor(_settings.BeforeConsume, _settings.AfterConsume);

                configurator.Create(interceptor);
            }
        }
    }
}