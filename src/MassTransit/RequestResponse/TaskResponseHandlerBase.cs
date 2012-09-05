// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RequestResponse
{
#if NET40
    using System;
    using System.Threading.Tasks;

    public abstract class TaskResponseHandlerBase<TResponse> :
        ResponseHandlerBase<TResponse>,
        TaskResponseHandler<TResponse>
        where TResponse : class
    {
        protected readonly TaskCompletionSource<TResponse> CompletionSource;

        protected TaskResponseHandlerBase(string requestId, Action<IConsumeContext<TResponse>, TResponse> handler)
            : base(requestId, null, handler)
        {
            CompletionSource = new TaskCompletionSource<TResponse>(TaskCreationOptions.None);
        }

        protected TaskResponseHandlerBase(string requestId, Action<TResponse> handler)
            : base(requestId, null, handler)
        {
            CompletionSource = new TaskCompletionSource<TResponse>(TaskCreationOptions.None);
        }

        Task TaskResponseHandler.Task
        {
            get { return CompletionSource.Task; }
        }

        Task<TResponse> TaskResponseHandler<TResponse>.Task
        {
            get { return CompletionSource.Task; }
        }

        Task<T> TaskResponseHandler.GetTask<T>()
        {
            var self = this as TaskResponseHandlerBase<T>;
            if (self == null)
                throw new InvalidOperationException("An incorrect task type was requested");

            return self.CompletionSource.Task;
        }

        public void HandleTimeout()
        {
            CompletionSource.TrySetCanceled();
        }
    }
#endif
}