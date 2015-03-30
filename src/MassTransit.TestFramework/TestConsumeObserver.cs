// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Pipeline;


    public class TestConsumeObserver<T> :
        IConsumeMessageObserver<T>
        where T : class
    {
        readonly TaskCompletionSource<T> _dispatchFaulted;
        readonly TaskCompletionSource<T> _postDispatched;
        readonly TaskCompletionSource<T> _preDispatched;

        public TestConsumeObserver(TaskCompletionSource<T> preDispatched, TaskCompletionSource<T> postDispatched,
            TaskCompletionSource<T> dispatchFaulted)
        {
            _preDispatched = preDispatched;
            _postDispatched = postDispatched;
            _dispatchFaulted = dispatchFaulted;
        }

        public Task PreDispatched
        {
            get { return _preDispatched.Task; }
        }

        public Task PostDispatched
        {
            get { return _postDispatched.Task; }
        }

        public Task DispatchedFaulted
        {
            get { return _dispatchFaulted.Task; }
        }

        async Task IConsumeMessageObserver<T>.PreConsume(ConsumeContext<T> context)
        {
            _preDispatched.TrySetResult(context.Message);
        }

        async Task IConsumeMessageObserver<T>.PostConsume(ConsumeContext<T> context)
        {
            _postDispatched.TrySetResult(context.Message);
        }

        async Task IConsumeMessageObserver<T>.ConsumeFault(ConsumeContext<T> context, Exception exception)
        {
            _dispatchFaulted.TrySetException(exception);
        }
    }
}