// Copyright 2011 Chris Patterson, Dru Sellers
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
namespace Automatonymous
{
    using Activities;
    using Binders;


    public static class RetryLaterExtensions
    {
        /// <summary>
        /// Resends the message to the input queue so that it can be received again later.
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static EventActivityBinder<TInstance, TData> RetryLater<TInstance, TData, TMessage>(
            this EventActivityBinder<TInstance, TData> source)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new RetryLaterActivity<TInstance, TData>());
        }
    }
}