namespace MassTransit.Topology.Observers
{
    using GreenPipes.Util;


    public class ConsumeTopologyConfigurationObservable :
        Connectable<IConsumeTopologyConfigurationObserver>,
        IConsumeTopologyConfigurationObserver
    {
        public void MessageTopologyCreated<T>(IMessageConsumeTopologyConfigurator<T> configuration)
            where T : class
        {
            All(observer =>
            {
                observer.MessageTopologyCreated(configuration);

                return true;
            });
        }
    }
}
