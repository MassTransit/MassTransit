// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Specialized;
    using Hosting;


    public class ManyConfigurationProviders :
        IConfigurationProvider
    {
        readonly IConfigurationProvider[] _configurationProviders;

        public ManyConfigurationProviders(params IConfigurationProvider[] configurationProviders)
        {
            _configurationProviders = configurationProviders;
        }

        public bool TryGetSetting(string name, out string value)
        {
            for (var i = 0; i < _configurationProviders.Length; i++)
            {
                if (_configurationProviders[i].TryGetSetting(name, out value))
                    return true;
            }

            value = null;
            return false;
        }

        public bool TryGetConnectionString(string name, out string connectionString, out string providerName)
        {
            for (var i = 0; i < _configurationProviders.Length; i++)
            {
                if (_configurationProviders[i].TryGetConnectionString(name, out connectionString, out providerName))
                    return true;
            }

            connectionString = null;
            providerName = null;
            return false;
        }

        public bool TryGetNameValueCollectionSection(string section, out NameValueCollection collection)
        {
            for (var i = 0; i < _configurationProviders.Length; i++)
            {
                if (_configurationProviders[i].TryGetNameValueCollectionSection(section, out collection))
                    return true;
            }

            collection = null;
            return false;
        }
    }
}