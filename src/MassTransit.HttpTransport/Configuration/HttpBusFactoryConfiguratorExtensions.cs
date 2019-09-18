namespace MassTransit.HttpTransport
{
    using System;
    using Specifications;


    public static class HttpBusFactoryConfiguratorExtensions
    {
        public static void Host(this IHttpBusFactoryConfigurator cfg, Uri hostAddress, Action<IHttpHostConfigurator> configure = null)
        {
            cfg.Host(hostAddress.Scheme, hostAddress.Host, hostAddress.Port, configure);
        }

        public static void Host(this IHttpBusFactoryConfigurator cfg, string scheme, string host, int port, Action<IHttpHostConfigurator> configure = null)
        {
            var httpHostCfg = new HttpHostConfigurator(scheme, host, port);

            configure?.Invoke(httpHostCfg);

            cfg.Host(httpHostCfg.Settings);
        }
    }
}
