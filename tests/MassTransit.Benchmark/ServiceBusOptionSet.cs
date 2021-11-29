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
    using MassTransit.Configuration;
    using NDesk.Options;


    class ServiceBusOptionSet :
        OptionSet,
        ServiceBusHostSettings
    {
        readonly HostSettings _hostSettings;

        string _accessKey;
        string _keyName;

        public ServiceBusOptionSet()
        {
            _hostSettings = new HostSettings();
            DefaultConnections = ServicePointManager.DefaultConnectionLimit;

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
                _hostSettings.NamedKeyCredential = new AzureNamedKeyCredential(_keyName, _accessKey);
            });
            Add<string>("key=", "The access key", x =>
            {
                _accessKey = x;
                _hostSettings.NamedKeyCredential = new AzureNamedKeyCredential(_keyName, _accessKey);
            });
            Add<int>("connections=", "The number of connections to configure for the service point manager", x => DefaultConnections = x);
            Add<bool>("split:", "Split into two bus instances to leverage separate connections", x => Split = x);
        }

        public int DefaultConnections { get; set; }
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
            Console.WriteLine("Service Point Manager.Default Connections: {0}", DefaultConnections);
        }
    }
}
