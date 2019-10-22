namespace MassTransit.HttpTransport.Hosting
{
    using System.Net.Http;


    public class ConfigurationHostSettings :
        HttpHostSettings
    {
        public ConfigurationHostSettings(string scheme, string host, int port, HttpMethod method)
        {
            Scheme = scheme;
            Host = host;
            Port = port;
            Method = method;
        }

        public ConfigurationHostSettings()
        {
        }

        public HttpMethod Method { get; set; }
        public string Scheme { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
