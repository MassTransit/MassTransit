// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.TestFramework
{
    using System;
    using System.Threading.Tasks;


    public class TestObserver<T> :
        IObserver<ConsumeContext<T>>
        where T : class
    {
        readonly TaskCompletionSource<bool> _completed;
        readonly TaskCompletionSource<Exception> _exception;
        readonly TaskCompletionSource<ConsumeContext<T>> _value;

        public TestObserver(TaskCompletionSource<ConsumeContext<T>> value, TaskCompletionSource<Exception> exception, TaskCompletionSource<bool> completed)
        {
            _value = value;
            _exception = exception;
            _completed = completed;
        }

        public Task<ConsumeContext<T>> Value
        {
            get { return _value.Task; }
        }

        public TaskCompletionSource<bool> Completed
        {
            get { return _completed; }
        }

        public TaskCompletionSource<Exception> Exception
        {
            get { return _exception; }
        }

        public void OnNext(ConsumeContext<T> value)
        {
            _value.TrySetResult(value);
        }

        public void OnError(Exception error)
        {
            _exception.TrySetException(error);
        }

        public void OnCompleted()
        {
            _completed.TrySetResult(true);
        }
    }
}