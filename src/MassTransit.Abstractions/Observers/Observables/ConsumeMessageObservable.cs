namespace MassTransit.Observables
{
    using System;
    using System.Threading.Tasks;
    using Util;


    public class ConsumeMessageObservable<T> :
        Connectable<IConsumeMessageObserver<T>>,
        IConsumeMessageObserver<T>
        where T : class
    {
        public Task PreConsume(ConsumeContext<T> context)
        {
            return ForEachAsync(x => x.PreConsume(context));
        }

        public Task PostConsume(ConsumeContext<T> context)
        {
            return ForEachAsync(x => x.PostConsume(context));
        }

        public Task ConsumeFault(ConsumeContext<T> context, Exception exception)
        {
            return ForEachAsync(x => x.ConsumeFault(context, exception));
        }
    }
}
