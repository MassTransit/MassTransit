namespace MassTransit.HttpTransport.Specifications
{
    using System.Net.Http;
    using Hosting;


    public class HttpHostConfigurator :
        IHttpHostConfigurator
    {
        readonly ConfigurationHostSettings _settings;

        public HttpHostConfigurator(string scheme, string host, int port)
        {
            _settings = new ConfigurationHostSettings
            {
                Scheme = scheme,
                Host = host,
                Port = port,
                Method = HttpMethod.Post
            };
        }

        public HttpHostSettings Settings => _settings;

        public HttpMethod Method
        {
            set => _settings.Method = value;
        }
    }
}
