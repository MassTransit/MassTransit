namespace MassTransit.HttpTransport.Configuration
{
    using System.Net.Http;
    using Hosting;
    using HttpTransport;


    public class HttpHostConfigurator : IHttpHostConfigurator
    {
        readonly HttpHostSettingsImpl _settings;

        public HttpHostConfigurator(string scheme, string host, int port)
        {
            _settings = new HttpHostSettingsImpl(scheme, host, port, HttpMethod.Post);
        }

        public void UseMethod(HttpMethod method)
        {
            _settings.Method = method;
        }

        public HttpHostSettings Settings
        {
            get
            {
                return _settings;
            }
        }
    }
}