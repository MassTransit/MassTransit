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
    using System;
    using System.Threading;

    public class CompleteResponseHandler<TResponse> :
        ResponseHandlerBase<TResponse>
        where TResponse : class
    {
        readonly IRequestComplete _complete;

        public CompleteResponseHandler(string requestId, IRequestComplete complete,
            SynchronizationContext synchronizationContext, Action<IConsumeContext<TResponse>, TResponse> handler)
            : base(requestId, synchronizationContext, handler)
        {
            _complete = complete;
        }

        public CompleteResponseHandler(string requestId, IRequestComplete complete,
            SynchronizationContext synchronizationContext, Action<TResponse> handler)
            : base(requestId, synchronizationContext, handler)
        {
            _complete = complete;
        }

        protected override void Success(IConsumeContext<TResponse> context)
        {
            _complete.Complete(context.Message);
        }

        protected override void Failure(Exception exception)
        {
            _complete.Fail(exception);
        }
    }
}