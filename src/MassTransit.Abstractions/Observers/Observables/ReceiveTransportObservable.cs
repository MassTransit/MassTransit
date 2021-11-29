namespace MassTransit.Observables
{
    using System.Threading.Tasks;
    using Util;


    public class ReceiveTransportObservable :
        Connectable<IReceiveTransportObserver>,
        IReceiveTransportObserver
    {
        public Task Ready(ReceiveTransportReady ready)
        {
            return ForEachAsync(x => x.Ready(ready));
        }

        public Task Completed(ReceiveTransportCompleted completed)
        {
            return ForEachAsync(x => x.Completed(completed));
        }

        public Task Faulted(ReceiveTransportFaulted faulted)
        {
            return ForEachAsync(x => x.Faulted(faulted));
        }
    }
}
