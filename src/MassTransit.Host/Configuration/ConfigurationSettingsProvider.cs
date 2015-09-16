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


    public class ConfigurationSettingsProvider :
        ISettingsProvider
    {
        readonly IConfigurationProvider _configurationProvider;
        readonly IObjectConverterCache _converterCache;

        public ConfigurationSettingsProvider(IConfigurationProvider configurationProvider, IObjectConverterCache convertCache)
        {
            _configurationProvider = configurationProvider;
            _converterCache = convertCache;
        }

        public bool TryGetSettings<T>(string prefix, out T settings)
            where T : ISettings
        {
            IObjectConverter converter = _converterCache.GetConverter(typeof(T));

            var provider = new ConfigurationObjectValueProvider(_configurationProvider, prefix);

            settings = (T)converter.GetObject(provider);
            return true;
        }

        public bool TryGetSettings<T>(out T settings)
            where T : ISettings
        {
            IObjectConverter converter = _converterCache.GetConverter(typeof(T));

            var provider = new ConfigurationObjectValueProvider(_configurationProvider);

            settings = (T)converter.GetObject(provider);
            return true;
        }
    }
}