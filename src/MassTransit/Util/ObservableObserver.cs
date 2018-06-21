// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
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