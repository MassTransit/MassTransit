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
namespace MassTransit.SystemView.Core
{
    using System;
    using System.Configuration;
    using System.Reflection;

    public class Configuration :
        IConfiguration
    {
        private const string _subscriptionServiceUriKey = "SubscriptionServiceUri";
        private const string _systemViewControlUriKey = "SystemViewControlUri";
        private const string _systemViewDataUriKey = "SystemViewDataUri";

        public Uri SubscriptionServiceUri
        {
            get
            {
                return GetUriApplicationSetting(_subscriptionServiceUriKey);
            }
        }

        public Uri SystemViewControlUri
        {
            get
            {
                return GetUriApplicationSetting(_systemViewControlUriKey);
            }
        }

        public Uri SystemViewDataUri
        {
            get
            {
                return GetUriApplicationSetting(_systemViewDataUriKey);
            }
        }

        private static Uri GetUriApplicationSetting(string key)
        {
            try
            {
                Uri value = new Uri(GetApplicationSetting(key));

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

        private static string GetApplicationSetting(string key)
        {
            string value = ConfigurationManager.AppSettings[key] ?? LocateConfiguration().AppSettings.Settings[key].Value;

            if (value == null)
                throw new ConfigurationErrorsException("There are no configuration string configured");

            return value;
        }

        private static System.Configuration.Configuration LocateConfiguration()
        {
            ExeConfigurationFileMap map = new ExeConfigurationFileMap
                {
                    ExeConfigFilename = Assembly.GetExecutingAssembly().Location + ".config"
                };

            return ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
        }
    }
}