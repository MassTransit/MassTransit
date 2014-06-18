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
namespace MassTransit.Advanced
{
	using System;
	using System.Configuration;
	using BusConfigurators;
	using Configuration;
	using EnvironmentConfigurators;

	public static class EnvironmentExtensions
	{
		public static void Environments(this ServiceBusConfigurator configurator,
		                                Action<EnvironmentsConfigurator> configureCallback)
		{
			var environmentsConfigurator = new EnvironmentsConfiguratorImpl();

			configureCallback(environmentsConfigurator);

			IServiceBusEnvironment environment = environmentsConfigurator.GetCurrentEnvironment();
			if (environment != null)
			{
				environment.Configure(configurator);
			}
		}

		/// <summary>
		/// Add an environment to the configuration
		/// </summary>
		/// <typeparam name="TEnvironment">The environment class to add</typeparam>
		/// <param name="configurator"></param>
		public static void Add<TEnvironment>(this EnvironmentsConfigurator configurator)
			where TEnvironment : class, IServiceBusEnvironment, new()
		{
			configurator.Add(() => new TEnvironment());
		}

		/// <summary>
		/// Add an environment to the configuration
		/// </summary>
		/// <typeparam name="TEnvironment">The environment class to add</typeparam>
		/// <param name="configurator"></param>
		/// <param name="environment">The environment instance already created and ready to use</param>
		public static void Add<TEnvironment>(this EnvironmentsConfigurator configurator, TEnvironment environment)
			where TEnvironment : class, IServiceBusEnvironment
		{
			configurator.Add(() => environment);
		}

		public static void Add(this EnvironmentsConfigurator configurator, string environmentName, Action<ServiceBusConfigurator> environmentConfigurator)
		{
			var environment = new DelegateEnvironmentConfigurator(environmentConfigurator);

			configurator.Add(environmentName, () => environment);
		}

		/// <summary>
		/// Add an environment to the configuration
		/// </summary>
		/// <typeparam name="TEnvironment">The environment class to add</typeparam>
		/// <param name="configurator"></param>
		/// <param name="environmentFactory">The factory method to create the environment instance</param>
		public static void Add<TEnvironment>(this EnvironmentsConfigurator configurator, Func<TEnvironment> environmentFactory)
			where TEnvironment : class, IServiceBusEnvironment
		{
			string environmentName = DefaultTypeNameConvention(typeof (TEnvironment));

			// do not collapse or 3.5 won't build
			configurator.Add(environmentName, () => environmentFactory());
		}

		/// <summary>
		/// Selects the current environment using an application setting from the App.config/Web.config
		/// </summary>
		/// <param name="configurator"></param>
		/// <param name="settingName"></param>
		public static void SelectByAppSetting(this EnvironmentsConfigurator configurator, string settingName)
		{
			string value = ConfigurationManager.AppSettings[settingName];
			if (string.IsNullOrEmpty(value))
			{
				throw new MassTransit.ConfigurationException("The application setting was not found: " + settingName);
			}

			configurator.Select(value);
		}

		/// <summary>
		/// Selects the current environment using an environment variable from the current process
		/// </summary>
		/// <param name="configurator"></param>
		/// <param name="valueName"></param>
		public static void SelectByEnvironmentVariable(this EnvironmentsConfigurator configurator, string valueName)
		{
			string value = Environment.GetEnvironmentVariable(valueName);
			if (string.IsNullOrEmpty(value))
			{
				throw new MassTransit.ConfigurationException("The application setting was not found: " + valueName);
			}

			configurator.Select(value);
		}

		/// <summary>
		/// Selects the current environment using the local machine name
		/// </summary>
		/// <param name="configurator"></param>
		public static void SelectByMachineName(this EnvironmentsConfigurator configurator)
		{
			configurator.Select(Environment.MachineName);
		}

		static string DefaultTypeNameConvention(Type environmentType)
		{
			string name = environmentType.Name.ToLowerInvariant();
			if (name.Contains("environment"))
				name = name.Replace("environment", "");

			int genericMarks = name.IndexOf('`');
			if (genericMarks >= 0)
				name = name.Remove(genericMarks);

			if (name.Length == 0)
				throw new ArgumentException("The type must have something left after taking away environment");

			return name;
		}
	}
}