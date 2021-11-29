namespace MassTransit.Observables
{
    using System;
    using System.Threading.Tasks;
    using Util;


    public class SendObservable :
        Connectable<ISendObserver>,
        ISendObserver
    {
        public Task PreSend<T>(SendContext<T> context)
            where T : class
        {
            return ForEachAsync(x => x.PreSend(context));
        }

        public Task PostSend<T>(SendContext<T> context)
            where T : class
        {
            return ForEachAsync(x => x.PostSend(context));
        }

        public Task SendFault<T>(SendContext<T> context, Exception exception)
            where T : class
        {
            return ForEachAsync(x => x.SendFault(context, exception));
        }
    }
}
