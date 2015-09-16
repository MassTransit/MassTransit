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
    using System;
    using System.Configuration;
    using System.IO;
    using System.Reflection;


    public class FileConfigurationProvider :
        ConfigurationProviderBase
    {
        readonly Func<AppSettingsSection> _appSettings;
        readonly Func<ConnectionStringsSection> _connectionStrings;
        readonly Func<string, ConfigurationSection> _getSection;

        public FileConfigurationProvider()
            : this(Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly())
        {
        }

        public FileConfigurationProvider(Assembly assembly)
        {
            var map = new ExeConfigurationFileMap();

            string filename = assembly.Location + ".config";
            if (!File.Exists(filename))
            {
                string assemblyName = Path.GetFileName(assembly.Location);
                filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyName + ".config");
            }
            map.ExeConfigFilename = filename;

            Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(map,
                ConfigurationUserLevel.None);

            _appSettings = GetAppSettings(configuration);

            _connectionStrings = GetConnectionStrings(configuration);
            _getSection = configuration.GetSection;
        }

        protected override AppSettingsSection GetAppSettings()
        {
            return _appSettings();
        }

        protected override ConnectionStringsSection GetConnectionStrings()
        {
            return _connectionStrings();
        }

        protected override ConfigurationSection GetSection(string sectionName)
        {
            return _getSection(sectionName);
        }
    }
}