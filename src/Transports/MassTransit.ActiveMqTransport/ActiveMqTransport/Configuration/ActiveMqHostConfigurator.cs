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
            switch (address.Scheme.ToLowerInvariant())
            {
                case ActiveMqHostAddress.AmqpScheme:
                    _settings = new AmqpHostSettings(address);
                    break;
                default:
                    _settings = new OpenWireHostSettings(address);
                    break;
            }


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

        public void UseSsl(bool enabled = true)
        {
            _settings.UseSsl = enabled;
            if (enabled && _settings.Port == 61616)
                _settings.Port = 61617;
        }

        public void UseSsl(bool enabled, bool updatePort)
        {
            _settings.UseSsl = enabled;
            if (enabled && updatePort && _settings.Port == 61616)
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

        public void EnableAsyncSend()
        {
            _settings.TransportOptions["nms.AsyncSend"] = "true";
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
