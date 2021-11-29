namespace MassTransit.Observables
{
    using System.Threading.Tasks;
    using Util;


    public class ReceiveEndpointObservable :
        Connectable<IReceiveEndpointObserver>,
        IReceiveEndpointObserver
    {
        public Task Ready(ReceiveEndpointReady ready)
        {
            return ForEachAsync(x => x.Ready(ready));
        }

        public Task Stopping(ReceiveEndpointStopping stopping)
        {
            return ForEachAsync(x => x.Stopping(stopping));
        }

        public Task Completed(ReceiveEndpointCompleted completed)
        {
            return ForEachAsync(x => x.Completed(completed));
        }

        public Task Faulted(ReceiveEndpointFaulted faulted)
        {
            return ForEachAsync(x => x.Faulted(faulted));
        }
    }
}
