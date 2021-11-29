namespace MassTransit.Observables
{
    using System;
    using System.Threading.Tasks;


    public class ConsumeObserverAdapter :
        IFilterObserver
    {
        readonly IConsumeObserver _observer;

        public ConsumeObserverAdapter(IConsumeObserver observer)
        {
            _observer = observer;
        }

        Task IFilterObserver.PreSend<T>(T context)
        {
            return ConsumeObserverConverterCache.PreConsume(typeof(T), _observer, context);
        }

        Task IFilterObserver.PostSend<T>(T context)
        {
            return ConsumeObserverConverterCache.PostConsume(typeof(T), _observer, context);
        }

        Task IFilterObserver.SendFault<T>(T context, Exception exception)
        {
            return ConsumeObserverConverterCache.ConsumeFault(typeof(T), _observer, context, exception);
        }
    }


    public class ConsumeObserverAdapter<T> :
        IFilterObserver<ConsumeContext<T>>
        where T : class
    {
        readonly IConsumeMessageObserver<T> _observer;

        public ConsumeObserverAdapter(IConsumeMessageObserver<T> observer)
        {
            _observer = observer;
        }

        Task IFilterObserver<ConsumeContext<T>>.PreSend(ConsumeContext<T> context)
        {
            return _observer.PreConsume(context);
        }

        Task IFilterObserver<ConsumeContext<T>>.PostSend(ConsumeContext<T> context)
        {
            return _observer.PostConsume(context);
        }

        Task IFilterObserver<ConsumeContext<T>>.SendFault(ConsumeContext<T> context, Exception exception)
        {
            return _observer.ConsumeFault(context, exception);
        }
    }
}
