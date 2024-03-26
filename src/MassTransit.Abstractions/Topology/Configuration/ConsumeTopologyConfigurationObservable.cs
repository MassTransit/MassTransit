namespace MassTransit.Configuration
{
    using Util;


    public class ConsumeTopologyConfigurationObservable :
        Connectable<IConsumeTopologyConfigurationObserver>,
        IConsumeTopologyConfigurationObserver
    {
        public void MessageTopologyCreated<T>(IMessageConsumeTopologyConfigurator<T> configuration)
            where T : class
        {
            ForEach(observer => observer.MessageTopologyCreated(configuration));
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
