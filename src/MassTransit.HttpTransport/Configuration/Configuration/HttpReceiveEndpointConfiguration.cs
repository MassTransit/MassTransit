namespace MassTransit.HttpTransport.Configuration
{
    using System;
    using Builders;
    using GreenPipes;
    using GreenPipes.Builders;
    using GreenPipes.Configurators;
    using MassTransit.Configuration;
    using Transport;
    using Transports;
    using Util;


    public class HttpReceiveEndpointConfiguration :
        ReceiveEndpointConfiguration,
        IHttpReceiveEndpointConfiguration,
        IHttpReceiveEndpointConfigurator
    {
        readonly IHttpEndpointConfiguration _endpointConfiguration;
        readonly IHttpHostConfiguration _hostConfiguration;
        readonly IBuildPipeConfigurator<HttpHostContext> _httpHostPipeConfigurator;
        readonly string _pathMatch;

        public HttpReceiveEndpointConfiguration(IHttpHostConfiguration hostConfiguration, string pathMatch, IHttpEndpointConfiguration endpointConfiguration)
            : base(endpointConfiguration)
        {
            _hostConfiguration = hostConfiguration;
            _pathMatch = pathMatch;
            _endpointConfiguration = endpointConfiguration;

            HostAddress = hostConfiguration.HostAddress;
            InputAddress = new Uri(hostConfiguration.HostAddress, $"{pathMatch}");

            _httpHostPipeConfigurator = new PipeConfigurator<HttpHostContext>();
        }

        IHttpTopologyConfiguration IHttpEndpointConfiguration.Topology => _endpointConfiguration.Topology;

        public override Uri HostAddress { get; }

        public override Uri InputAddress { get; }

        public void Build(IHttpHostControl host)
        {
            var builder = new HttpReceiveEndpointBuilder(host, this);

            ApplySpecifications(builder);

            var receiveEndpointContext = builder.CreateReceiveEndpointContext();

            var receiveSettings = new Settings(_pathMatch);

            var httpConsumerFilter = new HttpConsumerFilter(_hostConfiguration.Settings, receiveSettings, receiveEndpointContext);

            _httpHostPipeConfigurator.UseFilter(httpConsumerFilter);

            var transport = new HttpReceiveTransport(host, receiveEndpointContext, _httpHostPipeConfigurator.Build());
            transport.Add(httpConsumerFilter);

            var receiveEndpoint = new ReceiveEndpoint(transport, receiveEndpointContext);

            var queueName = string.IsNullOrWhiteSpace(_pathMatch) ? NewId.Next().ToString(FormatUtil.Formatter) : _pathMatch;

            host.AddReceiveEndpoint(queueName, receiveEndpoint);

            ReceiveEndpoint = receiveEndpoint;
        }


        class Settings :
            ReceiveSettings
        {
            public Settings(string pathMatch)
            {
                PathMatch = pathMatch;
            }

            public string PathMatch { get; }
        }
    }
}
