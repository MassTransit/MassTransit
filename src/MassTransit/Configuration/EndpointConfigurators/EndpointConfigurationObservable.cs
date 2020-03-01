namespace MassTransit.EndpointConfigurators
{
    using GreenPipes.Util;


    public class EndpointConfigurationObservable :
        Connectable<IEndpointConfigurationObserver>,
        IEndpointConfigurationObserver
    {
        public void EndpointConfigured<T>(T configurator)
            where T : IReceiveEndpointConfigurator
        {
            All(observer =>
            {
                observer.EndpointConfigured(configurator);

                return true;
            });
        }
    }
}
