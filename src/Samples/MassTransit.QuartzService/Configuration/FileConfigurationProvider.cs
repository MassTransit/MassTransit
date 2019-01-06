namespace MassTransit.QuartzService.Configuration
{
    using System;
    using System.Configuration;
    using System.Reflection;


    public class FileConfigurationProvider :
        IConfigurationProvider
    {
        readonly Func<AppSettingsSection> _appSettings;
        readonly Func<ConnectionStringsSection> _connectionStrings;
        readonly Func<string, object> _getSection;

        public FileConfigurationProvider()
            : this(Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly())
        {
        }

        public FileConfigurationProvider(Assembly assembly)
        {
            var map = new ExeConfigurationFileMap();

            map.ExeConfigFilename = assembly.Location + ".config";

            Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(map,
                ConfigurationUserLevel.None);

            _appSettings = () => configuration.AppSettings;
            _connectionStrings = () => configuration.ConnectionStrings;
            _getSection = configuration.GetSection;
        }

        public string GetSetting(string name)
        {
            AppSettingsSection settings = _appSettings();
            if (settings == null)
                return null;

            KeyValueConfigurationElement element = settings.Settings[name];
            if (element == null)
                return null;

            return element.Value;
        }

        public string GetConnectionString(string name)
        {
            ConnectionStringsSection connectionStrings = _connectionStrings();
            if (connectionStrings == null)
                return null;

            ConnectionStringSettings setting = connectionStrings.ConnectionStrings[name];
            if (setting == null)
                return null;

            return setting.ConnectionString;
        }
    }
}