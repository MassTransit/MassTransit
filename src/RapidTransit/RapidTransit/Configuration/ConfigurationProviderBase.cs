namespace RapidTransit.Configuration
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Xml;


    public abstract class ConfigurationProviderBase :
        IConfigurationProvider
    {
        bool IConfigurationProvider.TryGetSetting(string name, out string value)
        {
            AppSettingsSection settings = GetAppSettings();
            if (settings == null)
            {
                value = null;
                return false;
            }

            KeyValueConfigurationElement element = settings.Settings[name];
            if (element == null)
            {
                value = null;
                return false;
            }

            value = element.Value;
            return element.ElementInformation.IsPresent;
        }

        bool IConfigurationProvider.TryGetConnectionString(string name, out string connectionString, out string providerName)
        {
            ConnectionStringsSection connectionStrings = GetConnectionStrings();
            if (connectionStrings == null)
            {
                connectionString = null;
                providerName = null;
                return false;
            }

            ConnectionStringSettings setting = connectionStrings.ConnectionStrings[name];
            if (setting == null)
            {
                connectionString = null;
                providerName = null;
                return false;
            }

            connectionString = setting.ConnectionString;
            providerName = setting.ProviderName;
            return setting.ElementInformation.IsPresent;
        }

        bool IConfigurationProvider.TryGetNameValueCollectionSection(string section, out NameValueCollection collection)
        {
            ConfigurationSection configurationSection = GetSection(section);
            if (configurationSection == null)
            {
                collection = null;
                return false;
            }

            string xml = configurationSection.SectionInformation.GetRawXml();
            var sectionXmlDocument = new XmlDocument();
            sectionXmlDocument.LoadXml(xml);
            if (sectionXmlDocument.DocumentElement == null)
            {
                collection = null;
                return false;
            }

            var handler = new NameValueSectionHandler();
            collection = handler.Create(null, null, sectionXmlDocument.DocumentElement) as NameValueCollection;
            return collection != null;
        }

        protected abstract AppSettingsSection GetAppSettings();
        protected abstract ConnectionStringsSection GetConnectionStrings();
        protected abstract ConfigurationSection GetSection(string sectionName);

        protected static Func<AppSettingsSection> GetAppSettings(Configuration configuration)
        {
            bool appSettingsLoaded = false;
            AppSettingsSection appSettings = null;
            return () =>
                {
                    if (appSettingsLoaded)
                        return appSettings;

                    appSettings = configuration.AppSettings;
                    appSettingsLoaded = true;

                    return appSettings;
                };
        }

        protected static Func<ConnectionStringsSection> GetConnectionStrings(Configuration configuration)
        {
            bool connectionStringsLoaded = false;
            ConnectionStringsSection connectionStrings = null;
            return () =>
                {
                    if (connectionStringsLoaded)
                        return connectionStrings;

                    connectionStrings = configuration.ConnectionStrings;
                    connectionStringsLoaded = true;

                    return connectionStrings;
                };
        }
    }
}