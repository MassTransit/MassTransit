namespace MassTransit.RabbitMqTransport.Configuration
{
    using System;


    class RabbitMqBatchPublishConfigurator :
        IRabbitMqBatchPublishConfigurator
    {
        readonly ConfigurationHostSettings.ConfigurationBatchSettings _settings;

        public RabbitMqBatchPublishConfigurator(ConfigurationHostSettings.ConfigurationBatchSettings settings)
        {
            _settings = settings;
        }

        public bool Enabled
        {
            set => _settings.Enabled = value;
        }

        public int MessageLimit
        {
            set => _settings.MessageLimit = value;
        }

        public int SizeLimit
        {
            set => _settings.SizeLimit = value;
        }

        public TimeSpan Timeout
        {
            set => _settings.Timeout = value;
        }
    }
}
