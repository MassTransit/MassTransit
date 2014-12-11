namespace RapidTransit.Configuration
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