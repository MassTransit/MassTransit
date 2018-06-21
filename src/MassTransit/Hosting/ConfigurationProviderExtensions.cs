// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit
{
    using Hosting;


    public static class ConfigurationProviderExtensions
    {
        public static string GetSetting(this IConfigurationProvider configuration, string key, string defaultValue)
        {
            string value;
            return configuration.TryGetSetting(key, out value)
                ? (string.IsNullOrEmpty(value) ? defaultValue : value)
                : defaultValue;
        }

        public static int GetSetting(this IConfigurationProvider configuration, string key, int defaultValue)
        {
            string value;
            int result;
            return configuration.TryGetSetting(key, out value)
                ? (string.IsNullOrEmpty(value)
                    ? defaultValue
                    : int.TryParse(value, out result) ? result : defaultValue)
                : defaultValue;
        }

        public static long GetSetting(this IConfigurationProvider configuration, string key, long defaultValue)
        {
            string value;
            long result;
            return configuration.TryGetSetting(key, out value)
                ? (string.IsNullOrEmpty(value)
                    ? defaultValue
                    : long.TryParse(value, out result) ? result : defaultValue)
                : defaultValue;
        }

        public static ushort GetSetting(this IConfigurationProvider configuration, string key, ushort defaultValue)
        {
            string value;
            ushort result;
            return configuration.TryGetSetting(key, out value)
                ? (string.IsNullOrEmpty(value)
                    ? defaultValue
                    : ushort.TryParse(value, out result) ? result : defaultValue)
                : defaultValue;
        }

        public static bool GetSetting(this IConfigurationProvider configuration, string key, bool defaultValue)
        {
            string value;
            bool result;
            return configuration.TryGetSetting(key, out value)
                ? (string.IsNullOrEmpty(value)
                    ? defaultValue
                    : bool.TryParse(value, out result) ? result : defaultValue)
                : defaultValue;
        }

        public static decimal GetSetting(this IConfigurationProvider configuration, string key, decimal defaultValue)
        {
            string value;
            decimal result;
            return configuration.TryGetSetting(key, out value)
                ? (string.IsNullOrEmpty(value)
                    ? defaultValue
                    : decimal.TryParse(value, out result) ? result : defaultValue)
                : defaultValue;
        }

        public static double GetSetting(this IConfigurationProvider configuration, string key, double defaultValue)
        {
            string value;
            double result;
            return configuration.TryGetSetting(key, out value)
                ? (string.IsNullOrEmpty(value)
                    ? defaultValue
                    : double.TryParse(value, out result) ? result : defaultValue)
                : defaultValue;
        }

        public static uint GetSetting(this IConfigurationProvider configuration, string key, uint defaultValue)
        {
            string value;
            uint result;
            return configuration.TryGetSetting(key, out value)
                ? (string.IsNullOrEmpty(value)
                    ? defaultValue
                    : uint.TryParse(value, out result) ? result : defaultValue)
                : defaultValue;
        }
    }
}