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
namespace MassTransit.SystemView
{
    using System;
    using System.Configuration;
    using System.Reflection;
    using Logging;

    public class Configuration :
        IConfiguration
    {
        const string SubscriptionServiceUriKey = "SubscriptionServiceUri";
        const string SystemViewControlUriKey = "SystemViewControlUri";
        const string SystemViewDataUriKey = "SystemViewDataUri";

        static readonly ILog _log = Logger.Get(typeof (Configuration));

        public Uri SubscriptionServiceUri
        {
            get { return GetUriApplicationSetting(SubscriptionServiceUriKey); }
        }

        public Uri SystemViewControlUri
        {
            get { return GetUriApplicationSetting(SystemViewControlUriKey); }
        }

        public Uri SystemViewDataUri
        {
            get { return GetUriApplicationSetting(SystemViewDataUriKey); }
        }

        static Uri GetUriApplicationSetting(string key)
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

        static string GetApplicationSetting(string key)
        {
            string value = ConfigurationManager.AppSettings[key] ?? LocateConfiguration().AppSettings.Settings[key].Value;

            if (value == null)
                throw new ConfigurationErrorsException("There are no configuration string configured");

            return value;
        }

        static System.Configuration.Configuration LocateConfiguration()
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