namespace MassTransitBenchmark
{
    using System;
    using System.Net;
    using Azure;
    using Azure.Core;
    using Azure.Messaging.ServiceBus;
    using Azure.Messaging.ServiceBus.Administration;
    using MassTransit;
    using MassTransit.AzureServiceBusTransport.Configuration;
    using NDesk.Options;


    public class ServiceBusOptionSet :
        OptionSet,
        ServiceBusHostSettings
    {
        readonly HostSettings _hostSettings;

        string _accessKey;
        string _keyName;

        public ServiceBusOptionSet()
        {
            _hostSettings = new HostSettings();

            Add<string>("ns=", "The service bus namespace",
                x => ServiceUri = new UriBuilder
                {
                    Scheme = "sb",
                    Host = $"{x}.servicebus.windows.net",
                    Path = "Benchmark"
                }.Uri);
            Add<string>("keyname=", "The access key name", x =>
            {
                _keyName = x;
                if (!string.IsNullOrWhiteSpace(_keyName) && !string.IsNullOrWhiteSpace(_accessKey))
                    _hostSettings.NamedKeyCredential = new AzureNamedKeyCredential(_keyName, _accessKey);
            });
            Add<string>("key=", "The access key", x =>
            {
                _accessKey = x;
                if (!string.IsNullOrWhiteSpace(_keyName) && !string.IsNullOrWhiteSpace(_accessKey))
                    _hostSettings.NamedKeyCredential = new AzureNamedKeyCredential(_keyName, _accessKey);
            });
            Add<bool>("split:", "Split into two bus instances to leverage separate connections", x => Split = x);
        }

        public bool Split { get; private set; }

        public Uri ServiceUri { get; private set; }
        public ServiceBusClient ServiceBusClient => _hostSettings.ServiceBusClient;

        public ServiceBusAdministrationClient ServiceBusAdministrationClient => _hostSettings.ServiceBusAdministrationClient;

        public AzureNamedKeyCredential NamedKeyCredential => _hostSettings.NamedKeyCredential;

        public AzureSasCredential SasCredential => _hostSettings.SasCredential;

        public TokenCredential TokenCredential => _hostSettings.TokenCredential;

        public string ConnectionString => _hostSettings.ConnectionString;
        public TimeSpan RetryMinBackoff => _hostSettings.RetryMinBackoff;

        public TimeSpan RetryMaxBackoff => _hostSettings.RetryMaxBackoff;

        public int RetryLimit => _hostSettings.RetryLimit;

        public ServiceBusTransportType TransportType => _hostSettings.TransportType;

        public void ShowOptions()
        {
            Console.WriteLine("Service URI: {0}", ServiceUri);
            Console.WriteLine("Key Name: {0}", _keyName);
            Console.WriteLine("Access Key: {0}", new string('*', (_accessKey ?? "default").Length));
            Console.WriteLine("Split: {0}", Split);
        }
    }
}
