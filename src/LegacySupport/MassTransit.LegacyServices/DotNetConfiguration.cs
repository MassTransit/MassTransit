// Copyright 2007-2010 The Apache Software Foundation.
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
namespace MassTransit.LegacyRuntime
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Reflection;
    using log4net;

    public class DotNetConfiguration :
        MassTransit.Services.LegacyProxy.Configuration
    {
        const string LegacyServiceControlUriKey = "LegacyServiceControlUri";
        const string LegacyServiceDataUriKey = "LegacyServiceDataUri";
        const string LegacyServiceEnabledKey = "LegacyServiceEnabled";
        const string SubscriptionServiceUriKey = "SubscriptionServiceUri";
        const string LegacyServiceProxyUriKey = "LegacyServiceProxyUri";

        static readonly ILog _log = LogManager.GetLogger(typeof(DotNetConfiguration));

        public bool LegacyServiceEnabled
        {
            get { return GetApplicationSetting(LegacyServiceEnabledKey, true); }
        }

        public Uri LegacyServiceControlUri
        {
            get { return GetUriApplicationSetting(LegacyServiceControlUriKey); }
        }

        public Uri LegacyServiceDataUri
        {
            get { return GetUriApplicationSetting(LegacyServiceDataUriKey); }
        }

        public Uri SubscriptionServiceUri
        {
            get { return GetUriApplicationSetting(SubscriptionServiceUriKey); }
        }

        public Uri LegacyServiceProxyUri
        {
            get { return GetUriApplicationSetting(LegacyServiceProxyUriKey); }
        }

        Uri GetUriApplicationSetting(string key)
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


        string GetApplicationSetting(string key)
        {
            return GetApplicationSetting(key, () => { throw new ConfigurationErrorsException("The configuration string was not found: " + key); });
        }

        string GetApplicationSetting(string key, Func<string> defaultValueProvider)
        {
            string value = ConfigurationManager.AppSettings[key];

            if (value == null && LocateConfiguration().AppSettings.Settings[key] != null)
                value = LocateConfiguration().AppSettings.Settings[key].Value;

            return value ?? defaultValueProvider();
        }

        T GetApplicationSetting<T>(string key, T defaultValue)
        {
            string value = GetApplicationSetting(key, () => null);

            if (value == null)
                return defaultValue;

            if (string.IsNullOrEmpty(value))
                return defaultValue;

            return ConvertStringToValue<T>(key, value);
        }

        static T ConvertStringToValue<T>(string keyname, string value)
        {
            TypeConverter tc = TypeDescriptor.GetConverter(typeof(T));
            if (tc.CanConvertFrom(typeof(string)))
            {
                return (T) tc.ConvertFrom(value);
            }

            string message = string.Format("The configuration string [{0}] could not be converted to {1}", keyname, typeof(T).Name);
            throw new ConfigurationErrorsException(message);
        }

        System.Configuration.Configuration LocateConfiguration()
        {
            var map = new ExeConfigurationFileMap
                      {
                          ExeConfigFilename = Assembly.GetExecutingAssembly().Location + ".config"
                      };

            _log.InfoFormat("Using Configuration File: {0}", map.ExeConfigFilename);

            return ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
        }
    }
}