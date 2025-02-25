#nullable enable
namespace MassTransit.SqlTransport.Configuration
{
    using System;
    using System.Data;


    public abstract class SqlHostConfigurator :
        ISqlHostConfigurator
    {
        readonly ConfigurationSqlHostSettings _settings;

        protected SqlHostConfigurator(ConfigurationSqlHostSettings settings)
        {
            _settings = settings;

            var licensePath = Environment.GetEnvironmentVariable("MT_LICENSE_PATH");
            if (!string.IsNullOrWhiteSpace(licensePath))
                UseLicenseFile(licensePath);
            else
            {
                var license = Environment.GetEnvironmentVariable("MT_LICENSE");
                if (!string.IsNullOrWhiteSpace(license))
                    UseLicense(license);
            }
        }

        public abstract string? ConnectionString { set; }

        public string? ConnectionTag
        {
            set => _settings.ConnectionTag = value;
        }

        public string? Host
        {
            set => _settings.Host = value;
        }

        public string? InstanceName
        {
            set => _settings.InstanceName = value;
        }

        public int? Port
        {
            set => _settings.Port = value;
        }

        public string? Database
        {
            set => _settings.Database = value;
        }

        public string? Schema
        {
            set => _settings.Schema = value;
        }

        public string? Username
        {
            set => _settings.Username = value;
        }

        public string? Password
        {
            set => _settings.Password = value;
        }

        public string? VirtualHost
        {
            set => _settings.VirtualHost = value;
        }

        public string? Area
        {
            set => _settings.Area = value;
        }

        public IsolationLevel IsolationLevel
        {
            set => _settings.IsolationLevel = value;
        }

        public int ConnectionLimit
        {
            set => _settings.ConnectionLimit = value;
        }

        public bool MaintenanceEnabled
        {
            set => _settings.MaintenanceEnabled = value;
        }

        public TimeSpan MaintenanceInterval
        {
            set => _settings.MaintenanceInterval = value;
        }

        public TimeSpan QueueCleanupInterval
        {
            set => _settings.QueueCleanupInterval = value;
        }

        public int MaintenanceBatchSize
        {
            set => _settings.MaintenanceBatchSize = value;
        }

        public void UseLicense(string license)
        {
            _settings.License = license;
        }

        public void UseLicenseFile(string path)
        {
            _settings.LicenseFile = path;
        }
    }
}
