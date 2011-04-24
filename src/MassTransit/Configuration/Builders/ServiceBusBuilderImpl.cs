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
	using Configurators;
	using log4net;
	using Magnum;
	using Pipeline.Configuration;
	using Util;

	public class ServiceBusBuilderImpl :
		ServiceBusBuilder,
		IDisposable
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (ServiceBusBuilderImpl));

		readonly IList<Action<ServiceBus>> _postCreateActions;
		readonly BusSettings _settings;
		bool _disposed;
		IEndpointCache _endpointCache;

		public ServiceBusBuilderImpl(BusSettings settings, IEndpointCache endpointCache)
		{
			Guard.AgainstNull(settings, "description");
			_settings = settings;

			_postCreateActions = new List<Action<ServiceBus>>();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public BusSettings Settings
		{
			get { return _settings; }
		}

		public IEndpointCache EndpointCache
		{
			get { throw new NotImplementedException(); }
		}

		public IControlBus Build()
		{
			if (_settings.EndpointCache == null)
			{
				var endpointFactory = CreateEndpointFactory();

				_endpointCache = CreateEndpointCache(endpointFactory);
			}
			else
			{
				_endpointCache = _settings.EndpointCache;
			}

			ServiceBus bus = CreateServiceBus(_endpointCache);

//			ConfigurePoisonEndpoint(bus);

			ConfigureBusSettings(bus);

			ConfigureMessageInterceptors(bus);

			ConfigureControlBus(bus);

			ConfigureBusServices(bus);

			if (_settings.AutoStart)
			{
				bus.Start();
			}

			return bus;
		}


		public void UseControlBus(IControlBus controlBus)
		{
			_postCreateActions.Add(bus => bus.ControlBus = controlBus);
		}

		public void AddPostCreateAction(Action<ServiceBus> postCreateAction)
		{
			_postCreateActions.Add(postCreateAction);
		}

		public void AddBusServiceConfigurator(IBusServiceConfigurator configurator)
		{
			throw new NotImplementedException();
		}


		public void Match<T>([NotNull] Action<T> callback)
			where T : class, BusBuilder
		{
			Guard.AgainstNull(callback);

			if (typeof (T).IsAssignableFrom(GetType()))
				callback(this as T);
		}


		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			if (disposing)
			{
			}

			_disposed = true;
		}

		ServiceBus CreateServiceBus(IEndpointCache endpointCache)
		{
			IEndpoint endpoint = endpointCache.GetEndpoint(_settings.InputAddress);

			var serviceBus = new ServiceBus(endpoint, _settings.ObjectBuilder, endpointCache);

			return serviceBus;
		}

		void ConfigureBusSettings(ServiceBus bus)
		{
			// TODO validate these to ensure sane values are present

			if (_settings.ConcurrentConsumerLimit > 0)
				bus.MaximumConsumerThreads = _settings.ConcurrentConsumerLimit;

			if (_settings.ConcurrentReceiverLimit > 0)
				bus.ConcurrentReceiveThreads = _settings.ConcurrentReceiverLimit;

			bus.ReceiveTimeout = _settings.ReceiveTimeout;
		}

		void ConfigureMessageInterceptors(IServiceBus bus)
		{
			if (_settings.BeforeConsume != null || _settings.AfterConsume != null)
			{
				MessageInterceptorConfigurator.For(bus.InboundPipeline).Create(_settings.BeforeConsume, _settings.AfterConsume);
			}
		}
	}
}