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
    using Pipeline;
    using Util;


    public class TestConsumeObserver<T> :
        IConsumeMessageObserver<T>
        where T : class
    {
        readonly TaskCompletionSource<T> _consumeFaulted;
        readonly TaskCompletionSource<T> _postConsumed;
        readonly TaskCompletionSource<T> _preConsumed;

        public TestConsumeObserver(TaskCompletionSource<T> preConsumed, TaskCompletionSource<T> postConsumed,
            TaskCompletionSource<T> consumeFaulted)
        {
            _preConsumed = preConsumed;
            _postConsumed = postConsumed;
            _consumeFaulted = consumeFaulted;
        }

        public Task<T> PreConsumed => _preConsumed.Task;
        public Task<T> PostConsumed => _postConsumed.Task;
        public Task<T> ConsumeFaulted => _consumeFaulted.Task;

        Task IConsumeMessageObserver<T>.PreConsume(ConsumeContext<T> context)
        {
            _preConsumed.TrySetResult(context.Message);

            return TaskUtil.Completed;
        }

        Task IConsumeMessageObserver<T>.PostConsume(ConsumeContext<T> context)
        {
            _postConsumed.TrySetResult(context.Message);

            return TaskUtil.Completed;
        }

        Task IConsumeMessageObserver<T>.ConsumeFault(ConsumeContext<T> context, Exception exception)
        {
            _consumeFaulted.TrySetException(exception);

            return TaskUtil.Completed;
        }
    }
}