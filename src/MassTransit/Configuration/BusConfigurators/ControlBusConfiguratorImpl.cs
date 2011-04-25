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
namespace MassTransit.BusConfigurators
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using MassTransit.Builders;
	using MassTransit.Configuration;

	public class ControlBusConfiguratorImpl :
		ControlBusConfigurator,
		BusBuilderConfigurator
	{
		readonly IList<BusBuilderConfigurator> _configurators;
		readonly ServiceBusSettings _settings;

		public ControlBusConfiguratorImpl(ServiceBusDefaultSettings defaultSettings)
		{
			_configurators = new List<BusBuilderConfigurator>();
			_settings = new ServiceBusSettings(defaultSettings);
		}

		public BusBuilder Configure(BusBuilder builder)
		{
			builder.Match<ServiceBusBuilder>(x =>
				{
					if (_settings.InputAddress == null)
						_settings.InputAddress = builder.Settings.InputAddress.AppendToPath("_control");

					_settings.ObjectBuilder = builder.Settings.ObjectBuilder;
					_settings.AutoStart = true;

					BusBuilder controlBusBuilder = new ControlBusBuilderImpl(_settings, x.EndpointCache);

					controlBusBuilder = _configurators
						.Aggregate(controlBusBuilder, (current, configurator) => configurator.Configure(current));

					IControlBus controlBus = controlBusBuilder.Build();

					x.UseControlBus(controlBus);
				});

			return builder;
		}

		public void Validate()
		{
			_configurators.Each(x => x.Validate());
		}

		public void ReceiveFrom(Uri uri)
		{
			_settings.InputAddress = uri;
		}
	}
}