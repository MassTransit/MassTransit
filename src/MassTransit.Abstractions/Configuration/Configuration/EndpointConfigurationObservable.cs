namespace MassTransit.Configuration
{
    using Util;


    public class EndpointConfigurationObservable :
        Connectable<IEndpointConfigurationObserver>,
        IEndpointConfigurationObserver
    {
        public void EndpointConfigured<T>(T configurator)
            where T : IReceiveEndpointConfigurator
        {
            ForEach(observer => observer.EndpointConfigured(configurator));
        }

        public void Method4()
        {
        }

        public void Method5()
        {
        }

        public void Method6()
        {
        }
    }
}
