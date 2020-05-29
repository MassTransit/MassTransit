namespace MassTransit.Util
{
    using System;
    using GreenPipes.Util;


    public class ObservableObserver<T> :
        IObservable<T>,
        IObserver<T>
    {
        readonly Connectable<IObserver<T>> _observers;

        public ObservableObserver()
        {
            _observers = new Connectable<IObserver<T>>();
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _observers.Connect(observer);
        }

        public void OnNext(T value)
        {
            _observers.ForEachAsync(x =>
            {
                x.OnNext(value);

                return TaskUtil.Completed;
            });
        }

        public void OnError(Exception error)
        {
            _observers.ForEachAsync(x =>
            {
                x.OnError(error);

                return TaskUtil.Completed;
            });
        }

        public void OnCompleted()
        {
            _observers.ForEachAsync(x =>
            {
                x.OnCompleted();

                return TaskUtil.Completed;
            });
        }
    }
}
