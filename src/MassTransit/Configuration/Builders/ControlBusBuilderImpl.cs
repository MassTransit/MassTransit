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
	using Configuration;
	using log4net;
	using Magnum;
	using Util;

	public class ControlBusBuilderImpl :
		ControlBusBuilder
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (ControlBusBuilderImpl));

		readonly BusSettings _settings;

		public ControlBusBuilderImpl([NotNull] BusSettings settings)
		{
			Guard.AgainstNull(settings, "settings");

			_settings = settings;
		}

		public BusSettings Settings
		{
			get { return _settings; }
		}

		public IControlBus Build()
		{
			if (_log.IsDebugEnabled)
				_log.DebugFormat("Creating ControlBus at {0}", _settings.InputAddress);

			ServiceBus bus = CreateServiceBus();

			ConfigureBusSettings(bus);

			if (_settings.AutoStart)
			{
				bus.Start();
			}

			return bus;
		}

		public void Match<T>(Action<T> callback)
			where T : class, BusBuilder
		{
			Guard.AgainstNull(callback);

			if (typeof(T).IsAssignableFrom(GetType()))
				callback(this as T);
		}

		ServiceBus CreateServiceBus()
		{
			IEndpoint endpoint = _settings.EndpointCache.GetEndpoint(_settings.InputAddress);

			var serviceBus = new ServiceBus(endpoint, _settings.EndpointCache);

			return serviceBus;
		}

		void ConfigureBusSettings(ServiceBus bus)
		{
			if (_settings.ConcurrentConsumerLimit > 0)
				bus.MaximumConsumerThreads = _settings.ConcurrentConsumerLimit;

			if (_settings.ConcurrentReceiverLimit > 0)
				bus.ConcurrentReceiveThreads = _settings.ConcurrentReceiverLimit;

			bus.ReceiveTimeout = _settings.ReceiveTimeout;
		}
	}
}