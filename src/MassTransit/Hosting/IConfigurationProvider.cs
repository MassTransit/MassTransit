namespace MassTransit.Hosting
{
    using System.Collections.Specialized;


    public interface IConfigurationProvider
    {
        bool TryGetSetting(string name, out string value);
        bool TryGetConnectionString(string name, out string connectionString, out string providerName);
        bool TryGetNameValueCollectionSection(string section, out NameValueCollection collection);
    }
}