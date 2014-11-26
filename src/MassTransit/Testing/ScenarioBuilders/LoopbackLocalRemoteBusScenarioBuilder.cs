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
namespace MassTransit.Testing.ScenarioBuilders
{
	using System;
	using BusConfigurators;
	using Magnum.Extensions;
	using Scenarios;
	using Transports;

	public class LoopbackLocalRemoteBusScenarioBuilder :
		EndpointScenarioBuilderImpl<LocalRemoteTestScenario>,
		LocalRemoteScenarioBuilder
	{
		const string DefaultLocalUri = "loopback://localhost/mt_client";
		const string DefaultRemoteUri = "loopback://localhost/mt_server";

		readonly ServiceBusConfiguratorImpl _localConfigurator;
		readonly ServiceBusConfiguratorImpl _remoteConfigurator;
		readonly ServiceBusDefaultSettings _settings;

		public LoopbackLocalRemoteBusScenarioBuilder()
		{
			_settings = new ServiceBusDefaultSettings();
			_settings.ConcurrentConsumerLimit = 4;
			_settings.ReceiveTimeout = 50.Milliseconds();

			_localConfigurator = new ServiceBusConfiguratorImpl(_settings);
			_localConfigurator.ReceiveFrom(DefaultLocalUri);

			_remoteConfigurator = new ServiceBusConfiguratorImpl(_settings);
			_remoteConfigurator.ReceiveFrom(DefaultRemoteUri);
		}

		public void ConfigureLocalBus(Action<ServiceBusConfigurator> configureCallback)
		{
			configureCallback(_localConfigurator);
		}

		public void ConfigureRemoteBus(Action<ServiceBusConfigurator> configureCallback)
		{
			configureCallback(_remoteConfigurator);
		}

		public override LocalRemoteTestScenario Build()
		{
			IEndpointFactory endpointFactory = BuildEndpointFactory();

			var scenario = new LocalRemoteTestScenarioImpl(endpointFactory);

			BuildLocalBus(scenario);
			BuildRemoteBus(scenario);

			return scenario;
		}

		protected virtual void BuildLocalBus(LocalRemoteTestScenarioImpl scenario)
		{
//			_localConfigurator.ChangeSettings(x => { x.EndpointCache = scenario.EndpointCache; });

			scenario.LocalBus = _localConfigurator.CreateServiceBus();
		}

		protected virtual void BuildRemoteBus(LocalRemoteTestScenarioImpl scenario)
		{
	//		_remoteConfigurator.ChangeSettings(x => { x.EndpointCache = scenario.EndpointCache; });

			scenario.RemoteBus = _remoteConfigurator.CreateServiceBus();
		}
	}
}