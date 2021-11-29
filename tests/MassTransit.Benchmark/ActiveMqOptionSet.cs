namespace MassTransitBenchmark
{
    using System;
    using Apache.NMS;
    using MassTransit;
    using MassTransit.ActiveMqTransport;
    using MassTransit.ActiveMqTransport.Configuration;
    using NDesk.Options;


    class ActiveMqOptionSet :
        OptionSet,
        ActiveMqHostSettings
    {
        ConfigurationHostSettings _hostSettings;

        public ActiveMqOptionSet()
        {
            _hostSettings = new ConfigurationHostSettings(new Uri("activemq://localhost"));

            Add<string>("h|host:", "The host name of the broker", x =>
            {
                _hostSettings = new ConfigurationHostSettings(new Uri($"activemq://{x}"))
                {
                    Port = _hostSettings.Port,
                    Username = _hostSettings.Username,
                    Password = _hostSettings.Password
                };
            });
            Add<int>("port:", "The virtual host to use", value => _hostSettings.Port = value);
            Add<string>("u|username:", "Username (if using basic credentials)", value => _hostSettings.Username = value);
            Add<string>("p|password:", "Password (if using basic credentials)", value => _hostSettings.Password = value);
        }

        public string Host => _hostSettings.Host;

        public int Port => _hostSettings.Port;

        public string Username => _hostSettings.Username;

        public string Password => _hostSettings.Password;

        public Uri HostAddress => _hostSettings.HostAddress;

        public bool UseSsl => _hostSettings.UseSsl;

        public Uri BrokerAddress => _hostSettings.BrokerAddress;

        public IConnection CreateConnection()
        {
            return _hostSettings.CreateConnection();
        }

        public override string ToString()
        {
            return new UriBuilder
            {
                Scheme = UseSsl ? "ssl" : "tcp",
                Host = Host
            }.Uri.ToString();
        }

        public void ShowOptions()
        {
            Console.WriteLine("Host: {0}", HostAddress);
        }
    }
}
