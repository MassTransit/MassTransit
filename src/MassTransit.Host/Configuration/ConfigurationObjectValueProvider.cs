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
namespace MassTransit.Host.Configuration
{
    using Hosting;
    using Internals.Mapping;


    public class ConfigurationObjectValueProvider :
        IObjectValueProvider
    {
        readonly IConfigurationProvider _configurationProvider;
        readonly string _prefix;

        public ConfigurationObjectValueProvider(IConfigurationProvider configurationProvider, string prefix = null)
        {
            _configurationProvider = configurationProvider;
            _prefix = prefix ?? "";
        }

        public bool TryGetValue(string name, out object value)
        {
            string settingValue;
            bool found = _configurationProvider.TryGetSetting(_prefix + name, out settingValue);

            value = settingValue;
            return found;
        }

        public bool TryGetValue<T>(string name, out T value)
        {
            object obj;
            if (TryGetValue(name, out obj))
            {
                if (obj is T)
                {
                    value = (T)obj;
                    return true;
                }
            }

            value = default(T);
            return false;
        }
    }
}