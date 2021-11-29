namespace MassTransit.ActiveMqTransport.Configuration
{
    using System;
    using System.Collections.Generic;


    public class ActiveMqHostConfigurator :
        IActiveMqHostConfigurator
    {
        readonly ConfigurationHostSettings _settings;

        public ActiveMqHostConfigurator(Uri address)
        {
            _settings = new ConfigurationHostSettings(address);

            if (_settings.Port == 61617 || _settings.Host.EndsWith("amazonaws.com", StringComparison.OrdinalIgnoreCase))
                UseSsl();
        }

        public ActiveMqHostSettings Settings => _settings;

        public void Username(string username)
        {
            _settings.Username = username;
        }

        public void Password(string password)
        {
            _settings.Password = password;
        }

        public void UseSsl()
        {
            _settings.UseSsl = true;
            if (_settings.Port == 61616)
                _settings.Port = 61617;
        }

        public void FailoverHosts(string[] hosts)
        {
            _settings.FailoverHosts = hosts;
        }

        public void TransportOptions(IEnumerable<KeyValuePair<string, string>> options)
        {
            foreach (KeyValuePair<string, string> option in options)
                _settings.TransportOptions[option.Key] = option.Value;
        }

        public void EnableOptimizeAcknowledge()
        {
            _settings.TransportOptions["jms.optimizeAcknowledge"] = "true";
        }

        public void SetPrefetchPolicy(int limit)
        {
            _settings.TransportOptions["jms.prefetchPolicy.all"] = limit.ToString();
        }

        public void SetQueuePrefetchPolicy(int limit)
        {
            _settings.TransportOptions["jms.prefetchPolicy.queuePrefetch"] = limit.ToString();
        }
    }
}
