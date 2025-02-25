#nullable enable
namespace MassTransit.SqlTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Licensing;


    public abstract class ConfigurationSqlHostSettings :
        SqlHostSettings
    {
        readonly Lazy<Uri> _hostAddress;

        protected ConfigurationSqlHostSettings(Uri address)
            : this()
        {
            var hostAddress = new SqlHostAddress(address);

            Host = hostAddress.Host;
            if (address.Port != -1)
                Port = address.Port;

            if (!string.IsNullOrWhiteSpace(address.UserInfo))
            {
                var parts = address.UserInfo.Split(':');
                Username = UriDecode(parts[0]);

                if (parts.Length >= 2)
                    Password = UriDecode(parts[1]);
            }

            VirtualHost = hostAddress.VirtualHost;
            Area = hostAddress.Area;
        }

        protected ConfigurationSqlHostSettings()
        {
            VirtualHost = "/";

            IsolationLevel = IsolationLevel.RepeatableRead;

            ConnectionLimit = 10;

            _hostAddress = new Lazy<Uri>(FormatHostAddress);

            MaintenanceEnabled = true;
            MaintenanceInterval = TimeSpan.FromSeconds(5);
            QueueCleanupInterval = TimeSpan.FromMinutes(1);
            MaintenanceBatchSize = 10000;
        }

        public string? Host { get; set; }
        public string? InstanceName { get; set; }
        public int? Port { get; set; }
        public string? Database { get; set; }
        public string? Schema { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? License { get; set; }
        public string? LicenseFile { get; set; }

        public IsolationLevel IsolationLevel { get; set; }

        public int ConnectionLimit { get; set; }

        public bool MaintenanceEnabled { get; set; }
        public TimeSpan MaintenanceInterval { get; set; }
        public TimeSpan QueueCleanupInterval { get; set; }
        public int MaintenanceBatchSize { get; set; }

        public string? ConnectionTag { get; set; }

        public string? VirtualHost { get; set; }
        public string? Area { get; set; }

        public abstract ConnectionContextFactory CreateConnectionContextFactory(ISqlHostConfiguration configuration);

        public LicenseInfo? GetLicenseInfo()
        {
            if (!string.IsNullOrWhiteSpace(License))
                return LicenseReader.Load(License!);

            if (!string.IsNullOrWhiteSpace(LicenseFile))
                return LicenseReader.LoadFromFile(LicenseFile!);

            return null;
        }

        public Uri HostAddress => _hostAddress.Value;

        public virtual IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrWhiteSpace(Host))
                yield return this.Failure("Host", "Host must be specified");

            if(ConnectionLimit < 1)
                yield return this.Failure("ConnectionLimit", "must be >= 1");
        }

        static string UriDecode(string uri)
        {
            return Uri.UnescapeDataString(uri.Replace("+", "%2B"));
        }

        Uri FormatHostAddress()
        {
            if (string.IsNullOrWhiteSpace(Host))
                throw new ConfigurationException("Host cannot be empty");
            if (string.IsNullOrWhiteSpace(VirtualHost))
                throw new ConfigurationException("Domain cannot be empty");

            return new SqlHostAddress(Host!, InstanceName, Port, VirtualHost!, Area);
        }
    }
}
