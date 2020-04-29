namespace MassTransit.RabbitMqTransport.Configurators
{
    using System;


    class BatchPublishConfigurator :
        IBatchPublishConfigurator
    {
        readonly ConfigurationHostSettings.ConfigurationBatchSettings _settings;

        public BatchPublishConfigurator(ConfigurationHostSettings.ConfigurationBatchSettings settings)
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
