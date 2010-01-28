// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.RuntimeServices
{
	using System;
	using System.ComponentModel;
	using System.Configuration;
	using System.Reflection;
	using log4net;
	using Magnum.ObjectExtensions;

	public class Configuration :
		IConfiguration
	{
		private const string HealthServiceControlUriKey = "HealthServiceControlUri";
		private const string HealthServiceDataUriKey = "HealthServiceDataUri";
		private const string HealthServiceEnabledKey = "HealthServiceEnabled";
		private const string SubscriptionServiceEnabledKey = "SubscriptionServiceEnabled";
		private const string SubscriptionServiceUriKey = "SubscriptionServiceUri";
		private const string TimeoutServiceControlUriKey = "TimeoutServiceControlUri";
		private const string TimeoutServiceDataUriKey = "TimeoutServiceDataUri";
		private const string TimeoutServiceEnabledKey = "TimeoutServiceEnabled";

		private static readonly ILog _log = LogManager.GetLogger(typeof (Configuration));

		public bool SubscriptionServiceEnabled
		{
			get { return GetApplicationSetting(SubscriptionServiceEnabledKey, true); }
		}

		public Uri SubscriptionServiceUri
		{
			get { return GetUriApplicationSetting(SubscriptionServiceUriKey); }
		}

		public bool TimeoutServiceEnabled
		{
			get { return GetApplicationSetting(TimeoutServiceEnabledKey, true); }
		}

		public Uri TimeoutServiceControlUri
		{
			get { return GetUriApplicationSetting(TimeoutServiceControlUriKey); }
		}

		public Uri TimeoutServiceDataUri
		{
			get { return GetUriApplicationSetting(TimeoutServiceDataUriKey); }
		}

		public bool HealthServiceEnabled
		{
			get { return GetApplicationSetting(HealthServiceEnabledKey, true); }
		}

		public Uri HealthServiceControlUri
		{
			get { return GetUriApplicationSetting(HealthServiceControlUriKey); }
		}

		public Uri HealthServiceDataUri
		{
			get { return GetUriApplicationSetting(HealthServiceDataUriKey); }
		}

		private Uri GetUriApplicationSetting(string key)
		{
			try
			{
				var value = new Uri(GetApplicationSetting(key));

				return value;
			}
			catch (UriFormatException ex)
			{
				throw new ConfigurationErrorsException("The " + key + " is not a valid Uri", ex);
			}
			catch (ConfigurationErrorsException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new ConfigurationErrorsException("The " + key + " application setting failed to load", ex);
			}
		}

		private string GetApplicationSetting(string key)
		{
			return GetApplicationSetting(key, () => { throw new ConfigurationErrorsException("The configuration string was not found: " + key); });
		}

		private string GetApplicationSetting(string key, Func<string> defaultValueProvider)
		{
			string value = ConfigurationManager.AppSettings[key];

            if (value == null && LocateConfiguration().AppSettings.Settings[key] != null)
                value = LocateConfiguration().AppSettings.Settings[key].Value;

			return value ?? defaultValueProvider();
		}

		private T GetApplicationSetting<T>(string key)
		{
			string value = GetApplicationSetting(key);

			return ConvertStringToValue<T>(key, value);
		}

		private T GetApplicationSetting<T>(string key, T defaultValue)
		{
			string value = GetApplicationSetting(key, () => null);

			if (value == null)
				return defaultValue;

			if (value.IsNullOrEmpty())
				return defaultValue;

			return ConvertStringToValue<T>(key, value);
		}

		private System.Configuration.Configuration LocateConfiguration()
		{
			var map = new ExeConfigurationFileMap
				{
					ExeConfigFilename = Assembly.GetExecutingAssembly().Location + ".config"
				};

			_log.InfoFormat("Using Configuration File: {0}", map.ExeConfigFilename);

			return ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
		}

		private string GetConnectionString(string connectionName)
		{
			ConnectionStringSettings connectionSettings = ConfigurationManager.ConnectionStrings[connectionName] ?? LocateConfiguration().ConnectionStrings.ConnectionStrings[connectionName];

			if (connectionSettings == null)
				throw new ConfigurationErrorsException("There are no configuration string configured");

			string connectionString = connectionSettings.ConnectionString;

			return connectionString;
		}

		private static T ConvertStringToValue<T>(string keyname, string value)
		{
			TypeConverter tc = TypeDescriptor.GetConverter(typeof (T));
			if (tc.CanConvertFrom(typeof (string)))
			{
				return (T) tc.ConvertFrom(value);
			}

			string message = string.Format("The configuration string [{0}] could not be converted to {1}", keyname, typeof (T).Name);
			throw new ConfigurationErrorsException(message);
		}
	}
}