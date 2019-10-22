namespace MassTransit.HttpTransport
{
    using Hosting;


    public interface IHttpBusFactoryConfigurator :
        IBusFactoryConfigurator<IHttpReceiveEndpointConfigurator>,
        IReceiveConfigurator<IHttpHost, IHttpReceiveEndpointConfigurator>
    {
        void Host(HttpHostSettings settings);
    }
}
