namespace MassTransit.Host
{
    using System.Configuration;

    public class WinServiceSettings
    {
        public string ServiceName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string[] Dependencies { get; set; }

        public static WinServiceSettings DotNetConfig
        {
            get
            {
                return new WinServiceSettings()
                {
                    ServiceName = ConfigurationManager.AppSettings["serviceName"],
                    DisplayName = ConfigurationManager.AppSettings["displayName"],
                    Description = ConfigurationManager.AppSettings["description"],
                    Dependencies = ConfigurationManager.AppSettings["dependencies"].Split(',')
                };
            }
        }

        public static WinServiceSettings Custom(string serviceName, string displayName, string description, params string[] dependencies)
        {
            return new WinServiceSettings()
                       {
                           ServiceName = serviceName,
                           DisplayName = displayName,
                           Description = description,
                           Dependencies = dependencies
                       };
        }
    }
}